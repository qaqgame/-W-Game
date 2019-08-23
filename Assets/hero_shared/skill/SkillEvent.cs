using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SkillEvent : ScriptableObject
{
    public ObjectPicker picker;
    public List<Condition> conditions=null;//该事件的条件
    public List<string> objects;//目标对象的name
    public float startTime;//开始时间
    public List<Action> startActions;//开始事件
    public Skill skill;//所属技能

    protected float eventStartTime;
    protected SkillEvent instance;

    public SkillEvent(ref Skill _skill){
        instance=this;
        skill=_skill;
        startActions=new List<Action>();
    }

    public void setPicker(ref ObjectPicker _picker){
        picker=_picker;
    }

    public void execute(){
        if(picker!=null){
            objects=picker.execute();
        }
        eventStartTime=LockStepController.currentime;
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
