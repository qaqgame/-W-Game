using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Place : TimerEvent
{
    string objPath,objName;
    public Place(float _startTime,float _existTime,string _objPath,string _name,ref Skill _skill):base(ref _skill){
        startTime=_startTime;
        timeLength=_existTime;
        objName=_name;
        objPath=_objPath;
        startActions.Add(new PlaceObject(objPath,objName,ref instance));
        endActions.Add(new DestroyObject(objName,ref instance));
    }

    
}
