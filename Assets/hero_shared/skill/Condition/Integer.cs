using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Integer : BaseDataType
{
    int value;
    public Integer(int _value){
        value=_value;
    }

    public override bool Upper(BaseDataType d1){
        if(d1==null||d1.GetType()!=typeof(Integer)){
            Debug.LogError("cannot check the upper int,data type difference:"+new UnityException().ToString());
            return false;
        }
        return value>((Integer)d1).value;
    }

    public override bool NotUpper(BaseDataType d1){
        return !Upper(d1);
    }

    public override bool Lower(BaseDataType d1){
        if(d1==null||d1.GetType()!=typeof(Integer)){
            Debug.LogError("cannot check the upper int,data type difference:"+new UnityException().ToString());
            return false;
        }
        return value<((Integer)d1).value;
    }

    public override bool NotLower(BaseDataType d1){
        return !Lower(d1);
    }
}
