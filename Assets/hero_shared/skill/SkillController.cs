using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    BaseStarter skillStarter=null;
    List<Skill> runningSkill;//在运行的技能
    // Start is called before the first frame update
    void Start()
    {
        runningSkill=new List<Skill>();
        skillStarter=null;
    }

    // Update is called once per frame
    void Update()
    {
        if(skillStarter!=null){
            skillStarter.update();
        }
        for(int i=runningSkill.Count-1;i>=0;--i){
            runningSkill[i].onUpdate();
        }
    }

    void FixedUpdate() {
        if(skillStarter!=null){
            skillStarter.fixedUpdate();
        }
        for(int i=runningSkill.Count-1;i>=0;--i){
            runningSkill[i].onFixedUpdate();
        }
    }

    public void executeSkill(Skill skill){
        if(skill!=null){
            skill.skillController=this;
            skill.execute();
            //按照优先级将其加入技能list
            if(runningSkill.Count==0){
                runningSkill.Add(skill);
            }
            else{
                int i=runningSkill.Count;
                while(runningSkill[i-1].priority>=skill.priority){
                    if(--i==0){
                        break;
                    }
                }
                runningSkill.Insert(i,skill);
            }
        }
    }

    public void executeStarter(BaseStarter starter){
        if(skillStarter==null){
            skillStarter=starter;
        }
    }

    public void endSkill(Skill skill){
        Debug.Log("end skill called");
        if(skill!=null&&runningSkill.Contains(skill)){
            runningSkill.Remove(skill);
            skill.onEnd();
            skill.reset();
        }
    }

    public void endStarter(BaseStarter starter){
        if(skillStarter==starter){
            skillStarter=null;
        }
    }
}
