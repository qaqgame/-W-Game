using System.Collections;
using System.Collections.Generic;

namespace NetModels
{
    public class Opinion{
        public string type;
        public string desc;
        public string target;
        public string position;
    }
    public class Hash{
        public string player1Hash;
        public string player2Hash;
        public string player3Hash;
        public string player4Hash;
        public string player5Hash;
    }
    public class GamingInfo{
        public string UserID;
        public List<Opinion> Opinions;
        public Hash hash;
    }

}