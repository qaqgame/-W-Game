using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NetModels;
public class IAction
{
    public string objectName;//对象名
    public int frameNum;//帧数

    protected Opinion opinion;

    public IAction(string _objectName,int _frameNum){
        opinion=new Opinion();
        opinion.type=getType();
        frameNum=_frameNum;
        objectName=_objectName;
    }
    public virtual void Execute(){}

    public virtual string getType(){
        return "null";
    }

    public virtual Opinion toOpinion(){
        return null;
    }

}
