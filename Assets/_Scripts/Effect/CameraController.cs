/*
Maintaince Logs:
2010-10-10    Waigo    Initial version. To Handle the camera animation on the beginning or ending.

*/


using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	public float radius;
	private Vector3[] pathArr = new Vector3[201];
	private Vector3[] tmpArr;
	private int arrLen;
	
	void Start(){
		float delta = radius * 0.03f;
		int i=0;
		for (float x = 0f; x>-radius + 0.01f; x-=delta){
			float y = radius - Mathf.Sqrt(radius*radius - x*x);
			pathArr[i++] = new Vector3(x, 0f, y);
		}
		for (float x = -radius; x < -0.01f; x+=delta){
			float y = radius + Mathf.Sqrt(radius*radius - x*x);
			pathArr[i++] = new Vector3(x, 0f, y);
		}
		for (float x = 0f; x < radius - 0.01f; x+=delta){
			float y = radius + Mathf.Sqrt(radius*radius - x*x);
			pathArr[i++] = new Vector3(x, 0f, y);
		}
		for (float x = radius; x > 0.01f; x-=delta){
			float y = radius - Mathf.Sqrt(radius*radius - x*x);
			pathArr[i++] = new Vector3(x, 0f, y);
		}
		arrLen = i;
	}
	
	void PlayAnimateAround(Vector3 PlayerPos) {
		transform.position = PlayerPos + new Vector3(0f, 3f, -5f);
		tmpArr = new Vector3[arrLen];
		for (int i=0; i<arrLen; i++){
			tmpArr[i] = pathArr[i] + transform.position;
		}
		iTween.MoveTo(gameObject, iTween.Hash(
			"path", tmpArr, 
		    "looktarget", PlayerPos, 
		    "easetype", iTween.EaseType.linear, 
		    "looktime", 0.04f, 
		    "time", 4f, 
	        "ignoretimescale", true, 
		    "oncomplete", "ResumeTimeScale",
		    "movetopath", false
		));	
	}
	
	void PlayAnimateQuick(Vector3 PlayerPos) {
		transform.position = PlayerPos + new Vector3(0f, 3f, -5f);
		tmpArr = new Vector3[arrLen];
		for (int i=0; i<arrLen; i++){
			tmpArr[i] = pathArr[i] + transform.position;
		}
		iTween.MoveTo(gameObject, iTween.Hash(
			"path", tmpArr, 
		    "looktarget", PlayerPos, 
		    "easetype", iTween.EaseType.linear, 
		    "looktime", 0.04f, 
		    "time", 1.5f, 
	        "ignoretimescale", true, 
		    "oncomplete", "ResumeTimeScale",
		    "movetopath", false
		));	
	}
	
	void ResumeTimeScale()
	{
		Time.timeScale = 1f;
	}
}
