using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleBoundStarter : BaseStarter
{
    
    public float radius;

    public CircleBoundStarter(ref ObjectPicker _picker,float _radius,ref Skill _skill):base(ref _picker,ref _skill){
        radius=_radius;
    }
    public CircleBoundStarter(ref ObjectPicker _picker,float _radius,Vector3 _center,ref Skill _skill):base(ref _picker,_center,ref _skill){
        radius=_radius;
    }

    protected override void onStart(){
        Debug.Log("circlebound create,radius:"+radius);
        base.onStart();
        //创建边框
        obj=GameObjectInstance();
        obj.transform.localPosition=center;
        obj.transform.localScale=new Vector3(2*radius,obj.transform.localScale.y,2*radius);
    }    

    protected override void onMouseWorldMoved(Vector3 currentPos, Vector3 direction){
        Vector3 targetPos=currentPos;
        Vector3 worldCenter=PositionUtil.RelativeToWorldPosition(center,ParentObject().name);
        //Debug.Log("target pos:"+targetPos+" world center:"+worldCenter);
        //如果鼠标距离中心大于半径
        if((targetPos-worldCenter).sqrMagnitude>radius*radius){
            targetPos=worldCenter+((targetPos-worldCenter).normalized)*radius;
            Debug.Log("distance:"+(targetPos-worldCenter).sqrMagnitude+"radius*2:"+radius*radius+"changed target position:"+targetPos);
        }
        base.onMouseWorldMoved(targetPos,direction);
    }

    protected override string PrefabPath(){
        return "CircleBound";
    }

}


