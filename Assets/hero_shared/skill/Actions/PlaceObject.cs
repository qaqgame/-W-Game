using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceObject : Action
{
    string objPath,objName;
    GameObject obj;
    public PlaceObject(string _objPath,string _name,ref SkillEvent _skillEvent):base(ref _skillEvent){
        objName=_name;
        objPath=_objPath;
    }

    protected override void onStart(){
        obj=Instantiate(Resources.Load(objPath)) as GameObject;
        obj.name=objName;
        if(skillEvent.picker!=null){
            obj.transform.position=skillEvent.picker.center;
        }
        else{
            Debug.LogError("Action PlaceObject error:cannot get the picker");
            obj.transform.position=new Vector3(0,0,0);
        }
    }
}
