﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetModels;
using IActionUtil;
public class LockStepController : MonoBehaviour
{
    //开始时的同步id
    public static readonly int FirstLockStepTurnID = 0;
	//本类实例
	public static LockStepController Instance;
	
	public int LockStepTurnID = FirstLockStepTurnID;
	
    //用于执行的action
	private PendingActions pendingActions;
    //用于确认的action
	private ConfirmActions confirmActions;

    //网络层
    Client client;

    bool initialized = false;

    // Start is called before the first frame update
    void Awake()
    {
        enabled=false;
        Instance=this;
        Init();
    }

    #region GameStart
    public void InitGame(){
        if(initialized){return;}
        initialized=true;
    }

    public void Init(){
        LockStepTurnID = FirstLockStepTurnID;
        pendingActions = new PendingActions(this);
		confirmActions = new ConfirmActions();

        InitGame();
    }

    public void StartGame(){
        enabled=true;
    }
    #endregion

    #region Turn
    //向网络层添加action
    public void SendAction(IAction action){
        if(!initialized) {
			Debug.Log("Game has not started, action will be ignored.");
			return;
		}
        //client.addAction(action);
		Opinion opinion=action.toOpinion();
		Opinion[] os=new Opinion[]{opinion};
		RecieveActions(LockStepTurnID+1,"player1",os);
    }

    private bool LockStepTurn() {
		//Debug.Log("LockStepTurnID: " + LockStepTurnID);
		if(LockStepTurnID >= FirstLockStepTurnID + 3&&LockStepTurnID>pendingActions.currentTurn) {
			ProcessActions (gameFramesPerLocksetpTurn);
		}
		return NextTurn();
	}

    private void ProcessActions(int frame) {
		pendingActions.Porcess(frame);
	}

    //接收游戏数据
    public void RecieveActions(int turn,string objName,Opinion[] opinions){
		foreach (var opinion in opinions)
		{
			pendingActions.AddAction(IActionParser.ParseOpinion(objName,opinion),LockStepTurnID,turn);
		}
    }
    //确认回合
    public void ConfirmTurn(int turn){
		if(turn<=LockStepTurnID||turn>LockStepTurnID+3){
			Debug.LogError("can't confirm turn:"+turn);
			return;
		}
		confirmActions.Confirm(turn-LockStepTurnID-1);
    }

    private bool NextTurn() {
		if(confirmActions.ReadyForNextTurn() && pendingActions.ReadyForNextTurn()) {
			//增加回合数
			LockStepTurnID++;
			//将确认用action移至下一回合
			confirmActions.NextTurn();
			//将待执行action移至下一回合
			pendingActions.NextTurn();
			
			return true;
		}
		
		return false;
	}

    private float accumilatedTime = 0f;
	
	public float frameLength = 0.05f; //5 miliseconds

	public int gameFramesPerLocksetpTurn=4;

	private int gameFrame=0;
	
	//called once per unity frame
	public void Update() {
		//Basically same logic as FixedUpdate, but we can scale it by adjusting FrameLength
		accumilatedTime = accumilatedTime + Time.deltaTime;
		
		//in case the FPS is too slow, we may need to update the game multiple times a frame
		while(accumilatedTime > frameLength) {
			GameFrameTurn();
			accumilatedTime = accumilatedTime - frameLength;
		}
	}

	private void GameFrameTurn() {
		//first frame is used to process actions
		if(gameFrame == 0) {
			if(LockStepTurn()) {
				gameFrame++;
			}
		} else {
			ProcessActions(gameFrame);
			gameFrame++;
			if(gameFrame == gameFramesPerLocksetpTurn) {
				gameFrame = 0;
			}
		}
	}
    #endregion
}