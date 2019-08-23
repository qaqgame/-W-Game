using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimerEvent : SkillEvent{
    public float intervalTime;//间隔时间
    public float timeLength;//持续时间
    public List<Action> timerActions;//定时事件
    public List<Action> endActions;//结束事件
    public List<Action> interruptActions;//中断事件

    public int times;//触发次数

    private int curTimes;//当前次数

    public TimerEvent(ref Skill _skill):base(ref _skill){
        timerActions=new List<Action>();
        endActions=new List<Action>();
    }
    
    public override void onStart(){
        base.onStart();
        skill.runningEvent.Add(this);
    }

    public override void onFixedUpdate(){
        //间隔时间>=0
        if(intervalTime>=0&&curTimes<times&&LockStepController.currentime>eventStartTime+(curTimes+1)*intervalTime){
            onTimerUpdate();
        }
        if(LockStepController.currentime>=eventStartTime+timeLength)
            onEnd();
    }

    protected virtual void onTimerUpdate(){
        ++curTimes;
        foreach (var action in timerActions)
        {
            action.execute();
        }
    }

    public virtual void onInterrupt(){
        foreach (var action in interruptActions)
        {
            action.execute();
        }
    }

    public override void onEnd(){
        foreach (var action in endActions)
        {
            action.execute();
        }
    }

}
