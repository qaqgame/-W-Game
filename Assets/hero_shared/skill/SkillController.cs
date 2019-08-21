using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    BaseStarter skillStarter;
    List<Skill> runningSkill;//在运行的技能
    // Start is called before the first frame update
    void Start()
    {
        runningSkill=new List<Skill>(6);
        skillStarter=null;
    }

    // Update is called once per frame
    void Update()
    {
        skillStarter.update();
        foreach (var skill in runningSkill)
        {
            skill.onUpdate();
        }
    }

    void FixedUpdate() {
        skillStarter.fixedUpdate();
        foreach (var skill in runningSkill)
        {
            skill.onFixedUpdate();
        }
    }

    public void executeSkill(Skill skill){
        skill.execute();
        //按照优先级将其加入技能list
        int i=0;
        while(runningSkill[i].priority<skill.priority){
            if(++i>=runningSkill.Count){
                break;
            }
        }
        runningSkill.Insert(i,skill);
    }
}
