using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPicker : ScriptableObject
{
    public bool visible=false;//该对象选择器是否可见
    public bool storeEnergy=false;//是否可蓄力

    protected SkillEvent skillEvent;

    protected Vector3 center;

    public ObjectPicker(bool _visible){
        visible=_visible;
    }

    public void setSkillEvent(SkillEvent _event){
        skillEvent=_event;
    }

    public void execute(){
        skillEvent.objects=FilterObject();
    }

    protected virtual List<int> FilterObject(){
        return null;
    }

    //创建该选择器（如果可见）
    public virtual void create(){

    }

    //销毁该选择器（如果可见）
    public virtual void destroy(){

    }

    //蓄力time
    public virtual void store(float time){

    }

    //设置坐标（在鼠标移动时调用）
    public virtual void setPosition(Vector3 _position){
        center=_position;
    }

    //设置缩放（在蓄力时调用）
    //param: _scale[0] - width; _scale[1] - height
    public virtual void scaleTo(Vector2 _scale){

    }



}
