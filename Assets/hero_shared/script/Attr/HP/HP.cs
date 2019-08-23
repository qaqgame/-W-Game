using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HP : System.Object
{
    public int maxValue;//最大值
    public int curValue;//当前值
    public int minValue;//最小值

    public int cur{
        get{return curValue;}
        set{curValue=value;}
    }

    public int max{
        get{return maxValue;}
        set{maxValue=value;}
    }

    public int min{
        get{return minValue;}
        set{minValue=value;}
    }


}
