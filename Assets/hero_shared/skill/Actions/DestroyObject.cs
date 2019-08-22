using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : Action
{
    protected string name;
    public DestroyObject(string _name,ref SkillEvent _skillEvent):base(ref _skillEvent){
        name=_name;
    }

    protected override void onStart(){
        Destroy(GameObject.Find(name));
    }
}
