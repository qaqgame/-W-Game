using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleBoundStarter : BaseStarter
{
    
    public float radius;

    public CircleBoundStarter(ref ObjectPicker _picker,float _radius):base(ref _picker){
        radius=_radius;
    }
    public CircleBoundStarter(ref ObjectPicker _picker,float _radius,Vector3 _center):base(ref _picker,_center){
        radius=_radius;
    }

    protected override void onStart(){
        base.onStart();
        //创建边框
        obj=GameObjectInstance();
        obj.transform.position.Set(center.x,center.y,center.z);
        obj.transform.localScale.Set(2*radius,obj.transform.localScale.y,2*radius);
    }    

}


