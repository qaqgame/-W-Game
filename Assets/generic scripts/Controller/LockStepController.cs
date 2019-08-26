using System.Collections;
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

	public bool updating=true;//在回合中
	public bool readingData=false;//在阅读当前回合的数据
	
    //用于执行的action
	private PendingActions pendingActions;
    //用于确认的action
	private ConfirmActions confirmActions;

	protected int stateTurn;
	protected Queue<Pos> curState;//上回合的全部状态

    //网络层
    Client client;

    bool initialized = false;
	public bool running=false;

    // Start is called before the first frame update
    void Awake()
    {
        running=false;
        Instance=this;
        Init();
    }

    #region GameStart
    public void InitGame(){
        if(initialized){return;}
		client=GetComponent<Client>();
        initialized=true;
    }

    public void Init(){
        LockStepTurnID = FirstLockStepTurnID;
        pendingActions = new PendingActions();
		confirmActions = new ConfirmActions();
		curState=new Queue<Pos>();
        InitGame();
    }

    public void StartGame(){
        running=true;
    }
    #endregion

    #region Turn
    //向网络层添加action
    public void SendAction(IAction action){
        if(!initialized) {
			Debug.Log("Game has not started, action will be ignored.");
			return;
		}
        client.AddGamingInfo(action.toOpinion());
		// Opinion opinion=action.toOpinion();
		// Opinion[] os=new Opinion[]{opinion};
		// RecieveActions(LockStepTurnID+1,"player1",os);
    }

    private bool LockStepTurn() {
		//Debug.Log("LockStepTurnID: " + LockStepTurnID);
		if(LockStepTurnID >= FirstLockStepTurnID + 1&&LockStepTurnID>pendingActions.currentTurn) {
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
		if(turn<=LockStepTurnID){
			Debug.LogError("can't confirm turn:"+turn+",current turn:"+LockStepTurnID);
			return;
		}
		confirmActions.Confirm(turn);
    }

    private bool NextTurn() {
		//Debug.Log("tcy当前动作数："+pendingActions.currentActionsCount+"  当前回合："+LockStepTurnID);
		if(confirmActions.ReadyForNextTurn() && pendingActions.ReadyForNextTurn()&&!readingData) {
			updating=true;//表示正在进行回合
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

	public static float currentime=0f;

    private float accumilatedTime = 0f;
	
	public static float frameLength = 0.005f; //5 miliseconds

	public int gameFramesPerLocksetpTurn=4;

	private int gameFrame=0;
	
	private bool isFirst = true;
	//called once per unity frame
	public void Update() {
		if(this.isFirst){
			this.isFirst = false;
			Debug.Log("tcy调用接口");
			client.AddReady();
		}
		if(!running){
			return;
		}
		
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
				// currentime+=frameLength;
				// ProcessActions(gameFrame);
				// updateMainGameObject();
				gameFrame++;
			}
		} else {
			currentime+=frameLength;
			ProcessActions(gameFrame);
			updateMainGameObject();
			gameFrame++;
			if(gameFrame > gameFramesPerLocksetpTurn) {
				curState=CurrentState();
				stateTurn=LockStepTurnID;
				gameFrame = 0;
			}
		}
	}


	private void updateMainGameObject(){
		GameObject[] objs=GameObject.FindGameObjectsWithTag(MainObjectTypes.MAIN_OBJECT);
		foreach (var obj in objs)
		{
			obj.GetComponent<BasicRoleControll>().onFixedUpdate();
			obj.GetComponent<SkillController>().onFixedUpdate();
		}
	}
    #endregion

	#region GetData
	protected Queue<Pos> CurrentState(){
		GameObject[] objs=GameObject.FindGameObjectsWithTag(MainObjectTypes.MAIN_OBJECT);
		Queue<Pos> states=new Queue<Pos>();
		foreach (var obj in objs)
		{
			Pos pos=new Pos();
			pos.UserId=obj.name;
			pos.Position=obj.transform.position.ToString();
			states.Enqueue(pos);
		}
		return states;
	}
	public Queue<Pos> getCurStateInfo(){
		return curState;
	}

	public int getCurStateTurn(){
		return stateTurn;
	}

	public void setCurStateInfo(Queue<Pos> position){
		foreach (var pos in position)
		{
			Vector3 p=PositionUtil.StringToVector3(pos.Position);
			GameObject.Find(pos.UserId).transform.position=p;
		}
		curState=position;
	}
	#endregion
}
