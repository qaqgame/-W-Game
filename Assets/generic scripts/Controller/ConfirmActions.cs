using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmActions
{
    Dictionary<int, bool> confirmFlag;//判断是否已接收数据

    private static object locker = new object(); 
    
    public ConfirmActions(){
        confirmFlag=new Dictionary<int, bool>();
    }

    //确认之后第offsetturn回合的接收
    public void Confirm(int offsetTurn){
        
        confirmFlag[offsetTurn]=true;
        
        
    }

    public void NextTurn(){
        confirmFlag.Remove(LockStepController.Instance.LockStepTurnID-1);
    }

    public void setCurTurn(int turn){
        int[] keys=new int[confirmFlag.Keys.Count];
        confirmFlag.Keys.CopyTo(keys,0);
        foreach (var k in keys)
        {
            if(k<=LockStepController.Instance.LockStepTurnID){
                confirmFlag.Remove(k); 
            }
        }
    }
    
    public bool ReadyForNextTurn(){
        // if(LockStepController.Instance.LockStepTurnID==LockStepController.FirstLockStepTurnID){
        //     return true;
        // }


        if(confirmFlag.ContainsKey(LockStepController.Instance.LockStepTurnID + 1) ){
            return true;
        }

        return false;
    }
}
