using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//作为判断条件的数据结构的基类
public abstract class BaseDataType : System.Object
{
    public virtual bool Equal(BaseDataType d1){
        return this==d1;
    }

    public virtual bool NotEqual(BaseDataType d1){
        return this!=d1;
    }

    public abstract bool Upper(BaseDataType d1);
    public abstract bool NotUpper(BaseDataType d1);
    public abstract bool Lower(BaseDataType d1);
    public abstract bool NotLower(BaseDataType d1);
}
