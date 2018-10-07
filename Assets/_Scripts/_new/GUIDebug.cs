using UnityEngine;
using System.Collections;

public class GUIDebug : MonoBehaviour {

	static string debugInfo;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public static void Log(string info){
		debugInfo = info + "\n" + debugInfo;
	}
	
	void OnGUI(){
		debugInfo = GUI.TextArea(new Rect(Screen.width/2 - 100, 10, 200, 100), debugInfo, 200);
	}
	
	
}
