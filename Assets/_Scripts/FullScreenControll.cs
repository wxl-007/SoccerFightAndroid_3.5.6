using UnityEngine;
using System.Collections;

public class FullScreenControll : MonoBehaviour {
	
	public UIButton btnFullScreen, btnWindow;
	// Use this for initialization
	void Start () {
		Screen.fullScreen = (PlayerPrefs.GetInt("isFullScreen",0) == 1);
		StartCoroutine(Refresh());
	}
	
	IEnumerator Refresh () {
		yield return 1;
		btnFullScreen.Hide(Screen.fullScreen);
		btnWindow.Hide(!Screen.fullScreen);
		PlayerPrefs.SetInt("isFullScreen", Screen.fullScreen ? 1 : 0);
		PlayerPrefs.Save();
	}
	
	void Toggle () {
		Screen.fullScreen = !Screen.fullScreen;
		StartCoroutine(Refresh());
	}
}
