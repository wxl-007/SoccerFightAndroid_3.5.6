/*
Maintaince Logs:
2011-11-30    XuMingzhao    Controller Camera Size.
*/

using UnityEngine;
using System.Collections;

public class GUICameraController : MonoBehaviour {

	// Use this for initialization
	
	void Awake () {
		Camera cam = GetComponent<Camera>();
		float ration = (float) Screen.width / (float) Screen.height;
		if (Mathf.Abs(ration - 1.5f) < 0.01f)
			cam.orthographicSize = 320;
		else
			cam.orthographicSize = 384;
	}
	
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
