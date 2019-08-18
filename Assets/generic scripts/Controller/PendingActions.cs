using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendingActions
{
    public List<IAction> CurrentActions;
	private List<IAction> NextActions;
	private List<IAction> NextNextActions;
	//incase other players advance to the next step and send their action before we advance a step
	private List<IAction> NextNextNextActions;
	
	public int currentActionsCount;
	private int nextActionsCount;
	private int nextNextActionsCount;
	private int nextNextNextActionsCount;
	

	public int currentTurn;//当前回合
	// LockStepController lsm;
	
	public PendingActions () {
		// this.lsm = lsm;
		
		CurrentActions = new List<IAction>();
		NextActions = new List<IAction>();
		NextNextActions = new List<IAction>();
		NextNextNextActions = new List<IAction>();
		
		currentActionsCount = 0;
		nextActionsCount = 0;
		nextNextActionsCount = 0;
		nextNextNextActionsCount = 0;
		currentTurn=LockStepController.Instance.LockStepTurnID;
	}
	
	public void NextTurn() {
		//Finished processing this turns actions - clear it
		// for(int i=0; i<CurrentActions.Count; i++) {
		// 	CurrentActions[i] = null;
		// }
		CurrentActions.Clear();
		List<IAction> swap = CurrentActions;
		
		//last turn's actions is now this turn's actions
		CurrentActions = NextActions;
		currentActionsCount = nextActionsCount;
		
		//last turn's next next actions is now this turn's next actions
		NextActions = NextNextActions;
		nextActionsCount = nextNextActionsCount;
		
		NextNextActions = NextNextNextActions;
		nextNextActionsCount = nextNextNextActionsCount;
		
		//set NextNextNextActions to the empty list
		NextNextNextActions = swap;
		nextNextNextActionsCount = 0;
		currentTurn++;
	}
	
	public void Porcess(int frame){
		for(int i=0;i<CurrentActions.Count;++i){
			if(frame>=CurrentActions[i].frameNum){
				CurrentActions[i].Execute();
				CurrentActions.RemoveAt(i);
				currentActionsCount--;
			}
		}
	}

	public void AddAction(IAction action,int currentLockStepTurn, int actionsLockStepTurn) {
		//add action for processing later
		if(actionsLockStepTurn == currentLockStepTurn + 3) {
			//if action is for next turn, add for processing 3 turns away
			NextNextNextActions.Add(action);
			nextNextNextActionsCount++;
		} else if(actionsLockStepTurn == currentLockStepTurn+2) {
			//if recieved action during our current turn
			//add for processing 2 turns away
			
			NextNextActions.Add(action);
			nextNextActionsCount++;
		} else if(actionsLockStepTurn == currentLockStepTurn +1) {
			//if recieved action for last turn
			//add for processing 1 turn away
			
			NextActions.Add(action);
			nextActionsCount++;
		} else {
			//TODO: Error Handling
			return;
		}
	}
	
	public bool ReadyForNextTurn() {	
		//if this is the 1st turn, no actions had the chance to be recieved yet
		if(LockStepController.Instance.LockStepTurnID == LockStepController.FirstLockStepTurnID) {
			return true;
		}
		if(currentActionsCount==0){
			return true;
		}
		//if none of the conditions have been met, return false
		return false;
	}
}
