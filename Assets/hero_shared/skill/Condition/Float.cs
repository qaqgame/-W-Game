using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Float : BaseDataType
{
    float value;
    public Float(float _value){
        value=_value;
    }

    public override bool Upper(BaseDataType d1){
        if(d1==null||d1.GetType()!=typeof(Float)){
            Debug.LogError("cannot check the upper float,data type difference:"+new UnityException().ToString());
            return false;
        }
        return value>((Float)d1).value;
    }

    public override bool NotUpper(BaseDataType d1){
        return !Upper(d1);
    }

    public override bool Lower(BaseDataType d1){
        if(d1==null||d1.GetType()!=typeof(Integer)){
            Debug.LogError("cannot check the upper float,data type difference:"+new UnityException().ToString());
            return false;
        }
        return value<((Float)d1).value;
    }

    public override bool NotLower(BaseDataType d1){
        return !Lower(d1);
    }
}
