using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager 
{
    protected static Dictionary<int,Skill> skillMap;

    public static SkillManager Instance=null;

    private SkillManager(){
        skillMap=new Dictionary<int, Skill>();
        
    }

    public static SkillManager getInstance(){
        if(Instance==null){
            Instance=new SkillManager();
        }
        return Instance;
    }

    public void addSkill(Skill skill){
        if(!skillMap.ContainsKey(skill.id)){
            skillMap.Add(skill.id,skill);
        }
    }

    public Skill getSkill(int id){
        switch(id){
            case 1: return new TestSkill();
            default:return null;
        }
        // Debug.Log("current skill:"+skillMap[id].id);
        // if(skillMap.ContainsKey(id)){
        //     return skillMap[id].getNewInstance();
        // }
        // return null;
    }
}
