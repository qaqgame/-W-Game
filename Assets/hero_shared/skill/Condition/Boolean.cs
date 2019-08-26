using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boolean :BaseDataType
{
    bool value;
    public Boolean(bool _value){
        value=_value;
    }

    public override bool Upper(BaseDataType d1){
        Debug.LogError("bool cannot check the upper:"+new UnityException().ToString());
        return false;
    }

    public override bool NotUpper(BaseDataType d1){
        return Upper(d1);
    }

    public override bool Lower(BaseDataType d1){
        return Upper(d1);
    }

    public override bool NotLower(BaseDataType d1){
        return Upper(d1);
    }


}
