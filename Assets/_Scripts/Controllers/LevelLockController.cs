/*
Maintaince Logs: 
2011-11-29    XuMingzhao    Initial version.  controll level lock and unlock.
*/

using UnityEngine;
using System.Collections;

public class LevelLockController : MonoBehaviour {
	private UIButton Icon;
	public UIButton Lock;
	public MainMenuController MMC;
	public int levelNum;
	// Use this for initialization
	
	bool IsLevelLock()
	{
		int lockNum = PlayerPrefs.GetInt("LevelLock", 2);
		return ( ((1 << levelNum) & lockNum) == 0);
	}
	
	void Start () {
		Icon = GetComponent<UIButton>();
		Icon.SetValueChangedDelegate(OnTap);
		
	}
	
	void OnTap(IUIObject tmp)
	{
		GlobalManager.LoadLevel(levelNum);
	}
	
	// Update is called once per frame
	void Update () {
		Lock.gameObject.active = IsLevelLock();
	}
}
