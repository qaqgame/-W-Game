using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetModels;
public class ExecuteSkill : IAction
{
    int skillID;
    Vector3 position;
    public ExecuteSkill(string _objName,int _frameNum,int _skillID,Vector3 _position):base(_objName,_frameNum){
        skillID=_skillID;
        position=_position;
    }

    public override void Execute(){
        Skill skill=SkillManager.getInstance().getSkill(skillID);
        skill.skillController=GameObject.Find(objectName).GetComponent<SkillController>();
        skill.starter.picker.setPosition(position);
        skill.skillController.executeSkill(skill);
    }

    public override string getType(){
        return "skill";
    }

    public override Opinion toOpinion(){
        opinion.desc=skillID.ToString();
        opinion.target=position.ToString();
        opinion.position="0*0";
        opinion.framenum=frameNum;
        return opinion;
    }
}
