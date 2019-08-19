using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetModels;
public class Stop : IAction{
    public Stop(string _objName,int _frameNum):base(_objName,_frameNum){

    }

    public override void Execute(){
        GameObject.Find(objectName).GetComponent<BasicRoleControll>().Stop();
    }

    public override string getType(){
        return "stop";
    }

    public override Opinion toOpinion(){
        opinion.desc="none";
        opinion.target="none";
        opinion.position="0*0";
        opinion.framenum=frameNum;
        return opinion;
    }
}
