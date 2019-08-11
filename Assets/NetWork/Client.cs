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


public class Client : MonoBehaviour
{
    Socket socket;

    public string serverIp;
    public int serverPort;

    private Thread threadRecv;
    private Thread threadSeparation;
    private Thread threadParase;

    private System.Threading.Timer timer_heartbeats; //1000ms = 1s;
    private System.Threading.Timer timer_gaming;

    // NetworkStream ns;
    // StreamWriter sw;

    private static Queue<string> recvList = new Queue<string>();
    private Queue<string> waitList = new Queue<string>();

    private static object locker = new object();

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
            }else{
                timer_heartbeats = new System.Threading.Timer(SendHeartBeats, null, 0, 5);
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
            // 链接成功，启动接收信息线程
            threadRecv = new Thread(new ThreadStart(ReceiveFromServer));
            threadRecv.IsBackground = false;
            threadRecv.Start();

            threadSeparation = new Thread(new ThreadStart(Separation));
            threadSeparation.IsBackground = false;
            threadSeparation.Start();

            threadParase = new Thread(new ThreadStart(Parase));
            threadParase.IsBackground = false;
            threadParase.Start();
            
        }
        else{
            Debug.Log("Connected fialed!!!");

            //SendGameInfo();

            socket.EndConnect(ar);
        }

    }
    
    // [Thread] 接收并存储数据
    private void ReceiveFromServer(){
        while(true){
            if(!socket.Connected){
                Debug.Log("Failed to connect when try to recieve from server");
                socket.Close();
                break;
            }
            try{
                string recv = SocketRecv.socketrecv(ref this.socket);
                // 存储
                lock (locker){                    
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
        while(true){
            string getstr = null;
            
            if(recvList.Count > 0){
                lock (locker){
                    // getstr = recvList[0];
                    // recvList.RemoveAt(0);
                    getstr = recvList.Dequeue();
                }
                if(recvList.Count < 50){
                    Debug.Log("recvList safe");
                }
                else{
                    Debug.Log("recvList unsafe !!!");
                }
                Debug.Log("接受到的消息："+getstr);
                
                getstr = SocketRecv.split(getstr, ref this.waitList, SocketRecv.getParameter());
                Debug.Log("一次解析完成");
            }

            Debug.Log("分解后剩余的数据（未加入waitList）" + getstr);

            
        }

    }

    // [Thread] 解析分解完的数据并根据数据进行相应操作
    private void Parase(){
        string getstr = null;
        while(true){
            if(this.waitList.Count > 0){
                // getstr = this.waitList[0];
                // this.waitList.RemoveAt(0);
                getstr = this.waitList.Dequeue();
                Debug.Log("waitList中取出的"+getstr);

                if(getstr ==  null){
                    Debug.Log("居然有一个空数据！！！！！"+DateTime.Now.TimeOfDay.ToString());
                    continue;
                }

                
                if(getstr !=null && getstr == "all player connected"){
                    this.timer_heartbeats.Change(-1,0);
                    timer_gaming = new System.Threading.Timer(SendGameInfo, null, 0, 10); //1000ms = 1s
                    Debug.Log("&&&&& 发送GamingInfo计时器成功启动，解析多条数据没有问题 ^_^!");
                }
                else{
                    Debug.Log("&&&&& 发送GamingInfo计时器没有启动，解析数据的问题！！");
                }

                if(getstr.IndexOf("{\"result\"") != -1){
                    GamingInfo info = (GamingInfo)Json.StringtoObj(getstr);
                    if(info != null) Debug.Log("成功获得一个对象");
                }
                else{
                    Debug.Log("存在未能有效使用的单句 ！！");
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
    // [Timer] 发送 GamingInfo (游戏数据)
    private void SendGameInfo(System.Object state){
        Debug.Log("sending Game Info...");
        try{
            GamingInfo info  = (GamingInfo)Tests.assembly();
            string jsonData = Json.ObjtoString(info);
            
            Debug.Log(jsonData);
            byte[] ready = SocketSend.StringtoByte(jsonData);
               
            // this.sw.Write(ready);
            // this.sw.Flush();
            this.socket.Send(ready);
        }catch{
            Debug.Log("Failed when send Gameinfo");
        }
    }



    // Start is called before the first frame update
    void Start()
    {        
        this.Connect();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Escape)){
            if(timer_gaming != null){
                timer_gaming.Change(-1,0);
            }
            else if(timer_heartbeats != null){
                timer_heartbeats.Change(-1,0);
            }
            // threadRecv.exit = true;  // 终止线程thread
            // threadRecv.join();
            threadSeparation.Abort();
            threadParase.Abort();
        }
    }
}
