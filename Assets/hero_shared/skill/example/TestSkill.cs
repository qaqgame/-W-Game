using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSkill : Skill
{
    Skill skill;
    public TestSkill():base(){
        skill=this;
        id=1;
        content="0s后释放火球(1s后消失)并在0.5s后造成200伤害";

        ObjectPicker picker=new CirclePicker(3,true,false);
        starter=new CircleBoundStarter(ref picker,10,ref skill);

        SkillEvent place=new Place(0f,1f,PreFab(),ObjectNameManager.getName("ball"),ref skill);       
        SkillEvent damage=new DelayDamage(1.5f,200,ref skill);
        place.setPicker(ref picker);
        damage.setPicker(ref picker);

        events.Add(place);
        events.Add(damage);
        
    }

    protected virtual string PreFab(){
        return "Skill/Skill_1";
    }
}
