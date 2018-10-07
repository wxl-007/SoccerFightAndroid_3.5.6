/*
Maintaince Logs:
2011-11-08    XuMingzhao    Initial version.  Add many global value.
2011-11-10    XiMingzhao    Add PlayerPrefs description here.
2011-11-27    XuMingzhao    Add skill point description.
2011-11-29    XuMingzhao    Add LevelLock function.
2011-12-14    XuMingzhao    Add Scene to load string, Skill playerPrefs.
2011-12-15    XuMingzhao    Add level money and point controll.
*/

using UnityEngine;
using System.Collections;

public class LevelCharactorAttr
{
	public float attack;
	public float defend;
	public float jump;
	public float jumpDelta;
	public int skill;
	public int clothID;
	public int headID;
	public int moneyBase;
	public float moneyDelta;
	public int pointBase;
	public float pointDelta;
	public int AILevel;
	public float propTime;
	public int[] propFrequency;
	
	public LevelCharactorAttr(float _attack, 
	                          float _defend, 
	                          float _jump, 
	                          float _jumpDelta, 
	                          int _skill, 
	                          int _clothID, 
	                          int _headID, 
	                          int _monetBase, 
	                          float _moneyDelta, 
	                          int _pointBase, 
	                          float _pointDelta, 
	                          int _AILevel,
	                          float _propTime)
	{
		this.attack = _attack;
		this.defend = _defend;
		this.jump = _jump;
		this.jumpDelta = _jumpDelta;
		this.skill = _skill;
		this.clothID = _clothID;
		this.headID = _headID;
		this.moneyBase = _monetBase;
		this.moneyDelta = _moneyDelta;
		this.pointBase = _pointBase;
		this.pointDelta = _pointDelta;
		this.AILevel = _AILevel;
		this.propTime = _propTime;
	}
	
	public void AddPropFrequency(params int[] values)
	{
		int n = values.Length;
		this.propFrequency = new int[n];
		for (int i=0; i<n; i++){
			this.propFrequency[i] = values[i];
		}
	}
}

public class GlobalManager : MonoBehaviour 
{
	static public int curLevelNum = 0;
	
	static public int oppoHeadID = 0;
	static public int oppoClothID = 0;
	static public int oppoFootballID = 0;
	
	static public bool isServer = true;
	static public bool isSingleMode = true;
	
	static public float oppoJump = 1.6f;
	static public float oppoAttack = 0.9f;
	static public float oppoDefend = 0.9f;
	static public float oppoJumpDelta = 0.3f;
	static public int oppoSkill = 10;
	
	static public int moneyBase;
	static public float moneyDelta;
	static public int pointBase;
	static public float pointDelta;
	
	static public int AILevel = 1;
	
	static public int[] props;
	static public float propTime;
	
	static public string sceneToLoad;
	
	static public LevelCharactorAttr[] levelCharAttr = new LevelCharactorAttr[19];
	
	
	
	static public int totalCoins, totalSkillPoints;
	
	static public void SaveAllToPlayerPrefs(){
		// ToDo Save all to player prefs.
		
	}
	
	
	static public void initLevelAttr(){
		/*
		 * LevelCharactorAttr
		 *     float _attack, 
	     *     float _defend, 
	     *     float _jump, 
	     *     float _jumpDelta, 
	     *     int _skill, 
	     *     int _clothID, 
	     *     int _headID, 
	     *     int _monetBase, 
	     *     float _moneyDelta, 
	     *     int _pointBase, 
	     *     float _pointDelta,
	     *     int _AILevel,
	     *     float _proptime,
	     */
		levelCharAttr[0]  = new LevelCharactorAttr(0.90f, 0.90f, 1.60f, 0.30f, 05, 01, 01, 01, 0.3f, 02, 0.4f, 01, 6.0f);
		
		levelCharAttr[1]  = new LevelCharactorAttr(0.70f, 0.70f, 1.40f, 0.30f, 05, 01, 07, 01, 0.3f, 02, 0.4f, 01, 7.0f);
		levelCharAttr[2]  = new LevelCharactorAttr(0.80f, 0.80f, 1.50f, 0.35f, 05, 09, 00, 01, 0.3f, 02, 0.4f, 31, 7.0f);
		levelCharAttr[3]  = new LevelCharactorAttr(0.90f, 0.90f, 1.80f, 0.25f, 07, 03, 10, 02, 0.4f, 02, 0.5f, 02, 7.0f);
		levelCharAttr[4]  = new LevelCharactorAttr(1.15f, 0.90f, 2.00f, 0.30f, 07, 14, 09, 02, 0.5f, 03, 0.5f, 22, 6.5f);
		levelCharAttr[5]  = new LevelCharactorAttr(1.00f, 1.00f, 2.25f, 0.25f, 09, 15, 19, 03, 0.4f, 03, 0.6f, 02, 6.5f);
		levelCharAttr[6]  = new LevelCharactorAttr(0.80f, 1.10f, 2.10f, 0.40f, 10, 05, 18, 03, 0.5f, 05, 0.6f, 43, 6.5f);
		levelCharAttr[7]  = new LevelCharactorAttr(1.10f, 1.00f, 2.10f, 0.25f, 10, 03, 13, 04, 0.5f, 05, 0.6f, 03, 6.0f);
		levelCharAttr[8]  = new LevelCharactorAttr(1.10f, 1.25f, 2.00f, 0.15f, 11, 16, 08, 04, 0.6f, 07, 0.5f, 03, 6.0f);
		levelCharAttr[9]  = new LevelCharactorAttr(1.10f, 1.10f, 2.45f, 0.15f, 12, 13, 02, 05, 0.6f, 08, 0.6f, 33, 6.0f);
		levelCharAttr[10] = new LevelCharactorAttr(1.15f, 1.15f, 2.20f, 0.15f, 12, 01, 01, 05, 0.7f, 08, 0.7f, 04, 5.5f);
		levelCharAttr[11] = new LevelCharactorAttr(1.20f, 1.10f, 2.20f, 0.15f, 13, 03, 12, 06, 0.8f, 08, 0.7f, 04, 5.5f);
		levelCharAttr[12] = new LevelCharactorAttr(1.30f, 1.00f, 2.40f, 0.10f, 13, 13, 15, 06, 0.9f, 09, 0.8f, 34, 5.5f);
		levelCharAttr[13] = new LevelCharactorAttr(1.10f, 1.40f, 2.75f, 0.30f, 14, 05, 20, 07, 1.0f, 10, 0.9f, 04, 5.0f);
		levelCharAttr[14] = new LevelCharactorAttr(1.30f, 1.35f, 2.80f, 0.20f, 14, 13, 06, 07, 1.2f, 10, 1.0f, 04, 5.0f);
		levelCharAttr[15] = new LevelCharactorAttr(1.40f, 1.35f, 2.70f, 0.10f, 15, 09, 03, 08, 1.2f, 11, 1.1f, 05, 5.0f);
		levelCharAttr[16] = new LevelCharactorAttr(1.45f, 1.30f, 2.70f, 0.05f, 15, 03, 14, 10, 1.5f, 13, 1.4f, 25, 4.0f);
		levelCharAttr[17] = new LevelCharactorAttr(1.45f, 1.50f, 2.90f, 0.05f, 16, 03, 05, 10, 1.8f, 15, 2.0f, 05, 4.0f);
		levelCharAttr[18] = new LevelCharactorAttr(1.50f, 1.50f, 3.00f, 0.00f, 16, 01, 16, 10, 2.0f, 18, 2.5f, 05, 4.0f);
	
		levelCharAttr[0].AddPropFrequency(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 11, 11, 12, 13, 13, 13, 14, 14, 15, 15, 16, 17, 17, 18, 18);
		//levelCharAttr[1].AddPropFrequency(16,17,18);
		levelCharAttr[2].AddPropFrequency(0, 1, 2, 3, 3, 4, 4);
		levelCharAttr[3].AddPropFrequency(2, 3, 4, 8, 8, 9, 9);
		levelCharAttr[4].AddPropFrequency(8, 9, 5, 5, 6, 6);
		levelCharAttr[5].AddPropFrequency(14, 15, 10, 11, 16);
		levelCharAttr[6].AddPropFrequency(10, 11, 13, 13, 13);
		levelCharAttr[7].AddPropFrequency(0, 1, 2, 3, 4, 14, 14, 15, 15, 17);
		levelCharAttr[8].AddPropFrequency(7, 7, 12, 12, 5, 6, 2);
		levelCharAttr[9].AddPropFrequency(0, 1, 3, 4, 8, 9);
		levelCharAttr[10].AddPropFrequency(10, 11, 7, 12, 2, 14, 15, 16, 17);
		levelCharAttr[11].AddPropFrequency(3, 4, 5, 6, 8, 9, 13, 13, 13);
		levelCharAttr[12].AddPropFrequency(0, 1, 2, 10, 11, 14, 15, 18);
		levelCharAttr[13].AddPropFrequency(0, 1, 2, 3, 4, 5, 6, 7, 12, 16, 17, 18);
		levelCharAttr[14].AddPropFrequency(5, 6, 8, 9, 14, 15, 10, 11, 13, 13);
		levelCharAttr[15].AddPropFrequency(2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 17, 18);
		levelCharAttr[16].AddPropFrequency(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 13, 13, 13, 13, 14, 15, 16, 17, 18);
		levelCharAttr[17].AddPropFrequency(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 11, 11, 12, 13, 13, 13, 14, 15, 16, 17, 18);
		levelCharAttr[18].AddPropFrequency(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 11, 11, 12, 13, 13, 13, 14, 14, 15, 15, 16, 17, 18);
		
	}
	
	static public void LoadLevel(int LevelNum)
	{
		curLevelNum = LevelNum;
		Debug.Log("LevelNum");
		oppoFootballID = 0;
		oppoHeadID     = levelCharAttr[LevelNum].headID;
		oppoClothID    = levelCharAttr[LevelNum].clothID;
		oppoAttack     = levelCharAttr[LevelNum].attack;
		oppoDefend     = levelCharAttr[LevelNum].defend;
		oppoJump       = levelCharAttr[LevelNum].jump;
		oppoJumpDelta  = levelCharAttr[LevelNum].jumpDelta;
		oppoSkill      = levelCharAttr[LevelNum].skill;
		moneyBase      = levelCharAttr[LevelNum].moneyBase;
		moneyDelta     = levelCharAttr[LevelNum].moneyDelta;
		pointBase      = levelCharAttr[LevelNum].pointBase;
		pointDelta     = levelCharAttr[LevelNum].pointDelta;
		AILevel        = levelCharAttr[LevelNum].AILevel;
		props          = levelCharAttr[LevelNum].propFrequency;
		propTime       = levelCharAttr[LevelNum].propTime;
		
		if (LevelNum != 0)
			GlobalManager.LoadScene("GameSingle");
		else
			GlobalManager.LoadScene("GameNetwork");
	}
	
	static public void LoadScene(string sceneName)
	{
		GlobalManager.sceneToLoad = sceneName;
		Application.LoadLevel("LoadScene");
	}
	
	// Load scene without Loading screen
	static public void LoadSceneDirectly(string sceneName)
	{
		GlobalManager.sceneToLoad = sceneName;
		Application.LoadLevel(sceneName);
	}	
	
	
	/*
	 PlayerPrefs Document
	 
	 name             type         description
	 
	 SkillPoints      int          The skill point number that player have.
	 Coins            int          The coin number that player have.
	 SoundToogle      int          Controll Sound 1 means On, 0 means Off.
	 
	 HeadID           int          The head id now player use.
	 ClothID          int          The cloth id now player use.
	 FootballID       int          The football id now player use.
	 
	 Attack           float        The attack competence.
	 Defend           float        The defend competence.
	 Jump             float        The jump competence.
	 Skill            int          The Skill competence.
	 JumpDelta        float        The jump delta competence.
	 
	 AttackLevel      int          Attack competence level.
	 DefendLevel      int          Defend competence level.
	 JumpLevel        int          Jump competence level.
	 SkillLevel       int          Skill competence level.
	 
	 HeadLock         int          A binary system number, each digit means the head which numbers for the digit are lock or unlock, 1 means unlock, 0 means lock.
	 ClothLock        int          A binary system number, each digit means the cloth which numbers for the digit are lock or unlock, 1 means unlock, 0 means lock.
	 FootballLock     int          A binary system number, each digit means the football which numbers for the digit are lock or unlock, 1 means unlock, 0 means lock.
	 LevelLock        int          A binary system number, each digit means the Level which numbers for the digit are lock or unlock, 1 means unlock, 0 means lock.
	 
	*/
	
}
