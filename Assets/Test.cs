using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;

public class Test:MonoBehaviour{
    Socket socket;
    JObject obj;
    bool s=false;
    void Start() {
        obj=new JObject();
        obj["UserID"]="ttttcy";
        JArray array=new JArray();
        JObject o0=new JObject();
        o0["type"]="move";
        o0["desc"]="130*144";
        o0["target"]="none";
        o0["position"]="0*0";
        array.Add(o0);
        obj["Opinions"]=array;
        JObject hash=new JObject();
        hash["player1-hash"]="dfasofoh12441414";
        hash["player2-hash"]="sasdsafsaferewrk";
        hash["player3-hash"]="ssssssssssssssss";
        hash["player4-hash"]="eeeeeeeeeeeeeeee";
        hash["player5-hash"]="rrrrrrrrrrrrrrrr";
        obj["hash"]=hash;
        Connection();
    }

    void FixedUpdate() {
        if(s){
            send();
            receive();
        }
        else{
            sendHeart();
            receive();
        }
        if(Input.GetKey(KeyCode.Escape)){
            socket.Close();
        }
    }

    public void Connection(){
        socket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        socket.Connect("120.79.240.163",8080);
    }

    public void send(){
        Debug.Log("send:"+obj.ToString());
        byte[] sendBytes=System.Text.Encoding.UTF8.GetBytes(obj.ToString());
        byte[] length=getByte(sendBytes.Length);
        byte[] r=mergeByte(length,sendBytes);
        socket.Send(r);
    }

    public void sendHeart(){
        byte[] sendBytes=System.Text.Encoding.UTF8.GetBytes("heart beats");
        byte[] length=getByte(sendBytes.Length);
        byte[] r=mergeByte(length,sendBytes);
        socket.Send(r);
    }
    public void receive(){
        byte[] readBuff=new byte[2048];
        int count=socket.Receive(readBuff);
        string msg=System.Text.Encoding.UTF8.GetString(readBuff,0,count);
        Debug.Log(msg);
        if(msg=="all player connected"){
            s=true;
        }
    }

    private byte[] getByte(int value){
        byte[] src=new byte[4];
        src[3] = (byte)((value >> 24) & 0xFF);
        src[2] = (byte)((value >> 16) & 0xFF);
        src[1] = (byte)((value >> 8) & 0xFF);
        src[0] = (byte)(value & 0xFF);
        return src;
    }

    private byte[] mergeByte(byte[] a,byte[] b){
        byte[] result=new byte[a.Length+b.Length];
        a.CopyTo(result,0);
        b.CopyTo(result,a.Length);
        return result;
    }
}
