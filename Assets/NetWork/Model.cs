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
    public class GamingInfo{
        public int datatype;
        public string UserID;
        public int Roundnum;
        public Queue<Opinion> Opinions;
        public Hash hash;
    }

    public class ResGamingInfo{
        public string UserID;
        public Queue<Opinion> Opinions;
    }

    public class ResTitle{
        public int datatype;
        public string result;
        public int Roundnum;
    }

    public class Response{
        public int datatype;
        public string result;
        public int Roundnum;
        public Queue<ResGamingInfo> content; 
    }

    public class SendTitle{
        
        public int datatype;
        public string UserID;
        public int Roundnum;
    }

}