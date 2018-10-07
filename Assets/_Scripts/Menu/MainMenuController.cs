/*
Maintaince Logs:
2011-11-08    XuMingzhao    Initial version.  Main menu controll.
2011-11-09    XuMingzhao    Controll All menu.
2011-11-10    XuMingzhao    Add competence support, lock and unlock support.
2011-11-27    XuMingzhao    Add skill point support, unlock window support.
2011-11-29    XuMingzhao    Delete loadlevel1-7 function.
2011-11-30    XuMingzhao    Add random unlock head.
2011-12-14    XuMingzhao    Add Skill point plus and cut controll.
2011-12-18    XuMingzhao    Add unlock tips controll.
*/

using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    
	public UIPanelManager panelManager, windowManager;
	public UIButton btnSureToUnlock;
	public SpriteText[] CoinTexts;
	public SpriteText[] PointTexts;
	public UIButton headLock, clothLock, footballLock;
	public UIButton headUnlock, clothUnlock, footballUnlock;
	public UIButton headUnlockBg, clothUnlockBg, footballUnlockBg;
	public UIStateToggleBtn SoundButton;
	public UIButton[] btns_play;
	public SpriteText unlockCostText, needPtText;
	public GameObject[] audioSource;
	
	public int[] clothPrice;
	public int[] headPrice;
	public int[] footballPrice;
	
	private bool isUnlockTipsShow;
	private int curCost;
	private string curLockName;
	
	#region Init / Add singleton
    private static MainMenuController instance;
    public static MainMenuController Instance
    {
        get
        {
            if (instance == null)
            {
                //instance = new GameController();
				instance = (MainMenuController)FindObjectOfType(typeof(MainMenuController)); 
            }
			if (!instance)
            {
                Debug.LogError("MainMenuController could not find itself!");
             } 

            return instance;
        }
    }
    #endregion 
	
	void Awake()
	{
		if (PlayerPrefs.GetInt("HeadLock", 0) == 0)
		{
			PlayerPrefs.SetInt("HeadLock", 0);
			PlayerPrefs.Save();
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		int t = PlayerPrefs.GetInt("SoundToogle", 1);
		if (t == 0){
			foreach (GameObject a in audioSource)
				a.transform.position = new Vector3(a.transform.position.x, 10000f, a.transform.position.z);
			SoundButton.SetToggleState("SoundOff");
		}
		GlobalManager.initLevelAttr();
		Time.timeScale = 1f;
	}
	
	void StartRandomUnlock()
	{
		StartCoroutine(RandomUnlockHead(2));
	}
	
	IEnumerator RandomUnlockHead(int times)
	{
		windowManager.BringIn("GrayPanel");
		for (int i=0; i<times; i++)
		{
			int lockNum = PlayerPrefs.GetInt("HeadLock", 0);
			int curID;
			do
			{
				curID = Random.Range(1, 21);
			}while( ((1 << curID) & lockNum) != 0);
			Debug.Log(curID);
			lockNum |= (1 << curID);
			PlayerPrefs.SetInt("HeadLock", lockNum);
			PlayerPrefs.Save();
			int lastID = PlayerPrefs.GetInt("HeadID", 0);
			float timedelta = 2f / ((curID - lastID + 20) % 20);
			do
			{
				yield return new WaitForSeconds(timedelta);
				AvatarController.Instance.HeadChangeRight();
			}while (IsLock("Head"));
			yield return new WaitForSeconds(0.9f);
		}
		windowManager.BringIn("UnlockTips2Panel");
	}
	
	public bool IsLock(string name)
	{
		int lockNum = PlayerPrefs.GetInt(name + "Lock", 1);
		int curID = PlayerPrefs.GetInt(name + "ID", 0);
		return ( ((1 << curID) & lockNum) == 0);
	}
	
	public void Unlock()
	{
		int curCoinNum = PlayerPrefs.GetInt("Coins");
		curCoinNum -= curCost;
		PlayerPrefs.SetInt("Coins",curCoinNum);
		PlayerPrefs.Save();
		
		name = curLockName;
		
		int lockNum = PlayerPrefs.GetInt(name + "Lock", 1);
		int curID = PlayerPrefs.GetInt(name + "ID", 0);
		lockNum |= (1 << curID);
		PlayerPrefs.SetInt(name + "Lock",lockNum);
		PlayerPrefs.Save();
		
		DismissWindow();
	}
	
	void UpdateAllObjects()
	{
		bool isHeadLock = IsLock("Head");
		bool isClothLock = IsLock("Cloth");
		bool isFootballLock = IsLock("Football");
		headLock.Hide(!isHeadLock);
		headUnlock.Hide(!isHeadLock);
		headUnlockBg.Hide(!isHeadLock);
		headUnlockBg.spriteText.Text 
			= headPrice[PlayerPrefs.GetInt("HeadID", 0)].ToString();
		clothLock.Hide(!isClothLock);
		clothUnlock.Hide(!isClothLock);
		clothUnlockBg.Hide(!isClothLock);
		clothUnlockBg.spriteText.Text 
			= clothPrice[PlayerPrefs.GetInt("ClothID", 0)].ToString();
		footballLock.Hide(!isFootballLock);
		footballUnlock.Hide(!isFootballLock);
		footballUnlockBg.Hide(!isFootballLock);
		footballUnlockBg.spriteText.Text 
			= footballPrice[PlayerPrefs.GetInt("FootballID", 0)].ToString();
		
		foreach (UIButton btn_play in btns_play)
			btn_play.Hide(isHeadLock || isClothLock || isFootballLock);
	}
	
	void UpdateText()
	{
		foreach (SpriteText Text in CoinTexts)
			Text.Text = PlayerPrefs.GetInt("Coins").ToString();
		
		foreach (SpriteText Text in PointTexts)
			Text.Text = PlayerPrefs.GetInt("SkillPoints").ToString();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (panelManager.CurrentPanel.name == "CharactorSelectPanel" 
		    && PlayerPrefs.GetInt("HeadLock", 0) == 0
		    && !isUnlockTipsShow)
		{
			isUnlockTipsShow = true;
			PlayerPrefs.SetInt("HeadID", 0);
			PlayerPrefs.Save();
			AvatarController.Instance.SendMessage("ChangeAll");
			windowManager.BringIn("UnlockTips1Panel");
		}
		
		UpdateText();
		UpdateAllObjects();
	}
	
	void GoToCharactorSelect()
	{
		windowManager.Dismiss();
		panelManager.BringIn("CharactorSelectPanel");
	}
	
	void BackToMainMenu()
	{
		panelManager.BringIn("MainPanel");
	}
	
	void SinglePlay()
	{
		GlobalManager.isSingleMode = true;
		GoToCharactorSelect();
	}
	
	void MutiPlay()
	{
		GlobalManager.isSingleMode = false;
		GoToCharactorSelect();
	}
	
	void PlayGame()
	{
		if (GlobalManager.isSingleMode)
		{
			panelManager.BringIn("LevelPanel");
		}
		else
		{
			panelManager.BringIn("MultiPlayerPanel");
		}
	}
	
	void DismissWindow()
	{
		windowManager.Dismiss();
	}
	
	void NetworkHelp()
	{
		windowManager.BringIn("NetworkHelpPanel");
	}
	
	public void TryToBuy(int cost, string name)
	{
		int curCoinNum = PlayerPrefs.GetInt("Coins", 0);
		curCost = cost;
		curLockName = name;
		if (curCoinNum >= cost)
		{
			unlockCostText.Text = cost.ToString();
			windowManager.BringIn("UnlockPanel");
		}
		else
			windowManager.BringIn("MoreMoneyPanel");
	}
	void ClothUnLock()
	{
		TryToBuy(clothPrice[PlayerPrefs.GetInt("ClothID", 0)], "Cloth");
	}
	
	void HeadUnLock()
	{
		TryToBuy(headPrice[PlayerPrefs.GetInt("HeadID", 0)], "Head");
	}
	
	void FootballUnLock()
	{
		TryToBuy(footballPrice[PlayerPrefs.GetInt("FootballID", 0)], "Football");
	}
	
	void SoundChange()
	{
		int t = PlayerPrefs.GetInt("SoundToogle", 1);
		t = 1-t;
		if (t == 1)
			foreach (GameObject a in audioSource)
				a.transform.position = new Vector3(a.transform.position.x, 0f, a.transform.position.z);
		if (t == 0)
			foreach (GameObject a in audioSource)
				a.transform.position = new Vector3(a.transform.position.x, 10000f, a.transform.position.z);

		PlayerPrefs.SetInt("SoundToogle", t);
		PlayerPrefs.Save();
	}
	
	
	public void BtnMoreGames(){
		Application.OpenURL ("http://app.108km.com/public/MoreGamesiOS.html");	
	}
	
	public void RateThisGame(){
		Application.OpenURL ("itms-apps://ax.itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=492082036");			
	}
	
	
	
	#region Easter eggs
	private int clickCreditsFootballTimes;
	void clickCreditsFootball()
	{
		if (++clickCreditsFootballTimes >= 2)
		{
			clickCreditsFootballTimes = 0;
			panelManager.initialPanel.transform.Rotate(0f, 0f, 180f);
			panelManager.CurrentPanel.transform.Rotate(0f, 0f, 180f);
		}
	}
	#endregion
}
