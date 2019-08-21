using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPicker : ScriptableObject
{
    public bool visible=false;//该对象选择器是否可见
    public bool storeEnergy=false;//是否可蓄力

    protected SkillEvent skillEvent=null;

    protected Vector3 center;

    protected GameObject obj;

    public ObjectPicker(bool _visible,bool _storeEnergy){
        visible=_visible;
        storeEnergy=_storeEnergy;
        center=new Vector3(0,0,0);
    }

    public void setSkillEvent(SkillEvent _event){
        skillEvent=_event;
    }

    public void execute(){
        List<string> list=FilterObject();
        if(skillEvent!=null){
            skillEvent.objects=FilterObject();
        }
    }

    //筛选对象，必须重写
    protected virtual List<string> FilterObject(){
        return null;
    }

    //创建该选择器（如果可见）
    public virtual void create(){
        obj=Instantiate(Resources.Load("Picker/"+PrefabPath())) as GameObject; 
        obj.transform.SetParent(ParentObject().transform);
        Debug.Log("picker center:"+center);
        obj.transform.localPosition=center;
    }

    //销毁该选择器（如果可见）
    public virtual void destroy(){
        center=new Vector3(0,0,0);
        Destroy(obj);
    }

    //蓄力time
    public virtual void store(float time){

    }

    //设置坐标（在鼠标移动时调用）
    public virtual void setPosition(Vector3 _position){
        obj.transform.localPosition=PositionUtil.WorldToRelativePosition(_position,ParentObject().name);
        center=PositionUtil.WorldToRelativePosition(_position,ParentObject().name);
    }

    //设置缩放（在蓄力时调用）
    //param: _scale[0] - width; _scale[1] - height
    public virtual void scaleTo(Vector2 _scale){

    }

    protected virtual string PrefabPath(){
        return null;
    }

    //父物体，即释放的主体
    protected virtual GameObject ParentObject(){
        return GameObject.Find(Client.userID);
    }

}
