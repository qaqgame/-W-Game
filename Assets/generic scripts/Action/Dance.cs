using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetModels;
public class Dance : IAction
{
    int danceNum;
    public Dance(string _objName,int _frameNum,int _danceNum):base(_objName,_frameNum){
        danceNum=_danceNum;
    }

    public override void Execute(){
        GameObject.Find(objectName).GetComponent<BasicRoleControll>().Dance(danceNum);
    }

    public override string getType(){
        return "dance";
    }
    public override Opinion toOpinion(){
        opinion.desc=danceNum.ToString();
        opinion.target="none";
        opinion.position="0*0";
        opinion.framenum=frameNum;
        return opinion;
    }
}
