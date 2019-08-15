using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmActions
{
    bool[] confirmFlag;//判断是否已接收数据
    
    public ConfirmActions(){
        confirmFlag=new bool[3]{false,false,false};
    }

    //确认之后第offsetturn回合的接收
    public void Confirm(int offsetTurn){
        confirmFlag[offsetTurn]=true;
    }

    public void NextTurn(){
        confirmFlag[0]=confirmFlag[1];
        confirmFlag[1]=confirmFlag[2];
        confirmFlag[2]=false;
    }
    
    public bool ReadyForNextTurn(){
        if(LockStepController.Instance.LockStepTurnID==LockStepController.FirstLockStepTurnID){
            return true;
        }

        if(confirmFlag[0]){
            return true;
        }

        return false;
    }
}
