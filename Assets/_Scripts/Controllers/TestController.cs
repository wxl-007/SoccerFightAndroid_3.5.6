/*
Maintaince Logs:
2011-12-19    XuMingzhao    Add Debug Controller.
*/

using UnityEngine;
using System.Collections;

public class TestController : MonoBehaviour {
	
	void OnGUI () {
		
		if (Application.loadedLevelName == "TestController"){	
			if (GUI.Button(new Rect(100f, 100f, 100f, 50f), "GetMoney")){
				int coins = PlayerPrefs.GetInt("Coins",0);
				coins += 10000;
				PlayerPrefs.SetInt("Coins",coins);
			}
			if (GUI.Button(new Rect(250f, 100f, 100f, 50f), "GetPoints")){
				int points = PlayerPrefs.GetInt("SkillPoints",0);
				points += 500;
				PlayerPrefs.SetInt("SkillPoints",points);
			}
			if (GUI.Button(new Rect(400f, 100f, 100f, 50f), "UnlockLevel")){
				PlayerPrefs.SetInt("LevelLock",(1<<19) - 1);
			}
			if (GUI.Button(new Rect(100f, 250f, 100f, 50f), "ClearData")){
				PlayerPrefs.DeleteAll();
			}
			if (GUI.Button(new Rect(250f, 250f, 100f, 50f), "Quit")){
				GlobalManager.LoadScene("MainMenu");
			}
			PlayerPrefs.Save();
		}
		else{
			if (GUI.Button(new Rect(200f, 0f, 100f, 50f), "DebugControll")){
				GlobalManager.LoadScene("TestController");
			}
			
			if (GUI.Button(new Rect(200f, 100f, 100f, 50f), "StoreKitTestScene")){
				GlobalManager.LoadScene("StoreKitTestScene");
			}
		}
	}
}
