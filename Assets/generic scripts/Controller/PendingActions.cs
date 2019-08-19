using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendingActions
{
	Dictionary<int,List<IAction>> idleActions;
	

	// LockStepController lsm;
	
	public PendingActions () {
		// this.lsm = lsm;
		idleActions=new Dictionary<int,List<IAction>>();
	}
	
	public void NextTurn() {
		idleActions[LockStepController.Instance.LockStepTurnID-1].Clear();
		idleActions.Remove(LockStepController.Instance.LockStepTurnID -1);
	}
	
	public void Porcess(int frame){
		int turn=LockStepController.Instance.LockStepTurnID;
		if(idleActions.ContainsKey(turn)){
			for(int i = idleActions[turn].Count-1; i>=0; i--){
				if(frame>=idleActions[turn][i].frameNum){
					idleActions[turn][i].Execute();
					idleActions[turn].RemoveAt(i);
				}
			}
		}
	}

	public void AddAction(IAction action,int currentLockStepTurn, int actionsLockStepTurn) {
		//只存储之后回合的动作
		if(actionsLockStepTurn>LockStepController.Instance.LockStepTurnID){
			//如果已包含目标回合
			if(idleActions.ContainsKey(actionsLockStepTurn)){
				idleActions[actionsLockStepTurn].Add(action);
			}
			else{
				List<IAction> newActionList=new List<IAction>();
				idleActions.Add(actionsLockStepTurn,newActionList);
			}
		}
	}
	
	public bool ReadyForNextTurn() {	
		//if this is the 1st turn, no actions had the chance to be recieved yet
		if(LockStepController.Instance.LockStepTurnID == LockStepController.FirstLockStepTurnID) {
			return true;
		}
		//如果当前回合没有剩余动作
		if(idleActions[LockStepController.Instance.LockStepTurnID].Count==0){
			return true;
		}
		//if none of the conditions have been met, return false
		return false;
	}
}
