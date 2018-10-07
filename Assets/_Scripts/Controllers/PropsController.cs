/*
Maintaince Logs:
2011-11-01    XuMingzhao Initial version.Can creat random props and controll the effect.
						 Need to make props away from football.
						 Need to avoid too many thing act on the same thing.
2011-11-02    XuMingzhao When many thing act on the same thing, only effect once.
2011-11-03    XuMingzhao use List to fixed can't revery props bug.
2011-11-05    XuMingzhao Add DataPackage support.
2011-11-27    XuMingzhao fix props angle.
2011-12-01    XuMingzhao Add Prop Hurt Add Prop create speed cotroll.
2011-12-15    XuMingzhao    Add Sound Controll.
2011-12-26    Xu Mingzhao    Add Network support.
*/



using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class PropsController : MonoBehaviour 
{
	public class PropToRevert
	{
		public string name;
		public int playerID;
		public float timeToRevert;
		
		public PropToRevert(string propsName, int propsPlayerID, float propsTimeToRevert)
		{
			name = propsName;
			playerID = propsPlayerID;
			timeToRevert = propsTimeToRevert;
		}
		
		public int CompareTo(PropToRevert other)
		{
			return timeToRevert.CompareTo(other.timeToRevert);
		}
	}
	
	private int CompareProp(PropToRevert a, PropToRevert b)
	{
		return a.timeToRevert.CompareTo(b.timeToRevert);
	}
	
	public GameObject[] prefabProps;
	// save the prefabprop.
	
	private ArrayList occurProps = new ArrayList();
	// here are all the props we creat.
	private List<PropToRevert> revertProps = new List<PropToRevert>();
	
	// These three queue save when and what prop we need to revert.
	
	private float lastCreatTime;
	
	public GameObject[] players; // The two players
	public GameObject[] shigao;
	public GameObject football; 
	public GameObject[] goals; // The two goals, parallelism with players.
	public UIButton btnShoot;
	public GameObject[] playerIce = new GameObject[2];
	
	public PhysicMaterial normalFootBallMaterial;
	public PhysicMaterial heavyFootBallMaterial;
	
	public float CreatPropsIntervalTime = 8f;
	public float destroyPropsTime = 20f;
	
	public GameObject rogueLeft, rogueRight, rogueCur;
	
	
	[System.Serializable]
	public class PropsParameter
	{
		public float multipleBigGoal=1.5f, multipleSmallGoal=0.66f, timeGoalSize=8f;
		public float multipleSpeedup=1.5f, multipleSpeeddown=0.66f, timeSpeed=8f;
		public float multipleJumpHigh=2f, mutipleJumpLow=0.5f, timeJumpHeight=8f;
		public float multipleBigBall=1.5f, multipleSmallBall=0.66f, timeBallSzie=5f;
		public float timeDizzy=3f;
		public float timeBallBounce=5f;
		public float multipleGrowUp=1.35f, multipleShrink=0.75f, timePlayerSize=5f;
		public float timeRogue=10f, timeHurt=7f;
	}
	
	public PropsParameter propPara;
	private Vector3 initBallSize;
	private Vector3[] initGoalSize = new Vector3[2];
	private Color[] initColor = new Color[2];
	private float[] initAttackSpeed = new float[2];
	private float[] initDefendSpeed = new float[2];
	private float[] initJumpHeight = new float[2];
	
	#region Init / Add singleton
	private static PropsController instance;
    public static PropsController Instance
    {
        get
        {
            if (instance == null)
            {
                //instance = new GameController();
				instance = (PropsController)FindObjectOfType(typeof(PropsController)); 
            }
			if (!instance)
            {
                Debug.LogError("PropsController could not find itself!");
             } 

            return instance;
        }
    }
    #endregion
	
	void PropAppearAnimation(GameObject prop)
	{
		iTween.ScaleFrom(prop, iTween.Hash("scale", Vector3.zero,
		                                   "time", 1f,
		                                   "easetype", iTween.EaseType.easeOutElastic, 
		                                   "ignoretimescale", true));
	}
	
	IEnumerator PropDismissAnimation(GameObject prop)
	{
		iTween.ScaleTo(prop, iTween.Hash("scale", new Vector3(4f, 4f, 4f),
		                                   "time", 1f,
		                                   "easetype", iTween.EaseType.easeOutElastic, 
		                                   "ignoretimescale", true));
		
		iTween.ColorTo(prop, iTween.Hash("color", new Color(128f, 128f, 128f, 0f),
		                                 "time", 1f,
		                                 "easetype", iTween.EaseType.easeOutQuint,
		                                 "ignoretimescale", true));
		yield return new WaitForSeconds(1.5f);
		Destroy(prop);
	}
	
	[RPC]
	public void CreatProp(int propID, Vector3 propPos)
	{
		Debug.Log(propID);
		SoundController.Instance.play_prop_popup();
		Object tempprop = Instantiate( prefabProps[propID], propPos, Quaternion.identity );
		tempprop.name = prefabProps[propID].name;
		occurProps.Add(tempprop);
		PropAppearAnimation((GameObject) tempprop);
		Destroy(tempprop,destroyPropsTime);
		lastCreatTime = Time.time;
	}
	
	void CreatProp()
	{
		if (GameController.Instance.IsNetWorking && Network.isClient) return;
		lastCreatTime = Time.time;
		int propID = 0;
		if (GlobalManager.props != null && GlobalManager.props.Length > 0)
			propID = GlobalManager.props[ Random.Range(0, GlobalManager.props.Length) ];
		else
			return;
		Vector3 propPos;
		for (;;){
			bool isNearOther = false;
			float x = Random.Range(-7f, 7f);
			float y = Random.Range(3f, 7f);
			propPos = new Vector3(x, y, 0);
			foreach (Object tmpprop in occurProps){
				if (tmpprop != null && (((GameObject) tmpprop).transform.position - propPos).magnitude < 2)
					isNearOther = true;
			}
			if (!isNearOther) break;
		}
		if (GameController.Instance.IsNetWorking)
			networkView.RPC("CreatProp", RPCMode.Others, propID, propPos);
		
		SoundController.Instance.play_prop_popup();
		Object tempprop = Instantiate( prefabProps[propID], propPos, Quaternion.identity );
		tempprop.name = prefabProps[propID].name;
		occurProps.Add(tempprop);
		PropAppearAnimation((GameObject) tempprop);
		Destroy(tempprop,destroyPropsTime);
	}
	
	public void DestroyPropIcon(Vector3 pos){
		foreach (Object tempprop in occurProps){
			if (tempprop != null && (((GameObject) tempprop).transform.position - pos).magnitude < 0.001)
				StartCoroutine(PropDismissAnimation((GameObject) tempprop));
		}
	}
	
	public void Restart ()
	{
		//Destroy all props we creat.
		foreach ( Object tempprop in occurProps )
		{
			if (tempprop != null)
				Destroy(tempprop);
		}
		occurProps.Clear();
		
		//Revert all props effect.
		while (revertProps.Count > 0)
		{
			RevertOneObject();
		}
		
		lastCreatTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//if it's time to revert the nearest props.
		while ( revertProps.Count > 0 && Time.time > revertProps[0].timeToRevert )
		{
			RevertOneObject();
		}
		
		// if it's time to creatprop.
		if ( Time.time > lastCreatTime + CreatPropsIntervalTime )
		{
			if (GlobalManager.isServer)
			{
				CreatProp();
			}
		}
		
	}
	
	public void GetProp(string propName, int playerID)
	{
		SoundController.Instance.play_prop_touch();
		Debug.Log("Get" + propName + "  Time:" + Time.time);
		SendMessage("Get_" + propName, playerID);
	}
	
	
	// Use this for initialization
	void Start () 
	{
		CreatPropsIntervalTime = GlobalManager.propTime;
		lastCreatTime = Time.time;
		
		//record all the init parameter.
		
		initBallSize = football.transform.localScale;
		
		initColor[0] = players[0].GetComponentInChildren<SkinnedMeshRenderer>().material.color;
		initColor[1] = players[1].GetComponentInChildren<SkinnedMeshRenderer>().material.color;
		
		initGoalSize[0] = goals[0].transform.localScale;
		initGoalSize[1] = goals[1].transform.localScale;
		
		initAttackSpeed[0] = players[0].GetComponent<Controller2D>().competence.attackSpeed;
		initAttackSpeed[1] = players[1].GetComponent<Controller2D>().competence.attackSpeed;
		
		initJumpHeight[0] = players[0].GetComponent<Controller2D>().competence.jumpHeight;
		initJumpHeight[1] = players[1].GetComponent<Controller2D>().competence.jumpHeight;
		
		initDefendSpeed[0] = players[0].GetComponent<Controller2D>().competence.defendSpeed;
		initDefendSpeed[1] = players[1].GetComponent<Controller2D>().competence.defendSpeed;
	}
	
	void RevertOneObject()
	{
		string name = revertProps[0].name;
		int id = revertProps[0].playerID;
		revertProps.RemoveAt(0);
		SendMessage(name,id);
		Debug.Log(name + "  Time:" + Time.time);
		// revert the state that the most recent prop induce.
	}
	
	void AddPropToRevert(string name,int id,float time)
	{
		PropToRevert newprop = new PropToRevert(name, id, time);
		
		// remove the same name and playerid which should be revert.
		for (int i=0; i<revertProps.Count; i++)
		{
			while (i<revertProps.Count && revertProps[i].name == name && revertProps[i].playerID == id)
			{
				revertProps.RemoveAt(i);
			}
		}
		
		// make the list sorted.
		revertProps.Add(newprop);
		revertProps.Sort(CompareProp);
	}
	
#region	Design every props.
	
	#region	Prop ball size.
	
	void RevertBallSize(int playerID)
	{
			football.transform.localScale = initBallSize;
	}
	
	void GetBallSize(int playerID)
	{
		float duringTime = propPara.timeBallSzie;
		AddPropToRevert("RevertBallSize", 0, Time.time + duringTime);
	}
	
	void Get_prop_bigball(int playerID)
	{
		football.transform.localScale = initBallSize * propPara.multipleBigBall;
		GetBallSize(playerID);
	}
	
	void Get_prop_smallball(int playerID)
	{
		football.transform.localScale = initBallSize * propPara.multipleSmallBall;
		GetBallSize(playerID);
	}
	#endregion
	
	#region	Prop ball bounce.
	
	void RevertBallBounce(int playerID)
	{
			football.collider.material = normalFootBallMaterial;
	}
	
	void GetBallBounce(int playerID)
	{
		float duringTime = propPara.timeBallBounce;
		AddPropToRevert("RevertBallBounce", 0, Time.time + duringTime);	
	}
	
	void Get_prop_heavyball(int playerID)
	{
		football.collider.material = heavyFootBallMaterial;
		GetBallBounce(playerID);
	}
	#endregion
	
	#region	Prop goal size.
	
	void RevertGoalSize(int playerID)
	{
		goals[1-playerID].transform.localScale = initGoalSize[1-playerID];
	}
	
	void GetGoalSize(int playerID)
	{
		float duringTime = propPara.timeGoalSize;
		AddPropToRevert("RevertGoalSize", playerID, Time.time + duringTime);
	}
	
	void Get_prop_biggoal(int playerID)
	{
		goals[1-playerID].transform.localScale = initGoalSize[1-playerID] * propPara.multipleBigGoal;
		GetGoalSize(playerID);
	}
	
	void Get_prop_smallgoal(int playerID)
	{
		goals[1-playerID].transform.localScale = initGoalSize[1-playerID] * propPara.multipleSmallGoal;
		GetGoalSize(playerID);
	}
	#endregion
	
	#region	Prop player speed.
	
	void RevertPlayerSpeed(int playerID)
	{
		players[playerID].GetComponentInChildren<SkinnedMeshRenderer>().material.color = initColor[playerID];
		Controller2D controller = players[playerID].GetComponent<Controller2D>();
		controller.competence.attackSpeed = initAttackSpeed[playerID];
		controller.competence.defendSpeed = initDefendSpeed[playerID];
	}
	
	void GetPlayerSpeed(int playerID)
	{
		float duringTime = propPara.timeSpeed;
		AddPropToRevert("RevertPlayerSpeed", playerID, Time.time + duringTime);
	}
	
	void Get_prop_speedup(int playerID)
	{
		Controller2D controller = players[playerID].GetComponent<Controller2D>();
		players[playerID].GetComponentInChildren<SkinnedMeshRenderer>().material.color = initColor[playerID];
		controller.competence.attackSpeed = initAttackSpeed[playerID] * propPara.multipleSpeedup;
		controller.competence.defendSpeed = initAttackSpeed[playerID] * propPara.multipleSpeedup;
		GetPlayerSpeed(playerID);
	}
	
	void Get_prop_speeddown(int playerID)
	{
		Controller2D controller = players[playerID].GetComponent<Controller2D>();
		players[playerID].GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;
		controller.competence.attackSpeed = initAttackSpeed[playerID] * propPara.multipleSpeeddown;
		controller.competence.defendSpeed = initAttackSpeed[playerID] * propPara.multipleSpeeddown;
		GetPlayerSpeed(playerID);
	}
	#endregion
	
	#region Prop player dizzy
	
	void RevertPlayerDizzy(int playerID)
	{
		Controller2D controller = players[1-playerID].GetComponent<Controller2D>();
		players[1-playerID].SendMessage("ResumeAnimation");
		controller.canControl = true;
		playerIce[1-playerID].SetActiveRecursively(false);
	}
	
	void GetPlayerDizzy(int playerID)
	{
		float duringTime = propPara.timeDizzy;
		AddPropToRevert("RevertPlayerDizzy", playerID, Time.time + duringTime);
	}
	
	void Get_prop_oppodizzy(int playerID)
	{
		Controller2D controller = players[1-playerID].GetComponent(typeof(Controller2D)) as Controller2D;
		players[1-playerID].SendMessage("PauseAnimation");
		controller.canControl = false;
		playerIce[1-playerID].SetActiveRecursively(true);
		GetPlayerDizzy(playerID);
	}
	
	void Get_prop_selfdizzy(int playerID)
	{
		Controller2D controller = players[playerID].GetComponent(typeof(Controller2D)) as Controller2D;
		players[playerID].SendMessage("PauseAnimation");
		controller.canControl = false;
		playerIce[playerID].SetActiveRecursively(true);
		GetPlayerDizzy(1-playerID);
	}
	
	#endregion
	
	#region Prop player jump height
	
	void RevertPlayerJumpHeight(int playerID)
	{
		Controller2D controller = players[playerID].GetComponent<Controller2D>();
		players[playerID].GetComponentInChildren<SkinnedMeshRenderer>().material.color = initColor[playerID];
		controller.competence.jumpHeight = initJumpHeight[playerID];
	}
	
	void GetPlayerJumpHeight(int playerID)
	{
		float duringTime = propPara.timeJumpHeight;
		AddPropToRevert("RevertPlayerJumpHeight", playerID, Time.time + duringTime);
	}
	
	void Get_prop_highjump(int playerID)
	{
		Controller2D controller = players[playerID].GetComponent<Controller2D>();
		players[playerID].GetComponentInChildren<SkinnedMeshRenderer>().material.color = initColor[playerID];
		controller.competence.jumpHeight = initJumpHeight[playerID] * propPara.multipleJumpHigh;
		GetPlayerJumpHeight(playerID);
	}
	
	void Get_prop_lowjump(int playerID)
	{
		Controller2D controller = players[playerID].GetComponent<Controller2D>();
		players[playerID].GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;
		controller.competence.jumpHeight = initJumpHeight[playerID] * propPara.mutipleJumpLow;
		GetPlayerJumpHeight(playerID);
	}
	#endregion
	
	#region Prop player size change
	
	void RevertPlayerSize(int playerID)
	{
		players[playerID].transform.localScale = new Vector3(1f, 1f, 1f);
	}
	
	void GetPlayerSize(int playerID)
	{
		float duringTime = propPara.timePlayerSize;
		AddPropToRevert("RevertPlayerSize", playerID, Time.time + duringTime);
	}
	
	void Get_prop_growup(int playerID)
	{
		float ratio = propPara.multipleGrowUp;
		players[playerID].transform.localScale = new Vector3(ratio, ratio, ratio);
		GetPlayerSize(playerID);
	}
	
	void Get_prop_shrink(int playerID)
	{
		float ratio = propPara.multipleShrink;
		players[playerID].transform.localScale = new Vector3(ratio, ratio, ratio);
		GetPlayerSize(playerID);
	}
	#endregion
	
	#region	Prop Rogue
	
	void RevertRogue(int playerID)
	{
		if (GameController.Instance.IsNetWorking){
			if (Network.isServer)
				Network.Destroy(rogueCur);
		}
		else
			Destroy(rogueCur);
	}
	
	void GetRogue(int playerID)
	{
		float duringTime = propPara.timeRogue;
		AddPropToRevert("RevertRogue", 0, Time.time + duringTime);
	}
	
	void Get_prop_rogue(int playerID)
	{
		if (rogueCur != null)
		{
			if (GameController.Instance.IsNetWorking){
				if (Network.isServer)
					Network.Destroy(rogueCur);
			}
			else
				Destroy(rogueCur);
		}
		if ( Random.Range(0,2) == 0)
			if (GameController.Instance.IsNetWorking){
				if (Network.isServer)
					rogueCur = Network.Instantiate(rogueLeft, rogueLeft.transform.position, rogueLeft.transform.rotation, 0) as GameObject;
			}
			else
				rogueCur = Instantiate(rogueLeft) as GameObject;
		else
			if (GameController.Instance.IsNetWorking){
				if (Network.isServer)
					rogueCur = Network.Instantiate(rogueRight, rogueRight.transform.position, rogueRight.transform.rotation, 0) as GameObject;
			}
			else
				rogueCur = Instantiate(rogueRight) as GameObject;
		rogueCur.GetComponent<RogueAI>().football = football;
		GetRogue(playerID);
	}
	#endregion
	
	#region Prop Hurt
		void RevertPlayerHurt(int playerID)
		{
			players[1-playerID].GetComponent<Controller2D>().canKick = true;
			shigao[1-playerID].SetActiveRecursively(false);
			if (players[1-playerID] == GameController.Instance.myPlayer)
				btnShoot.SetControlState(UIButton.CONTROL_STATE.NORMAL);
		}
		
		void GetPlayerHurt(int playerID)
		{
			float duringTime = propPara.timeHurt;
			AddPropToRevert("RevertPlayerHurt", playerID, Time.time + duringTime);
		}
		
		void Get_prop_oppohurt(int playerID)
		{
			players[1-playerID].GetComponent<Controller2D>().canKick = false;
			shigao[1-playerID].SetActiveRecursively(true);
			if (players[1-playerID] == GameController.Instance.myPlayer)
				btnShoot.SetControlState(UIButton.CONTROL_STATE.DISABLED);
			GetPlayerHurt(playerID);
		}
		
		void Get_prop_selfhurt(int playerID)
		{
			players[playerID].GetComponent<Controller2D>().canKick = false;
			shigao[playerID].SetActiveRecursively(true);
			if (players[playerID] == GameController.Instance.myPlayer)
				btnShoot.SetControlState(UIButton.CONTROL_STATE.DISABLED);
			GetPlayerHurt(1-playerID);
		}
	#endregion
	
	#region Prop player energy
	
		void Get_prop_getenergy20(int playerID)
		{
			GameController.Instance.GetEnergyDelta(playerID, 20);
		}
		
		void Get_prop_getenergy40(int playerID)
		{
			GameController.Instance.GetEnergyDelta(playerID, 40);
		}
		
		void Get_prop_getenergy60(int playerID)
		{
			GameController.Instance.GetEnergyDelta(playerID, 60);
		}
	
	#endregion
	
#endregion
}
