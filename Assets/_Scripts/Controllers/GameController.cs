/*
Maintaince Logs:
2011-09-12    Waigo    Initial version.  Controlling the game. Spawn new levels.
2011-10-20    XuMingzhao    Add SelectPlayer and IsPlatformPortable function.
2011-10-21    XuMingzhao    Change the player can controll only when two people select there player 
							Add Score and Restart Controll.
2011-10-28    XuMingzhao	Fixed in resume time player goals more than one time.
							Add a random force to football when game start.
2011-11-03    XuMingzhao    Add a little time rest when scene begin.
2011-11-10    XuMingzhao    Add competence support.
2011-11-27    XuMingzhao    Add skill point support.
2011-11-29    XuMingzhao    Add unlock level function. count down time text.
2011-12-05    XuMingzhao    Add skill controll.
2011-12-14    XuMingzhao    Add skill competence controll.
2011-12-15    XuMingzhao    Add Sound Controll.
2011-12-16    XuMingzhao    Add Camera Zoom Controll.
2011-12-18    XuMingzhao    Add Help Controll.
*/

using UnityEngine;
using System.Collections;

public enum GameState { Playing, Pause, GameOver };

public class GameController : MonoBehaviour {

    public static GameState gameState;
	public UIPanelManager pannelManager;
	public SpriteText scoreText, timeText, winCoinText, loseCoinText, winPointText, losePointText, countDownTimeText;
	public bool IsNetWorking;
	private int playerCount;
	public float getEnergyInterval = 2.5f;
	public UIProgressBar energyBar;
	public UIButton btnSkill, btnNextLevel;
	
	public Color mySkillColor, oppoSkillColor;
	
	public UIButton Time3, Time2, Time1, TexGo, TexGoal, TexGoldenGoal;
    
	private int coinNum = 0, pointNum = 0;
	public int[] energyNum = new int[2];
	private float[] lastGetEnergyTime = new float[2];
	private Vector3 footballDir;
	
	public Camera mainCamera, guiCamera, skillCamera;
	public Transform playerTrans;
	public GameObject[] players;
	public GameObject myPlayer,oppoPlayer;
	public GameObject footBall;
	
	public float playingTime = 60f;
	
	private bool curCantDoSkill;
	private float lastGetScoreTime = 0f, lastDoSkillTime = 0f;
	const int playerLeft = 0 , playerRight = 1;
	private int ScoreLeft , ScoreRight;
	private Vector3 initMyPosition , initBallPosition , initOppoPosition;
	
	private Ray rayUp,rayLeft,rayRight;
	private RaycastHit hit;

    #region Init / Add singleton
    private static GameController instance;
    public static GameController Instance
    {
        get
        {
            if (instance == null)
            {
                //instance = new GameController();
				instance = (GameController)FindObjectOfType(typeof(GameController)); 
            }
			if (!instance)
            {
                Debug.LogError("GameController could not find itself!");
             } 

            return instance;
        }
    }
    #endregion 
	
	void HelpEnd()
	{
		StartCoroutine(ResumeGame());
		pannelManager.BringIn("BtnControllPanel");
	}
	
	void Awake () 
	{
		Time.timeScale = 0.0f;
		gameState = GameState.Pause;
		if (!IsNetWorking)
		{
			SelectPlayer(0);
			if (GlobalManager.curLevelNum == 1) {
				bool isPortable = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer);
				if (isPortable) {
					pannelManager.BringIn("HelpPanel1");
				} else {
					pannelManager.BringIn("HelpPanelPC");
				}
			}
			else if (GlobalManager.curLevelNum == 2)
				pannelManager.BringIn("HelpPanel3");
			else
        		StartCoroutine(ResumeGame());
		}
		else{
			Network.isMessageQueueRunning = true;
		}
	}
	
	void SendOptions()
	{
		int footballID  = PlayerPrefs.GetInt("FootballID");
		int ClothID     = PlayerPrefs.GetInt("ClothID");
		int HeadID      = PlayerPrefs.GetInt("HeadID");
		float Attack    = PlayerPrefs.GetFloat("Attack",0.9f);
		float Defend    = PlayerPrefs.GetFloat("Defend",0.9f);
		float Jump      = PlayerPrefs.GetFloat("Jump",1.6f);
		float JumpDelta = PlayerPrefs.GetFloat("JumpDelta",0.3f);
		int Skill       = PlayerPrefs.GetInt("Skill", 10);
		networkView.RPC("ReceiveOptions", RPCMode.Others, 
		                footballID, 
		                ClothID, 
		                HeadID, 
		                Attack, 
		                Defend, 
		                Jump, 
		                JumpDelta, 
		                Skill);
	}
	
	[RPC]
	void ReceiveOptions(int footballID, 
		                int ClothID, 
		                int HeadID, 
		                float Attack, 
		                float Defend, 
		                float Jump, 
		                float JumpDelta, 
		                int Skill)
	{
		GlobalManager.oppoFootballID = footballID;
		GlobalManager.oppoClothID = ClothID;
		GlobalManager.oppoHeadID = HeadID;
		GlobalManager.oppoAttack = Attack;
		GlobalManager.oppoDefend = Defend;
		GlobalManager.oppoJump = Jump;
		GlobalManager.oppoJumpDelta = JumpDelta;
		GlobalManager.oppoSkill = Skill;
		AvatarController.Instance.NetWorkChange();
		ChangeCompentence();
	}
	
	
	[RPC]
	public void NewPlayerCome()
	{
		playerCount++;
		if (playerCount>=2)
		{
			StartCoroutine(ResumeGame());
		}
	}
	
	void ChangeCompentence()
	{
		Controller2D controller = myPlayer.GetComponent<Controller2D>();
		controller.competence.attackSpeed = PlayerPrefs.GetFloat("Attack",0.9f);
		controller.competence.defendSpeed = PlayerPrefs.GetFloat("Defend",0.9f);
		controller.competence.jumpHeight = PlayerPrefs.GetFloat("Jump",1.6f);
		controller.competence.jumpHeightDelta = PlayerPrefs.GetFloat("JumpDelta",0.3f);
		
		
		controller = oppoPlayer.GetComponent<Controller2D>();
		controller.competence.attackSpeed = GlobalManager.oppoAttack;
		controller.competence.defendSpeed = GlobalManager.oppoDefend;
		controller.competence.jumpHeight = GlobalManager.oppoJump;
		controller.competence.jumpHeightDelta = GlobalManager.oppoJumpDelta;
		PropsController.Instance.SendMessage("Start");
	}
	
	public void SelectPlayer(int i)
	{
		GameController.Instance.playerTrans = players[i].transform;
		myPlayer = players[i];
		oppoPlayer = players[1 - i];
		InputController.Instance.player = myPlayer;
		InputController.Instance.SetController2D();
		
		initBallPosition = footBall.transform.position;
		initMyPosition = myPlayer.transform.position;
		initOppoPosition = oppoPlayer.transform.position;
		
		if (IsNetWorking)
			networkView.RPC("NewPlayerCome", RPCMode.All);
		mainCamera.SendMessage("CameraReset");
	}
	
	void Start()
	{
		if (Network.isServer)
			SelectPlayer(0);
		if (Network.isClient)
			SelectPlayer(1);
		if (IsNetWorking)
			SendOptions();
		else
			ChangeCompentence();
	}
	
	void PauseGame()
	{
		if (IsNetWorking)
			networkView.RPC("PauseGameSingle", RPCMode.Others);
		pannelManager.BringIn("PausePanel");
		Time.timeScale = 0.0f; //Pause the game
        gameState = GameState.Pause;
	}
	
	[RPC]
	void PauseGameSingle()
	{
		pannelManager.BringIn("PausePanel");
		Time.timeScale = 0.0f; //Pause the game
        gameState = GameState.Pause;
	}
	
	void ResumePauseGame()
	{
		if (IsNetWorking)
			networkView.RPC("ResumePauseGameSingle", RPCMode.Others);
		Time.timeScale = 1.0f;
		gameState = GameState.Playing;
		pannelManager.BringIn("BtnControllPanel");
	}
	
	[RPC]
	void ResumePauseGameSingle()
	{
		Time.timeScale = 1.0f;
		gameState = GameState.Playing;
		pannelManager.BringIn("BtnControllPanel");
	}

    void StartGame()
    {
		SoundController.Instance.play_beep_start();
		(myPlayer.GetComponent(typeof(Controller2D)) as Controller2D).canControl = true;
		
		(oppoPlayer.GetComponent(typeof(Controller2D)) as Controller2D).canControl = true;
		footBall.SendMessage("StartGame",SendMessageOptions.DontRequireReceiver);
		
        Time.timeScale = 1.0f;
        gameState = GameState.Playing;
    }
	
	void GetCoins(int coinDelta)
	{
		int curCoins = PlayerPrefs.GetInt("Coins");
		curCoins += coinDelta;
		PlayerPrefs.SetInt("Coins",curCoins);
		PlayerPrefs.Save();
	}
	
	void GetPoints(int pointDelta)
	{
		int curPoints = PlayerPrefs.GetInt("SkillPoints");
		curPoints += pointDelta;
		PlayerPrefs.SetInt("SkillPoints",curPoints);
		PlayerPrefs.Save();
	}
	
	void LoseGame()
	{
		SoundController.Instance.play_fail();
		coinNum = GlobalManager.moneyBase - (int)(Mathf.Abs(ScoreLeft - ScoreRight) * GlobalManager.moneyDelta);
		pointNum = GlobalManager.pointBase;
		pointNum /= 2;
		if (coinNum < 0) coinNum = 0;
		if (pointNum < 0) pointNum = 0;
		GetCoins(coinNum);
		GetPoints(pointNum);
		loseCoinText.Text = coinNum.ToString();
		losePointText.Text = pointNum.ToString();
		pannelManager.BringIn("LosePanel");
	}
	
	void LoadNextLevel()
	{
		GlobalManager.LoadLevel(GlobalManager.curLevelNum + 1);
	}
	
	void ReloadLevel()
	{
		if (GameController.Instance.IsNetWorking)
		{
			networkView.RPC("ReloadLevelSingle", RPCMode.Others);
			Network.isMessageQueueRunning = false;
		}
		GlobalManager.LoadLevel(GlobalManager.curLevelNum);
	}
	
	[RPC]
	void ReloadLevelSingle()
	{
		Network.isMessageQueueRunning = false;
		GlobalManager.LoadLevel(GlobalManager.curLevelNum);
	}
	
	void LoadMenu()
	{
		if (GameController.Instance.IsNetWorking && (Network.isServer || Network.isClient)){
			Network.Disconnect();
		}
		GlobalManager.LoadScene("MainMenu");
	}
	
	void LevelUnlock()
	{
		int lockNum = PlayerPrefs.GetInt("LevelLock", 2);
		lockNum |= (1 << (GlobalManager.curLevelNum + 1));
		PlayerPrefs.SetInt("LevelLock", lockNum);
		PlayerPrefs.Save();
	}
	
	void WinGame()
	{
		SoundController.Instance.play_goal_or_win();
		coinNum = GlobalManager.moneyBase + (int)(Mathf.Abs(ScoreLeft - ScoreRight) * GlobalManager.moneyDelta);
		pointNum = GlobalManager.pointBase + (int)(Mathf.Abs(ScoreLeft - ScoreRight) * GlobalManager.pointDelta);
		GetCoins(coinNum);
		GetPoints(pointNum);
		winCoinText.Text = coinNum.ToString();
		winPointText.Text = pointNum.ToString();
		
		if (GlobalManager.curLevelNum == 18)
			btnNextLevel.Hide(true);
		pannelManager.BringIn("WinPanel");
		LevelUnlock();
	}

    IEnumerator GameOver()
    {
		gameState = GameState.GameOver;
		TexGoldenGoal.Hide(true);
		SoundController.Instance.play_beep_end();
		yield return new WaitForSeconds(1.1f);
        Time.timeScale = 0.0f; //Pause the game
		
		if ( ( myPlayer == players[0] ) ^ (ScoreLeft > ScoreRight) )
			LoseGame();
		else
			WinGame();
    }
	
	void ResetPlayer(GameObject player , Vector3 pos)
	{
		(player.GetComponent(typeof(Controller2D)) as Controller2D).canControl = false;
		player.transform.position = pos;
		(player.GetComponent(typeof(ControllerAnimation)) as ControllerAnimation).Reset();
	}
	
	IEnumerator ResumeGame()
	{
		Debug.Log("reset");
		PropsController.Instance.Restart();
		ResetPlayer(myPlayer, initMyPosition);
		ResetPlayer(oppoPlayer, initOppoPosition);
		footBall.SendMessage("Reset",initBallPosition,SendMessageOptions.DontRequireReceiver);
		mainCamera.SendMessage("CameraReset");
		Time.timeScale = 1f;
		Time3.Hide(false);
		yield return new WaitForSeconds(1.0f);
		Time3.Hide(true);
		Time2.Hide(false);
		yield return new WaitForSeconds(1.0f);
		Time2.Hide(true);
		Time1.Hide(false);
		yield return new WaitForSeconds(1.0f);
		Time1.Hide(true);
		TexGo.Hide(false);
		StartGame();
		yield return new WaitForSeconds(1.0f);
		TexGo.Hide(true);
	}
	
	void myEnergyChange()
	{
		int playerNum = (myPlayer == players[0]) ? 0 : 1;
		if (energyNum[playerNum] >= 100) 
		{
			btnSkill.Hide(false);
			btnSkill.SetControlState(UIButton.CONTROL_STATE.NORMAL);
		}
		else 
		{	
			btnSkill.SetControlState(UIButton.CONTROL_STATE.DISABLED);
			btnSkill.Hide(true);
		}
	}
	
	public void GetEnergyDelta(int playerNum, int delta)
	{
		energyNum[playerNum] += delta;
		if (players[playerNum] == myPlayer)
			energyBar.Value = energyNum[playerNum] / 100f;
		myEnergyChange();
	}
	
	[RPC]
	public void GetEnergy(int playerNum)
	{
		if (Time.time - lastGetEnergyTime[playerNum] < 2.5f || gameState != GameState.Playing) return;
		if (IsNetWorking)
			networkView.RPC("GetEnergy", RPCMode.Others, playerNum);
		lastGetEnergyTime[playerNum] = Time.time;
		int skillDelta;
		if (players[playerNum] == myPlayer)
			skillDelta = PlayerPrefs.GetInt("Skill",10);
		else
			skillDelta = GlobalManager.oppoSkill;
		energyNum[playerNum] += skillDelta + 1;
		if (energyNum[playerNum] > 100) energyNum[playerNum] = 100;
		if (players[playerNum] == myPlayer)
			energyBar.Value = energyNum[playerNum] / 100f;
		myEnergyChange();
	}
	
	public void UseSkillMe()
	{
		if (myPlayer == players[0])
			UseSkill(0);
		else 
			UseSkill(1);
	}
	
	bool CanDoSkill(int playerNum)
	{
		int direct = 0;
		if (playerNum == 0) direct = 1;
		else direct = -1;
		Vector3 dis = footBall.transform.position 
					- players[playerNum].transform.position
					- new Vector3(0f, 1f, 0f);
		//Debug.Log(dis.x.ToString() + "  " + direct.ToString() + "  " +dis.magnitude.ToString());
		return (dis.x * direct > -1 && dis.sqrMagnitude < 9);
	}
	
	void moveFootballFromBody(int playerNum)
	{
		Vector3 dis = footBall.transform.position 
					- players[playerNum].transform.position
					- new Vector3(0f, 1f, 0f);
		if (dis.magnitude < 1.1f) dis *= 1.1f/dis.magnitude;
		
		footBall.transform.position = dis 
									+ players[playerNum].transform.position
									+ new Vector3(0f, 1f, 0f);
		
		dis = footBall.transform.position 
			- players[1 - playerNum].transform.position
			- new Vector3(0f, 1f, 0f);
		if (dis.magnitude < 1.1f) dis *= 1.1f/dis.magnitude;
		
		footBall.transform.position = dis 
									+ players[1 - playerNum].transform.position
									+ new Vector3(0f, 1f, 0f);
	}
	
	[RPC]
	public IEnumerator DoSkill(int playerNum, bool canDoSkill, bool receiver)
	{
		if (IsNetWorking && !receiver)
			networkView.RPC("DoSkill", RPCMode.Others, playerNum, canDoSkill, true);
		SoundController.Instance.play_fireball_burning();
		if (players[playerNum] == myPlayer)
			skillCamera.backgroundColor = mySkillColor;
		else
			skillCamera.backgroundColor = oppoSkillColor;
		energyNum[playerNum] = 0;
		if (players[playerNum] == myPlayer)
			energyBar.Value = 0f;
		myEnergyChange();
		Vector3 footballVel = footBall.rigidbody.velocity;
		footBall.rigidbody.isKinematic = true;
		if (!canDoSkill)
		{
			energyNum[playerNum] = 50;
			if (players[playerNum] == myPlayer)
				energyBar.Value = energyNum[playerNum] / 100f;
		}
		yield return new WaitForSeconds(0.1f); // To make the football stop.
		moveFootballFromBody(playerNum);
		pannelManager.Dismiss();
		mainCamera.enabled = false;
		guiCamera.enabled = false;
		gameState = GameState.Pause;
		if (canDoSkill)
			skillCamera.SendMessage("PlayAnimateAround", players[playerNum].transform.position);
		else
			skillCamera.SendMessage("PlayAnimateQuick", players[playerNum].transform.position);
		Time.timeScale = 0f;
		yield return new WaitForSeconds(0.1f);
		
		mainCamera.enabled = true;
		guiCamera.enabled = true;
		pannelManager.BringIn("BtnControllPanel");
		gameState = GameState.Playing;
		if (canDoSkill)
		{
			SoundController.Instance.play_fireball_shoot();
			Vector3 footballTarget = new Vector3(10f * (playerNum==0 ? 1 : -1), 1f, 0f);
			footballDir = footballTarget - footBall.transform.position;
			footballDir.Normalize();
			if (!IsNetWorking || Network.isServer){
				StartCoroutine(FootballMoveTo(footballTarget, 40f));
			}
		}
		else{
			footBall.rigidbody.isKinematic = false;
			footBall.rigidbody.velocity = footballVel;
			curCantDoSkill = false;
		}
	}
	
	public IEnumerator FootballMoveTo(Vector3 target, float speed){
		Vector3 startPos = footBall.transform.position;
		float duringTime = (startPos - target).magnitude / speed;
		for (float curTime = 0f; curTime < duringTime + 0.02f; curTime += 0.02f){
			yield return new WaitForSeconds(0.02f);
			float t = curTime / duringTime;
			if (t > 1) t = 1;
			footBall.transform.position = Vector3.Lerp(startPos, target, t);
		}
		SkillEnd();
	}
	
	[RPC]
	public void UseSkill(int playerNum)
	{
		if (IsNetWorking && !Network.isServer){
			networkView.RPC("UseSkill", RPCMode.Server, playerNum);
			return;
		}
		Debug.Log(playerNum.ToString() + energyNum[playerNum]);
		if (players[playerNum].GetComponent<Controller2D>().canControl &&
		gameState == GameState.Playing && energyNum[playerNum] >= 100 && !curCantDoSkill)
		{
			bool canDoSkill = CanDoSkill(playerNum);
			curCantDoSkill = true;
			StartCoroutine(DoSkill(playerNum, canDoSkill, false));
		}
	}
	
	public void SkillEnd()
	{
		footBall.rigidbody.isKinematic = false;
		footBall.rigidbody.AddForce(footballDir * 100f);
		curCantDoSkill = false;
	}
	
	[RPC]
	public IEnumerator GetScore(int playerDirect)
	{
		//Avoid in resume time player goals more than one time.
		if (Time.time > lastGetScoreTime + 2.5f)
		{
			lastGetScoreTime = Time.time;
			if (IsNetWorking)
				networkView.RPC("GetScore", RPCMode.Others, playerDirect);
			if (gameState != GameState.GameOver)
			{
				if (playerDirect == playerLeft) ScoreLeft++;
				if (playerDirect == playerRight) ScoreRight++;
				if (players[playerDirect] == myPlayer)
					SoundController.Instance.play_goal_or_win();
				else
					SoundController.Instance.play_fail();
				scoreText.Text = ScoreLeft + " : " + ScoreRight;
				TexGoal.Hide(false);
	
				if (playingTime > 0)
				{
					gameState = GameState.Pause;
					yield return new WaitForSeconds(1f);
					TexGoal.Hide(true);
					yield return new WaitForSeconds(1f);
					StartCoroutine(ResumeGame());
				}
				else
				{
					yield return new WaitForSeconds(1f);
					TexGoal.Hide(true);
				}
			}
		}
	}
	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		float tmpPlayingTime = playingTime;
		if (stream.isWriting){
			stream.Serialize(ref tmpPlayingTime);
		}
		if (stream.isReading){
			stream.Serialize(ref tmpPlayingTime);
			playingTime = tmpPlayingTime + (float) (Network.time - info.timestamp);
		}
	}

	void Update () 
	{
		if (gameState == GameState.Playing)
		{
			float lastPlayingTime = playingTime;
			playingTime -= Time.deltaTime;
			if (playingTime < 10f)
			{
				if (Mathf.CeilToInt(lastPlayingTime) != Mathf.CeilToInt(playingTime))
					SoundController.Instance.play_count_down();
			}
			if (playingTime < 0)
			{
				
				if (ScoreLeft != ScoreRight)
					StartCoroutine(GameOver());
				else
					TexGoldenGoal.Hide(false);
				playingTime = 0;
				
			}
			timeText.Text = playingTime.ToString("F0");
		}
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		MasterServer.UnregisterHost();
		Network.Disconnect();
		Time.timeScale = 0;
		pannelManager.BringIn("AnotherLeftPanel");
	}
	
	void OnApplicationPause (bool pause)
	{
		if (IsNetWorking){
			Network.Disconnect();
		}
	}
	
	void OnDisconnectedFromServer (NetworkDisconnection info) 
	{
		Time.timeScale = 0;
    	pannelManager.BringIn("AnotherLeftPanel");
	}
}