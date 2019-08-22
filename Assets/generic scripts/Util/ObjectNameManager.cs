using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectNameManager 
{
    static int count=0;
    public static string getName(string baseName){
        return baseName+(count++).ToString();
    }
}
