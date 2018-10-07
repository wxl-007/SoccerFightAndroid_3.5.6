using UnityEngine;
using System.Collections;

public class GlobalScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		float xPos = 50;
    	if (GUI.Button(new Rect(xPos, 10, 48, 48), "Load")){
			Debug.Log(Application.loadedLevelName);

    		if (!Application.isLoadingLevel) {
				Application.LoadLevel("Game");
			}
    	}	
	}
	
	
}
