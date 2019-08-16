using System.Collections;
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
    // private System.Threading.Timer timer_gaming;

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

    // 定义一个委托 标志为 connectCallback，下面的Connect函数中最后两个参数,c为小写的函数没有使用，如果需要外部传入参数再改
    public delegate void connectCallback();

    public void Connect(connectCallback connectCallback = null, connectCallback connectFailCallback = null){
        
        try{
            
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
    
    // [Thread] 接收并存储数据
    private void ReceiveFromServer(){
        while(!threadstop){
            if(!socket.Connected){
                Debug.Log("Failed to connect when try to recieve from server");
                socket.Close();
                break;
            }
            try{
                string recv = SocketRecv.socketrecv(ref this.socket);
                if(recv == null){
                    Debug.Log("从socket里边拿出来就是空的");
                }
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
                    Debug.Log("分解后剩余的数据（未加入waitList）" + getstr);
                }else{
                    Debug.Log("recvList中存在空数据 或 从空recvList中取数据");
                }

                // 检查recvList
                if(recvList.Count < 50){
                    Debug.Log("recvList safe");
                }
                else{
                    Debug.Log("recvList unsafe !!!");
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
                    Debug.Log("waitList中取出的"+getstr);
                    
                    if(getstr == "all player connected"){
                        this.timer_heartbeats.Change(-1,0);
                        this.isGaming = true;
                        // tcy 在下面update加的标识，不知道有啥用。。
                        running=true;                        
                        LockStepController.Instance.StartGame();
                        Debug.Log("接收到 all player connected，开始游戏 ^_^!");
                    }
                    else{
                        JObject res = JObject.Parse(getstr);
                        ResTitle title = res.ToObject<ResTitle>();
                        Debug.Log("接收到的response的类型："+title.datatype);
                        if(title.datatype == 1){
                            Debug.Log("调用 RecieveActions 接口");
                            Response all = res.ToObject<Response>();
                            if(all.content != null){
                                foreach(var resgame in all.content){
                                    LockStepController.Instance.RecieveActions(all.Roundnum, resgame.UserID, resgame.Opinions.ToArray()); 
                                }  
                            }
                            this.SendAck(all.Roundnum);
                                                      
                            
                        }else if(title.datatype == 4){
                            Debug.Log("调用 ConfirmTrun 接口");
                            LockStepController.Instance.ConfirmTurn(title.Roundnum);

                        }else{
                            Debug.Log("接收到未知类型的reponse");
                        }
                    }                        

                }else{
                    Debug.Log("waitList中存在空数据 或 从空waitList中取数据");
                }
                
                // 检查waitList
                if(recvList.Count < 50){
                    Debug.Log("waitList safe");
                }
                else{
                    Debug.Log("waitList unsafe !!!");
                }                
                
            }
        }
    }

    // 留出的接口 用于接收等待发送的游戏操作
    public void AddGamingInfo(Opinion op){
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
        Debug.Log("sending HeartBeats...");   
        try{            
            string str = "heart beats";
            byte[] ready = SocketSend.StringtoByte(str);   

            // this.sw.Write(ready);
            // this.sw.Flush();
            this.socket.Send(ready);

        }catch{
            Debug.Log("Failed when send Heartbeats");
        }     
    }

    // Start is called before the first frame update
    void Start()
    {        
        this.Connect();
    }



    private float AccumilatedTime = 0f;
    private float FrameLength = 0.05f;
    private float TurnLength = 0.2f;
    private int GameFrame = 0;
    private int GameFramesPerLocksetpTurn = 4;
    private int GameTurn = 0;

    private bool running=false;

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKey(KeyCode.Escape)){
            Debug.Log("按下了Esc键！！！！!!!!!!!!!!!!!");
            // if(timer_gaming != null){
            //     timer_gaming.Change(-1,0);
            //     Debug.Log("执行完终止计时器！！！！!!!!!!!!!!!!!");
            // }
            if(timer_heartbeats != null){
                timer_heartbeats.Change(-1,0);
                Debug.Log("执行完终止计时器！！！！!!!!!!!!!!!!!");
            }

            this.threadstop = true;
            running=false;
        }
        if(running){
            Debug.Log("is running");
            if(this.isGaming){
                Debug.Log("is Gaming");
                this.AccumilatedTime = this.AccumilatedTime + Time.deltaTime; // deltatime 是每一帧的时间,即每此update间隔的时间

                while(this.AccumilatedTime > this.FrameLength){
                    GameFrameTurn();
                    this.AccumilatedTime -= this.FrameLength;

                }
            }
        }


    }

    private void GameFrameTurn(){
        Debug.Log("GameFrame:"+this.GameFrame);
        this.GameFrame++;
        if(this.GameFrame == this.GameFramesPerLocksetpTurn) {
            this.GameFrame = 0;
            this.GameTurn++;
            Debug.Log("GameTurn"+this.GameTurn);
            lock(lockerChange){
                useSend = false;
            }
            if(useSend){
                lock(sendList_2){
                    SendInfo(ref sendList_2);
                    sendList_2.Clear();
                }
            }
            else{
                lock(sendList_1){
                    SendInfo(ref sendList_1);
                    sendList_1.Clear();
                }
            }
        }


    }

    private void SendInfo(ref Queue<Opinion> sendList){
        Debug.Log("Sending GamingInfo");
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
        string str = JsonConvert.SerializeObject(info);
        Debug.Log("Send str:"+str);
        byte[] buffer = SocketSend.StringtoByte(str);
        Debug.Log("byte[]:"+buffer);
        this.socket.Send(buffer);
    }

    private void SendAck(int turnNum){
        Debug.Log("Sending ack");
        AckTitle tit = new AckTitle();
        tit.Roundnum = turnNum;
        tit.UserID = "holdonbush";
        tit.datatype = 3;
        string str = JsonConvert.SerializeObject(tit);
        byte[] buffer = SocketSend.StringtoByte(str);
        this.socket.Send(buffer);
    }
}
