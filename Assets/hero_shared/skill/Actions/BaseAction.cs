using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action:ScriptableObject{
    public bool continuous=false;//是否持续
    protected Skill skill;

    public Skill OweSkill{
        set{skill=value;}
        get{return skill;}
    }

    public virtual void execute(){
        if(continuous)
        {
            skill.continuousActions.Add(this);
        }
    }

    public virtual void update(){

    }
    //结束该action，在update里手动调用
    public virtual void end(){
        if(continuous){
            skill.continuousActions.Remove(this);
        }
    }

    public virtual void reset(){

    }
}