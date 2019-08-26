using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DString : BaseDataType
{
    string value;
    public DString(string _value){
        value=_value;
    }

    public override bool Upper(BaseDataType d1){
        if(d1==null||d1.GetType()!=typeof(DString)){
            Debug.LogError("cannot check the upper string,data type difference:"+new UnityException().ToString());
            return false;
        }
        return value[0]>((DString)d1).value[0];
    }

    public override bool NotUpper(BaseDataType d1){
        return !Upper(d1);
    }

    public override bool Lower(BaseDataType d1){
        if(d1==null||d1.GetType()!=typeof(DString)){
            Debug.LogError("cannot check the upper string,data type difference:"+new UnityException().ToString());
            return false;
        }
        return value[0]<((DString)d1).value[0];
    }

    public override bool NotLower(BaseDataType d1){
        return !Lower(d1);
    }
}
