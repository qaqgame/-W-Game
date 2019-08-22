using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayDamage : SkillEvent
{
    int damage;
    public DelayDamage(float _startTime,int _damage,ref Skill _skill):base(ref _skill){
        startTime=_startTime;
        damage=_damage;
        startActions.Add(new Damage(damage,ref instance));
        startActions.Add(new FinishSkill(ref instance));
    }
}
