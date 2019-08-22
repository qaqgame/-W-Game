using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : ScriptableObject
{
    public BaseStarter starter;
    public int id;//编号
    public string content;//描述
    public int priority;//优先级
    protected float startTime;//开始时间
    protected bool runnning=false;//是否在进行
    public List<TimerEvent> runningEvent;//正在运行的事件
    public List<SkillEvent> startEvent;//开始时的事件
    public List<SkillEvent> endEvent;//结束时的事件
    public List<SkillEvent> interruptEvent;//中断事件
    public List<SkillEvent> events;//中间的事件
    public List<Action> continuousActions;//持续作用的action

    protected List<SkillEvent> usedEvent;

    public SkillController skillController=null;

    public Skill(){
        runningEvent=new List<TimerEvent>();
        startEvent=new List<SkillEvent>();
        endEvent=new List<SkillEvent>();
        interruptEvent=new List<SkillEvent>();
        events=new List<SkillEvent>();
        continuousActions=new List<Action>();
        usedEvent=new List<SkillEvent>();
    }

    //外部调用，执行
    public void execute(){
        runnning=true;
        onStart();
        startTime=Time.time;
    }
    //开始时
    public virtual void onStart(){
        foreach (var e in startEvent)
        {
            e.execute();
        }
    }
    //更新时
    public virtual void onUpdate(){
        if(events.Count>0){
            int i=0;
            //更新events
            while(Time.time-startTime>=events[i].startTime){
                //如果i位置的event可以执行
                if(ConditionUtil.CheckConditions(events[i].conditions)){
                    //执行并从events中移除
                    events[i].execute();
                    usedEvent.Add(events[i]);
                    events.Remove(events[i]);
                }
                //否则检查下一个
                if(++i>=events.Count){
                    break;
                }
            }
        }
        //更新action
        foreach (var action in continuousActions)
        {
            action.update();
        }
    }
    //按时
    public virtual void onFixedUpdate(){
        foreach (var e in runningEvent)
        {
            e.onFixedUpdate();
        }
    }
    //正常结束时
    public virtual void onEnd(){
        foreach (var e in endEvent)
        {
            e.execute();
        }
    }
    //中断时
    public virtual void onInterrupt(){
        foreach (var e in runningEvent)
        {
            e.onInterrupt();
        }
        foreach (var e in interruptEvent)
        {
            e.execute();
        }
    }
    //重置
    public virtual void reset(){
        runnning=false;
        continuousActions.Clear();
        foreach (var e in usedEvent)
        {
            events.Add(e);
        }
        events.Sort((x, y) => x.startTime.CompareTo(y.startTime));
        usedEvent.Clear();
        skillController=null;
        resetAllEvent();
    }
    //重置所有事件
    protected virtual void resetAllEvent(){
        foreach (var e in startEvent)
        {
            e.reset();
        }
        foreach (var e in endEvent)
        {
            e.reset();
        }
        foreach (var e in events)
        {
            e.reset();
        }
        foreach (var e in interruptEvent)
        {
            e.reset();
        }
    }

    public virtual Skill getNewInstance(){
        return this.MemberwiseClone() as Skill;
    }
}
