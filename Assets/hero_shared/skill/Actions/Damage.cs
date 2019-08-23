using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : Action
{
    List<string> objs;
    int damage;
    public Damage(int _damage,ref SkillEvent _skillEvent):base(ref _skillEvent){
        continuous=false;
        skillEvent=_skillEvent;
        damage=_damage;
    }

    protected override void onStart(){
        objs=skillEvent.objects;
        foreach (var name in objs)
        {
            GameObject.Find(name).GetComponent<BasicAttrController>().hP.cur-=50;
            Debug.Log("damage "+name+" "+damage);
        }
    }
}
