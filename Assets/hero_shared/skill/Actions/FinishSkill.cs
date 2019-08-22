using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishSkill : Action
{
    public FinishSkill(ref SkillEvent _skillEvent):base(ref _skillEvent){
        continuous=false;
    }

    protected override void onStart(){
        skill.skillController.endSkill(skill);
    }
}
