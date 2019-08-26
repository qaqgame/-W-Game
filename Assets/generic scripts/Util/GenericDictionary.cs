using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class GenericDictionary 
{

    private Dictionary<DataKey,DataValue> dataMap;

    public void add(string _name,BaseDataType _value){
        DataKey key=new DataKey(_name);
        DataValue value=new DataValue(_value);
    }

    public DataValue get(string _name){
        DataKey key=new DataKey(_name);
        if(dataMap.ContainsKey(key)){
            DataValue value=dataMap[key];
            return value;
        }
        return null;
    }

    

    
}

public class DataKey
{
   string name;
   public DataKey(string _name){
       name=_name;
   }

   public static bool operator ==(DataKey key_1,DataKey key_2){
       bool result=false;
       if(key_1.name==key_2.name){
           result=true;
       }
       return result;
   }
   public static bool operator !=(DataKey key_1,DataKey key_2){
       bool result=true;
       if(key_1.name==key_2.name){
           result=false;
       }
       return result;
   }

   // override object.Equals
   public override bool Equals(object obj)
   {
       //
       // See the full list of guidelines at
       //   http://go.microsoft.com/fwlink/?LinkID=85237
       // and also the guidance for operator== at
       //   http://go.microsoft.com/fwlink/?LinkId=85238
       //
       
       if (obj == null || GetType() != obj.GetType())
       {
           return false;
       }
       return this==(DataKey)obj;
   }
   
   // override object.GetHashCode
   public override int GetHashCode()
   {
       // TODO: write your implementation of GetHashCode() here
       throw new System.NotImplementedException();
       return base.GetHashCode();
   }
}

public class DataValue
{
    public BaseDataType value;
    public DataValue(BaseDataType _value){
        value=_value;
    }
}


