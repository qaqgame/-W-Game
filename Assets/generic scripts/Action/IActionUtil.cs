using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetModels;

namespace IActionUtil
{
    public class IActionType{
        public const string MOVE="move";
        public const string DANCE="dance";
        public const string SKILL="skill";
        public const string STOP="stop";
    }
    public class IActionParser{
        public static IAction ParseOpinion(string objName,Opinion opinion){
            switch(opinion.type){
                case IActionType.MOVE:{
                    return ParseForMove(objName,opinion);
                }
                case IActionType.DANCE:{
                    return ParseForDance(objName,opinion);
                }
                case IActionType.STOP:{
                    return ParseForStop(objName,opinion);
                }
            }
            return null;
        }


        protected static IAction ParseForMove(string objName,Opinion opinion){
            string[] pos_str=opinion.desc.Split('*');
            Vector3 position=new Vector3();
            position.x=float.Parse(pos_str[0]);
            position.z=float.Parse(pos_str[1]);
            position.y=0;
            Move move=new Move(objName,opinion.framenum,position);
            return move;
        }

        protected static IAction ParseForDance(string objName,Opinion opinion){
            int danceNum=int.Parse(opinion.desc);
            Dance dance=new Dance(objName,opinion.framenum,danceNum);
            return dance;
        }

        protected static IAction ParseForStop(string objName,Opinion opinion){
            Stop stop=new Stop(objName,opinion.framenum);
            return stop;
        }
    }
}
