using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
public class DataParseUtil 
{
    public const int INT=0;
    public const int FLOAT=1;
    public const int STRING=2;
    public const int BOOL=3;
    public const int DATA=4;//内部存储类型
    public const int INNER=5;//内部自带类型
    public const int OUTER=6;//外部类型
    public static int ParseForJudgeType(string origin){
        switch(origin){
            case "==":return ConditionUtil.EQUAL;
            case "!=":return ConditionUtil.NOT_EQUAL;
            case ">":return ConditionUtil.UPPER;
            case ">=":return ConditionUtil.NOT_LOWER;
            case "<":return ConditionUtil.LOWER;
            case "<=":return ConditionUtil.NOT_UPPER;
            default:{
                throw new UnityException("parse for judge type error:"+origin);
            }
        }
    }

    //仅解析使用的数据的类型
    public static int ParseForDataType(string origin){
        if(origin=="true"||origin=="false"){
            return BOOL;
        }
        if(origin[0]=='\''){
            if(origin[0]==origin[origin.Length-1]){
                return STRING;
            }
            Debug.LogError("cannot parse data type:"+origin);
        }
        if(origin[0]=='$'){
            return DATA;
        }
        if(origin[0]=='%'){
            return INNER;
        }
        if(origin[0]=='@'){
            return OUTER;
        }
        if(Regex.IsMatch(origin, @"^[+-]?\d*$")){
            return INT;
        }
        if(Regex.IsMatch(origin,@"^(-?\\d+)(\\.\\d+)?$")){
            return FLOAT;
        }
        throw new UnityException("cannot parse data type:"+origin);
    }

    public static bool ParseForBool(string origin){
        if(origin=="true"){
            return true;
        }
        if(origin=="false"){
            return false;
        }
        throw new UnityException("parse data for bool error:"+origin);
    }

    public static int ParseForInt(string origin){
        return int.Parse(origin);
    }

    public static float ParseForFloat(string origin){
        return float.Parse(origin);
    }

    public static string ParseForString(string origin){
        return origin.Remove(origin.Length-1).Remove(0);
    }

    public Action ParseForAction(string origin){
        origin=origin.Trim();
        int startIndex=origin.IndexOf('(');
        int endIndex=origin.IndexOf(')');
        string name=origin.Substring(0,startIndex);
        string prms=origin.Substring(startIndex+1,endIndex-startIndex-1);
        string[] param=prms.Split(',');
        List<System.Object> parameters=new List<System.Object>();
        foreach (var item in param)
        {
            ParseParameters(item,ref parameters);
        }
        Type type=Type.GetType(name);
        return (Action)Activator.CreateInstance(type,parameters.ToArray());
    }

    

    protected static void ParseParameters(string param,ref List<System.Object> parameters){
        try{
            switch(ParseForDataType(param)){
                case INT:{
                    parameters.Add(ParseForInt(param));
                    break;
                }
                case FLOAT:{
                    parameters.Add(ParseForFloat(param));
                    break;
                }
                case STRING:{
                    parameters.Add(ParseForString(param));
                    break;
                }
                case BOOL:{
                    parameters.Add(ParseForBool(param));
                    break;
                }
            }
        }catch(UnityException e){
            Debug.LogError("parse parameters error:"+e.ToString());
        }
    }
}
