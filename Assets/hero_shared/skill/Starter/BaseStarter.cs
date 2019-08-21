using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//该类代表技能指示器

/*
todoList:
1.将鼠标坐标改为游戏中的世界坐标
 */
public class BaseStarter : ScriptableObject
{
    Skill skill=null;
    bool running=false;
    public Vector3 center;

    protected ObjectPicker picker;//选择器

    protected GameObject obj;//技能指示器实体
    protected GameObject rootObject;//释放技能的主体
    protected SkillController skillController;//技能控制器

    private static Vector2 lastMousePosition;//上次的鼠标位置
    private static Vector3 lastMouseWorldPosition;//上次的鼠标对应的世界坐标
    public BaseStarter(ref ObjectPicker _picker){
        picker=_picker;
        center=new Vector3(0,0.1f,0);
        rootObject=ParentObject();
        skillController=rootObject.GetComponent<SkillController>();
    }
    public BaseStarter(ref ObjectPicker _picker,Vector3 _center){
        picker=_picker;
        center=_center;
        rootObject=ParentObject();
        skillController=rootObject.GetComponent<SkillController>();
    }

    public void execute(){
        if(!running){
            onStart();
            skillController.executeStarter(this);
        }
    }

    //每帧调用
    public void update(){
        //判断点击事件
        //左键按下
        if(Input.GetMouseButtonDown(0)){
            onMouseLeftClicked();
        }
        else if(Input.GetMouseButtonDown(1)){
            onMouseRightClicked();
        }
        Vector2 curPos=new Vector2(Input.mousePosition.x,Input.mousePosition.y);

        if(curPos!=lastMousePosition){
            onMouseMoved(curPos.x,curPos.y,(curPos-lastMousePosition).normalized);
            lastMousePosition=curPos;

            Vector3 curWorldPos=PositionUtil.ScreenToWorldPosition(curPos,MainObjectTypes.MAIN_PLANE,MainLayerTypes.MAIN_PLANE);
            onMouseWorldMoved(curWorldPos,(curWorldPos-lastMouseWorldPosition).normalized);
            lastMouseWorldPosition=curWorldPos;
        }

        onUpdate();
    }

    //定时调用
    public void fixedUpdate(){
        //当左键处于按下状态时
        if(Input.GetMouseButton(0)){
            onMouseHold(Time.fixedDeltaTime);
        }

        onFixedUpdate(Time.fixedDeltaTime);
    }

    protected virtual void onStart(){
        running=true;
        //如果选择器能够显现
        if(picker.visible){
            picker.create();
        }
    }
    
    protected virtual void onUpdate(){
        
    }

    protected virtual void onFixedUpdate(float time){

    }


    //结束目标选定
    protected virtual void onSuccess(){
        running=false;
        skillController.endStarter(this);
        skillController.executeSkill(skill);
        picker.execute();
        destroyObjects();
    }

    //中断目标选定
    protected virtual void onInterrupt(){
        running=false;
        skillController.endStarter(this);
        destroyObjects();
    }

    protected virtual void onMouseLeftClicked(){
        onSuccess();
    }

    protected virtual void onMouseRightClicked(){
        onInterrupt();
    }

    //鼠标左键持续按压时
    //param: time - 间隔时间
    protected virtual void onMouseHold(float time){
        if(picker.storeEnergy)
            picker.store(time);
    }

    protected virtual void onMouseMoved(float x,float y,Vector2 direction){
        
    }

    protected virtual void onMouseWorldMoved(Vector3 currentPos,Vector3 direction){
        if(picker.visible){
            picker.setPosition(currentPos);
        }
    }

    //销毁物体
    protected virtual void destroyObjects(){
        Debug.Log("destroy called");
        if(picker.visible){
            picker.destroy();
        }
        Destroy(obj);
    }


    protected GameObject GameObjectInstance(){
        GameObject _obj= Instantiate(Resources.Load("SkillBound/"+PrefabPath())) as GameObject;
        _obj.transform.SetParent(rootObject.transform);
        return _obj;
    }

    //该边界的实例
    protected virtual string PrefabPath(){
        return null;
    }

    //父物体，即释放的主体
    protected virtual GameObject ParentObject(){
        return GameObject.Find(Client.userID);
    }
}
