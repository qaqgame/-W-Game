using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SkillEvent : ScriptableObject
{
    public ObjectPicker picker;
    public List<Condition> conditions;//该事件的条件
    public List<int> objects;//目标对象的id
    public float startTime;//开始时间
    public List<Action> startActions;//开始事件
    protected Skill skill;//所属技能

    public SkillEvent(ref Skill _skill){
        skill=_skill;
    }

    public Skill OweSkill{
        set{skill=value;}
        get{return skill;}
    }
    public void execute(){
        if(picker!=null){
            picker.setSkillEvent(this);
            picker.execute();
        }
        startTime=Time.time;
        if(startActions!=null){
            foreach (var action in startActions)
            {
                action.execute();
            }
        }
        onStart();
    }
    
    // Start is called before the first frame update
    public virtual void onStart()
    {
        
        
    }

    public virtual void onFixedUpdate(){
        
    }

    public virtual void onEnd(){
    }

    public virtual void reset(){

    }
}
