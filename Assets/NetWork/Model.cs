using System.Collections;
using System.Collections.Generic;

namespace NetModels
{
    public class Opinion{
        public string type;
        public string desc;
        public string target;
        public string position;
        public int framenum;
    }
    public class Hash{
        public string player1Hash;
        public string player2Hash;
        public string player3Hash;
        public string player4Hash;
        public string player5Hash;
    }
    
    
    public class ResGamingInfo{
        public string UserID;
        public Queue<Opinion> Opinions;
    }


    // 发送数据
    public class GamingInfo{
        public int datatype;
        public string UserID;
        public int Roundnum;
        public Queue<Opinion> Opinions;
        public Hash hash;
    }


    // 接收数据
    public class Response{
        public int datatype;
        public string result;
        public int Roundnum;
        public Queue<ResGamingInfo> content; 
    }

    // 发送确认
    public class AckTitle{
        
        public int datatype;
        public string UserID;
        public int Roundnum;
    }

    // 接收确认
    public class ResTitle{
        public int datatype;
        public string result;
        public int Roundnum;
    }

    // 位置
    public class Pos{
        public string UserId;
        public string Position;
    }

    // 状态同步
    public class Status{
        public int datatype;
        public string result;
        public int Roundnum;
        public Queue<Pos> AllStatus; 
    }
    

}