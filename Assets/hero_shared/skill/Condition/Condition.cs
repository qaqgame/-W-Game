using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Condition {
    bool available();
}

public class ConditionUtil{
    public static bool CheckConditions(Condition[] conditions){
        foreach (var condition in conditions)
        {
            if(!condition.available()){
                return false;
            }
        }
        return true;
    }

    public static bool CheckConditions(List<Condition> conditions){
        foreach (var condition in conditions)
        {
            if(!condition.available()){
                return false;
            }
        }
        return true;
    }
}
