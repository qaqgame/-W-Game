using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetModels;
public class Move : IAction
{
    
    public Vector3 position;

    public Move(string _objName,int _frameNum,Vector3 _position):base(_objName,_frameNum){
        position=_position;
    }
    
    public override void Execute(){
        GameObject.Find(objectName).GetComponent<BasicRoleControll>().Move(position);
    }

    public override string getType(){
        return "move";
    }

    public override Opinion toOpinion(){
        opinion.desc=position.x.ToString()+"*"+position.z.ToString();
        opinion.target="none";
        opinion.position="0*0";
        opinion.framenum=frameNum;
        return opinion;
    }
}
