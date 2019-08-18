﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net.Sockets;
using System.Threading;
//using System.Threading.Timer;
using System;
using System.IO;

using NetModels;
using NetUtils;
//using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class Client : MonoBehaviour
{
    // 帧时间
    [SerializeField]
    public static float frameStep = 0.005f;  // 每0.05s = 50ms 一帧

    // socket 参数
    Socket socket;
    public string serverIp;
    public int serverPort;

    // thread
    private Thread threadRecv;
    private Thread threadSeparation;
    private Thread threadParase;

    // 标志
    private bool threadstop = false;
    private bool isGaming = false;

    // 计时器
    private System.Threading.Timer timer_heartbeats; //1000ms = 1s;
    private System.Threading.Timer timer_gaming;

    // NetworkStream ns;
    // StreamWriter sw;

    // 存储队列
    private static Queue<string> recvList = new Queue<string>();
    private static Queue<string> waitList = new Queue<string>();
    private static Queue<Opinion> sendList_1 = new Queue<Opinion>();
    private static Queue<Opinion> sendList_2 = new Queue<Opinion>();
    private static bool useSend = true;

    // 锁
    private static object lockerChange = new object();
    private static object lockerRecv = new object();
    private static object lockerSend = new object();


    private int countType1 = 0;
    private int countType2 = 0;
    private int countAll = 0;


    // 定义一个委托 标志为 connectCallback，下面的Connect函数中最后两个参数,c为小写的函数没有使用，如果需要外部传入参数再改
    public delegate void connectCallback();

    public void Connect(connectCallback connectCallback = null, connectCallback connectFailCallback = null){
        
        try{
            
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SendBufferSize = 10240;
            socket.NoDelay = true;
            // 这里是异步链接，但目前没有作用

            IAsyncResult result = socket.BeginConnect(this.serverIp, this.serverPort, new AsyncCallback(this.ConnectCallback), socket);

            DateTime beforeDT = System.DateTime.Now;
            bool success = result.AsyncWaitHandle.WaitOne(5000,true);
            DateTime afterDT = System.DateTime.Now;
            TimeSpan ts = afterDT.Subtract(beforeDT);
            
            Debug.Log("计时："+ts.TotalMilliseconds);
            Debug.Log("检查连接："+ socket.Connected);
            // 这步是判断是否超时而不判断连接结果
            if(!success){
                socket.Close();
                Debug.Log("Connected time out!!!");
            }
                
            //this.ns = new NetworkStream(this.socket);
            //this.sw = new StreamWriter(ns);
            
        }
        catch (Exception ex){
            Debug.Log(ex.ToString());
        }
    }


    // 异步操作相当于 [Thread]？？？ 链接成功时调用
    private void ConnectCallback(IAsyncResult ar){
        // ar.AsysncState 是拿到BeginConnect最后一个参数，也就是socket
        Socket socket = (Socket)ar.AsyncState;
        if(socket.Connected){
            Debug.Log("Connected success");

            // 发送心跳包
            timer_heartbeats = new System.Threading.Timer(SendHeartBeats, null, 0, 5);
            
            // 启动接收信息线程
            this.threadRecv = new Thread(new ThreadStart(ReceiveFromServer));
            this.threadRecv.IsBackground = true;
            this.threadRecv.Start();

            // 启动分解信息线程
            this.threadSeparation = new Thread(new ThreadStart(Separation));
            this.threadSeparation.IsBackground = true;
            this.threadSeparation.Start();

            // 启动解析信息线程
            this.threadParase = new Thread(new ThreadStart(Parase));
            this.threadParase.IsBackground = true;
            this.threadParase.Start();
            
        }
        else{
            Debug.Log("Connected fialed!!!");

            socket.EndConnect(ar);
        }

    }
    

    
    private class TestSocket{
        public Socket socket;
        public byte[] buffer;

        public TestSocket(ref Socket socket){
            this.socket = socket;
            this.buffer  = new byte[1024];
        }
    }
    // [Thread] 接收并存储数据
    private void ReceiveFromServer(){
        while(!threadstop){
            if(!socket.Connected){
                Debug.Log("Failed to connect when try to recieve from server");
                socket.Close();
                break;
            }
            try{
                // TestSocket ts = new TestSocket(ref this.socket);
                // try{
                //     socket.BeginReceive(ts.buffer, 0, ts.buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), ts);
                // }
                // catch(SocketException ex){
                //     Debug.Log("BeginRecv 失败");
                // }
                
                string recv = SocketRecv.socketrecv(ref this.socket);
                if(recv == null){
                    //Debug.Log("从socket里边拿出来就是空的");
                }
                Debug.Log("接收消息时间"+DateTime.Now.TimeOfDay.ToString()+"内容："+recv);
                // 存储
                lock (lockerRecv){                    
                    recvList.Enqueue(recv);
                }
            }
            catch(SocketException ex){
                Debug.Log("Failed when write to buffer"+DateTime.Now.TimeOfDay.ToString());
            }
        }
    }

    private void ReceiveCallback(IAsyncResult ar){
        TestSocket ts = (TestSocket)ar.AsyncState;
        int count  = ts.socket.EndReceive(ar);
        if(!ts.socket.Connected) Debug.Log("检查socket时发现链接断开");
        if (count > 0)
        {
            string recv = System.Text.Encoding.Default.GetString(ts.buffer,0,count);
        
        
            Debug.Log("BeginRecv接收到的："+recv+"时间："+DateTime.Now.TimeOfDay.ToString());
            lock(lockerRecv){
                recvList.Enqueue(recv);
            }

        }else{
            Debug.Log("接收失败");
        }
        
        
    }
    // [Thread] 分解接收到的多条数据为单条
    private void Separation(){
        while(!threadstop){
            string getstr = null;

            if(recvList.Count > 0){
                lock(lockerRecv){
                    if(recvList.Count > 0){
                        getstr = recvList.Dequeue();
                    }
                }

                if(getstr != null){
                    Debug.Log("recvList中取出："+getstr);
                    getstr = SocketRecv.split(getstr, ref waitList, SocketRecv.getParameter());
                    //Debug.Log("分解后剩余的数据（未加入waitList）" + getstr);
                }else{
                    //Debug.Log("recvList中存在空数据 或 从空recvList中取数据");
                }

                // 检查recvList
                if(recvList.Count < 50){
                    //Debug.Log("recvList safe");
                }
                else{
                    //Debug.Log("recvList unsafe !!!");
                }
                
            }
            
        }

    }

    // [Thread] 解析分解完的数据并根据数据进行相应操作
    private void Parase(){
        while(!threadstop){
            string getstr = null;
            if(waitList.Count > 0){
                lock(waitList){
                    if(waitList.Count > 0) getstr = waitList.Dequeue();
                }
                
                if(getstr != null){
                    //Debug.Log("waitList中取出的"+getstr);
                    
                    if(getstr == "GameStart"){
                        this.timer_heartbeats.Change(-1,0);
                        this.timer_gaming = new System.Threading.Timer(SendInfo, null, 0, 20);
                        this.isGaming = true;
                        // tcy 在下面update加的标识，不知道有啥用。。
                        running=true;                        
                        LockStepController.Instance.StartGame();
                        Debug.Log("接收到 all player connected，开始游戏 ^_^!");
                        //Debug.Log("all player时间："+DateTime.Now.TimeOfDay.ToString());
                    }
                    else{
                        JObject res = JObject.Parse(getstr);
                        ResTitle title = res.ToObject<ResTitle>();
                        //Debug.Log("接收到的response的类型："+title.datatype);
                        if(title.datatype == 1){
                            ////Debug.Log("调用 RecieveActions 接口");
                            Response all = res.ToObject<Response>();
                            if(all.content != null){
                                foreach(var resgame in all.content){
                                    LockStepController.Instance.RecieveActions(all.Roundnum, resgame.UserID, resgame.Opinions.ToArray()); 
                                }  
                            }
                            this.SendAck(all.Roundnum);
                                                      
                            
                        }else if(title.datatype == 4){
                            Debug.Log("调用 ConfirmTrun 接口，传入回合："+title.Roundnum);
                            LockStepController.Instance.ConfirmTurn(title.Roundnum);

                        }else{
                            Debug.Log("接收到未知类型的reponse");
                        }
                    }                        

                }else{
                    //Debug.Log("waitList中存在空数据 或 从空waitList中取数据");
                }
                
                // 检查waitList
                if(recvList.Count < 50){
                    //Debug.Log("waitList safe");
                }
                else{
                    //Debug.Log("waitList unsafe !!!");
                }                
                
            }
        }
    }
    // 接口
    public void AddReady(){
        Debug.Log("tcy告诉我Ready");
        string str = "Ready";
        byte[] buffer = SocketSend.StringtoByte(str);
        this.socket.BeginSend(buffer, 0, buffer.Length, 0,new AsyncCallback(SendCallback), this.socket);

    }

    private static Dictionary<int, int> testUseTime = new Dictionary<int, int>();
    private int countNumAPI = 0;
    private int countNumSend = 0;
    
    // 留出的接口 用于接收等待发送的游戏操作
    public void AddGamingInfo(Opinion op){
        this.countNumAPI++;
        testUseTime.Add(this.countNumAPI, System.Environment.TickCount);
        // if(this.GameTurn - LockStepController.Instance.LockStepTurnID > 3){
        //     return;
        // }
        lock(lockerChange){
            if(useSend){
                lock(sendList_1){
                    op.framenum = this.GameFrame;
                    sendList_1.Enqueue(op);
                }
            }else{
                lock(sendList_2){
                    op.framenum = this.GameFrame;
                    sendList_2.Enqueue(op);
                }
            }
        }       
        
    }
    
    // [Timer] 发送 HeartBeats
    private void SendHeartBeats(System.Object state){
        //Debug.Log("sending HeartBeats...");   
        try{            
            string str = "heart beats";
            byte[] ready = SocketSend.StringtoByte(str);   

            // this.sw.Write(ready);
            // this.sw.Flush();
            this.socket.Send(ready);

        }catch{
            //Debug.Log("Failed when send Heartbeats");
        }     
    }

    // Start is called before the first frame update
    void Start()
    {
        this.Connect();
    }



    private float AccumilatedTime = 0f;
    private float FrameLength = frameStep; // 原来是0.05f
    private float TurnLength = frameStep * GameFramesPerLocksetpTurn;
    private int GameFrame = 0;
    public static int GameFramesPerLocksetpTurn = 4;
    private int GameTurn = 0;

    private bool running=false;

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKey(KeyCode.Escape)){
            //Debug.Log("按下了Esc键！！！！!!!!!!!!!!!!!");
            // if(timer_gaming != null){
            //     timer_gaming.Change(-1,0);
            //     //Debug.Log("执行完终止计时器！！！！!!!!!!!!!!!!!");
            // }
            if(timer_heartbeats != null){
                timer_heartbeats.Change(-1,0);
                //Debug.Log("执行完终止计时器！！！！!!!!!!!!!!!!!");
            }
            if(timer_gaming != null){
                timer_gaming.Change(-1,0);
            }
            this.socket.Close();
            this.threadstop = true;
            running=false;
            Debug.Log(" 已发送数量："+this.countAll+" type1 发送数量："+this.countType1+" ack发送数量："+this.countType2+" 锁完成次数："+this.testlocker+" 跳过的回合数："+this.countjump+" 减少的发送数据次数："+this.reduce);
        }
        if(running){
            // Debug.Log("is running");
            if(this.isGaming){
                //Debug.Log("回合开始时间："+DateTime.Now.TimeOfDay.ToString());
                // Debug.Log("is Gaming");
                this.AccumilatedTime = this.AccumilatedTime + Time.deltaTime; // deltatime 是每一帧的时间,即每此update间隔的时间

                while(this.AccumilatedTime > this.FrameLength){
                    GameFrameTurn();
                    this.AccumilatedTime -= this.FrameLength;

                }
            }
        }


    }

    private void GameFrameTurn(){
        // Debug.Log("GameFrame:"+this.GameFrame);
        if(this.GameFrame == GameFramesPerLocksetpTurn) {
            return;            
        }
        this.GameFrame++;
    }

    private int testlocker = 0;
    private bool isSending = false;
    private int countjump = 0;
    private void SendInfo(System.Object state){
        if(this.GameTurn - LockStepController.Instance.LockStepTurnID > 10){
            return;
        }
        if(this.isSending){
            this.countjump++;
            return;

        }
        this.isSending = true;
        Debug.Log("发送前的当前回合数："+this.GameTurn);
        lock(lockerChange){
            useSend = !useSend;
        }
        if(useSend){
            lock(sendList_2){
                Send(ref sendList_2);
                this.testlocker++;
                sendList_2.Clear();
            }
        }
        else{
            lock(sendList_1){
                Send(ref sendList_1);
                this.testlocker++;
                sendList_1.Clear();
            }
        }
        
        // if(this.GameTurn - LockStepController.Instance.LockStepTurnID <= 3){            
            this.GameTurn++;
            this.GameFrame = 0;
        // }

        // 保证计时器发送send上一次没执行完就不发
        this.isSending = false;

        this.countNumSend++;
        
        Debug.Log("第"+this.countNumSend+"次调用API开始到发送结束用时(ms)："+ (System.Environment.TickCount - testUseTime[this.countNumSend]).ToString());
        testUseTime.Remove(this.countNumSend);    
    }
    private int lastTurn = -1;
    private int reduce = 0;
    private void Send(ref Queue<Opinion> sendList){
        
        this.countType1++;
        //Debug.Log("Sending GamingInfo"+this.countType1);
        GamingInfo info = new GamingInfo();
        info.Roundnum = this.GameTurn;
        info.UserID = "holdonbush";
        info.datatype = 2;
        info.Opinions = sendList;
        Hash hash_1 = new Hash(){
                player1Hash = "dfasofoh12441414",
                player2Hash = "dfasofoh12441414",
                player3Hash = "dfasofoh12441414",
                player4Hash = "dfasofoh12441414",
                player5Hash = "dfasofoh12441414"};
        info.hash = hash_1;
        //string start = DateTime.Now.TimeOfDay.ToString();
        if(info.Roundnum == this.lastTurn && info.Opinions.Count == 0){
            Debug.Log("判断数据："+info.Roundnum+"上一回合："+this.lastTurn);
            this.reduce++;
            return;
        }

        string str = JsonConvert.SerializeObject(info);
        //string end = DateTime.Now.TimeOfDay.ToString();
        //Debug.Log("序列化开始时间"+start);
        //Debug.Log("序列化结束时间"+end);
        // 总计0.2s,时间过长
        
        Debug.Log("Send str:"+str);
        byte[] buffer = SocketSend.StringtoByte(str);
        // this.socket.Send(buffer);
        
        // string testsocket = "{\"datatype\":2,\"UserID\":\"holdonbush\",\"Roundnum\":12,\"Opinions\":[{\"type\":\"move\",\"desc\":\"130*144\",\"target\":\"none\",\"position\":\"0*0\",\"framenum\":4},{\"type\":\"skill\",\"desc\":\"123\",\"target\":\"144*144\",\"position\":\"100*70\",\"framenum\":5}],\"hash\":{\"player1Hash\":\"dfasofoh12441414\",\"player2Hash\":\"safdjojasfojofjs\",\"player3Hash\":\"fdsafoowjfojaofs\",\"player4Hash\":\"jfoasjoejfoajofj\",\"player5Hash\":\"ojojaodsjfiiofaj\"}}";
        // byte[] testbyte = SocketSend.StringtoByte(testsocket);
        
            
        this.socket.BeginSend(buffer, 0, buffer.Length, 0,new AsyncCallback(SendCallback), this.socket);
        this.lastTurn = info.Roundnum;
    }

    private void SendCallback(IAsyncResult ar){        
        this.countAll++;
        // ar.AsysncState 是拿到BeginConnect最后一个参数，也就是socket
        Socket socket = (Socket)ar.AsyncState;
        int bytesSent = socket.EndSend(ar);
        //Debug.Log("发送字节长度"+bytesSent+"发送次数："+this.countAll);
    }

    private void SendAck(int turnNum){
        this.countType2++;
        Debug.Log("Sending ack 回合数："+turnNum);
        AckTitle tit = new AckTitle();
        tit.Roundnum = turnNum;
        tit.UserID = "holdonbush";
        tit.datatype = 3;
        string str = JsonConvert.SerializeObject(tit);
        byte[] buffer = SocketSend.StringtoByte(str);
        this.socket.BeginSend(buffer, 0, buffer.Length, 0,new AsyncCallback(SendCallback), this.socket);
    }
}
