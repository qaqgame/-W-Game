using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SingleCondition{
    string paramName;//属性名
    int judgeType;//判断类型
    string obj;//判断对象

    public SingleCondition(string _param,int _type,string _obj){
        paramName=_param;
        judgeType=_type;
        obj=_obj;
    
    }
}

public class Conditions {
    SkillEvent skillEvent;//所属事件
    List<SingleCondition> conditions;

    public Conditions(ref SkillEvent _skillEvent){
        skillEvent=_skillEvent;
    }
    public bool available(){
        return true;
    }
}

public class ConditionUtil{
    public const int EQUAL=0;
    public const int NOT_EQUAL=1;
    public const int UPPER=2;
    public const int NOT_UPPER=3;
    public const int LOWER=4;
    public const int NOT_LOWER=5;
    public static bool CheckConditions(Conditions[] conditions){
        if(conditions!=null){
            foreach (var condition in conditions)
            {
                if(!condition.available()){
                    return false;
                }
            }
        }
        return true;
    }

    public static bool CheckConditions(List<Conditions> conditions){
        if(conditions!=null){
            foreach (var condition in conditions)
            {
                if(!condition.available()){
                    return false;
                }
            }
        }
        return true;
    }

    //判断两对象
    public static bool CheckObject(BaseDataType obj1,BaseDataType obj2,int type){
        switch(type){
            case EQUAL:return obj1.Equal(obj2);
            case NOT_EQUAL:return obj1.NotEqual(obj2);
            case UPPER:return obj1.Upper(obj2);
            case NOT_UPPER:return obj1.NotUpper(obj2);
            case LOWER:return obj1.Lower(obj2);
            case NOT_LOWER:return obj1.NotLower(obj2);
            default:{
                throw new UnityException("cannot check the conditon:"+type);
            }
        }
    }



}
