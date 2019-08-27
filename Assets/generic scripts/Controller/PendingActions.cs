using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendingActions
{
	Dictionary<int,List<IAction>> idleActions;

	public int currentTurn;
	

	// LockStepController lsm;
	
	public PendingActions () {
		// this.lsm = lsm;
		idleActions=new Dictionary<int,List<IAction>>();
		currentTurn=LockStepController.Instance.LockStepTurnID;
	}
	
	public void NextTurn() {
		
		if(idleActions.ContainsKey(currentTurn)){
			idleActions[currentTurn].Clear();
			idleActions.Remove(currentTurn);
		}
		currentTurn++;
	}

	public void setCurTurn(int turn){
		currentTurn=turn;
		int [] keys=new int[idleActions.Keys.Count];
		idleActions.Keys.CopyTo(keys,0);
		foreach (var k in keys)
		{
			if(k<=currentTurn){
				idleActions[k].Clear();
				idleActions.Remove(k);
			}
		}
	}
	
	public void Porcess(int frame){
		//int turn=LockStepController.Instance.LockStepTurnID;
		if(idleActions.ContainsKey(currentTurn)){
			for(int i = idleActions[currentTurn].Count-1; i>=0; i--){
				if(frame>=idleActions[currentTurn][i].frameNum){
					idleActions[currentTurn][i].Execute();
					idleActions[currentTurn].RemoveAt(i);
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
			//否则添加新的list
			else{
				List<IAction> newActionList=new List<IAction>();
				newActionList.Add(action);
				idleActions.Add(actionsLockStepTurn,newActionList);
			}
		}
	}
	
	public bool ReadyForNextTurn() {	
		//if this is the 1st turn, no actions had the chance to be recieved yet
		if(LockStepController.Instance.LockStepTurnID == LockStepController.FirstLockStepTurnID) {
			return true;
		}
		//如果当前回合没动作或者没有剩余动作
		if((!idleActions.ContainsKey(currentTurn))||idleActions[currentTurn].Count==0){
			return true;
		}
		//if none of the conditions have been met, return false
		return false;
	}
}
