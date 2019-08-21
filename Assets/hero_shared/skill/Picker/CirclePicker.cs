using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclePicker : ObjectPicker
{
    public float radius;//半径
    public CirclePicker(float _radius,bool _visible,bool _storeEnergy):base(_visible,_storeEnergy){
        radius=_radius;
    }

    protected override List<string> FilterObject(){
        List<string> objects=new List<string>();
        Vector3 worldCenter=PositionUtil.RelativeToWorldPosition(center,ParentObject().name);
        foreach (var gameObject in GameObject.FindGameObjectsWithTag(MainObjectTypes.MAIN_OBJECT))
        {
            Debug.Log("object pos:"+gameObject.transform.position+" center:"+worldCenter);
            if((gameObject.transform.position-worldCenter).sqrMagnitude<radius*radius){
                objects.Add(gameObject.name);
                Debug.Log("objects picked:"+gameObject.name);
            }
            
        }
        return objects;
    }

    public override void create(){
        base.create();
        obj.transform.localScale=new Vector3(2*radius,1,2*radius);
    }

    protected override string PrefabPath(){
        return "CirclePicker";
    }

}
