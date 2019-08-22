using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action:ScriptableObject{
    public bool continuous=false;//是否持续
    protected Skill skill;
    protected SkillEvent skillEvent;
    

    public Action(ref SkillEvent _skillEvent){
        skill=_skillEvent.skill;
        skillEvent=_skillEvent;
    }

    public void execute(){
        if(continuous)
        {
            skill.continuousActions.Add(this);
        }
        onStart();
    }

    protected virtual void onStart(){

    }

    protected virtual void onEnd(){

    }

    public virtual void update(){

    }
    //结束该action，在update里手动调用
    public virtual void end(){
        if(continuous){
            skill.continuousActions.Remove(this);
        }
        onEnd();
    }

    public virtual void reset(){

    }
}