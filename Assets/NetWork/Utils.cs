using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NetModels;
using Newtonsoft.Json;
using System.Net.Sockets;

namespace NetUtils
{
    public class SocketSend{
        // 组装为 byte[] 的辅助函数，用于添加数据长度防止粘包
        private static byte[] getByte(int value){
            byte[] src=new byte[4];
            src[3] = (byte)((value >> 24) & 0xFF);
            src[2] = (byte)((value >> 16) & 0xFF);
            src[1] = (byte)((value >> 8) & 0xFF);
            src[0] = (byte)(value & 0xFF);
            return src;
        }
        // 从 string 组装为待发送的 byte[]
        public static byte[] StringtoByte(string jsonData){
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(jsonData);
            byte[] head = SocketSend.getByte(buffer.Length);
            byte[] ready = new byte[head.Length+buffer.Length];
            head.CopyTo(ready, 0);
            buffer.CopyTo(ready, head.Length);
            return ready;
        }
    }
    
    public class SocketRecv{
        // 从 socket 接收为 原始string 
        public static string socketrecv(ref Socket socket){
            byte[] readbuff = new byte[4096];
            int count = socket.Receive(readbuff);
            string recv = System.Text.Encoding.Default.GetString(readbuff,0,count);
            return recv;
        } 
        
        // 创建参数列表 每个参数要求超过5字符！！（用于分解接收的原始 string）
        public static string[] getParameter(){
            string para_1 = "{\"datatype\":1";
            string para_3 = "{\"datatype\":4";
            string para_2 = "all player connected";
            // string para_2 = "";
            string[] paraList = new string[3];
            paraList[0] = para_1;
            paraList[1] = para_2;
            paraList[2] = para_3;
            return paraList;
        }

        private static int IndexHelp(string getstr, string[] para, int from = 0){
            int num = 1;
            int min = getstr.IndexOf(para[0], from);
            Debug.Log("IndexHelp 第一次的min值"+min);

            while(num < para.Length){
                int temp = getstr.IndexOf(para[num], from);
                if(min == -1){
                    min = temp;
                }else if(temp != -1 && temp < min){
                    min = temp;
                }
                num++;
            }
            Debug.Log("IndexHelp 返回的min值"+min);
            return min;
        }
        
        // 从接收的原始 string 使用参数列表 分解粘连信息为单句
        public static string split(string getstr, ref Queue<string> waitList, string[] para){
            
            int start;
            int end = 0;

            if(waitList.Count < 50){
                Debug.Log("waitList safe");
            }else{
                Debug.Log("waitList unsafe !!!");
            }


            while(end != -1){
                start = IndexHelp(getstr, para);
                if(start == -1) {
                    Debug.Log("是break出去的！！！");
                    break;

                }
                end = IndexHelp(getstr, para, start+5);
                if(end != -1){
                    string substr = getstr.Substring(start, end - start);
                    getstr = getstr.Remove(start, end - start);
                    Debug.Log("分解后加入waitList的数据"+substr);
                    lock(waitList){
                        waitList.Enqueue(substr);
                    }                    
                }else if(end == -1){
                    string substr = getstr.Substring(start);
                    getstr = getstr.Remove(start);
                    Debug.Log("分解后加入waitList的数据"+substr);
                    lock(waitList){
                        waitList.Enqueue(substr);
                    }                    
                }
            }
            return getstr;
        }      
    }


    // public class Tests{
    //     // 组装待发送的游戏数据  测试用
    //     public static object assembly(){
    //         Opinion Opinion_1 = new Opinion(){type  = "move", desc = "130*144", target = "none", position = "0*0"};
    //         var OpinionList = new List<Opinion>();
    //         OpinionList.Add(Opinion_1);
    //         Hash hash_1 = new Hash(){
    //             player1Hash = "dfasofoh12441414",
    //             player2Hash = "dfasofoh12441414",
    //             player3Hash = "dfasofoh12441414",
    //             player4Hash = "dfasofoh12441414",
    //             player5Hash = "dfasofoh12441414"};
    //         GamingInfo info = new GamingInfo(){ UserID = "holdonbush", Opinions = OpinionList, hash = hash_1 };
    //         return info;
    //     }
    // }
}
