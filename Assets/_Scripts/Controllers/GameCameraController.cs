/*
Maintaince Logs:
2011-12-22    XuMingzhao    Initial version. controll game camera.
2011-12-23    XuMingzhao    Add Network support.
2011-12-28    XuMingzhao    Add football and player distance controll.
*/

using UnityEngine;
using System.Collections;

public class GameCameraController : MonoBehaviour {
	
	private GameObject camTarget;
	private GameObject myPlayer;
	private GameObject football;
	
	private Ray rayUp,rayLeft,rayRight;
	private RaycastHit hit;
	private Camera mainCamera;
	public Vector3 initPosition;
	public Vector3 farthestPosition;
	public float edgeWidth = 12f;
	public LayerMask RayMask;
	
	void Start ()
	{
		mainCamera = Camera.mainCamera;
	}
	
	void LateUpdate () 
	{
		myPlayer = GameController.Instance.myPlayer;
		football = GameController.Instance.footBall;
		ApplyCameraMove();
	}
	
	void CameraReset()
	{
		camTarget = GameController.Instance.footBall;
	}
	
	void ApplyCameraMove()
	{
		if (!myPlayer || !football) return;
		float camPosX = 0f, camPosY = 0f, camPosZ = 0f;
		float newPosX = 0f, newPosY = 0f, newPosZ = 0f;
		float mainViewWidth = 2f;
		float curUp = 0f, curLeft = 0f, curRight = 0f;
		
		rayUp = mainCamera.ScreenPointToRay(new Vector3(Screen.width /2, Screen.height, 0f));
		if (Physics.Raycast(rayUp, out hit, 1000f, RayMask))
			curUp = hit.point.y;
		
		if (camTarget.transform.position.x > mainViewWidth) camPosX = camTarget.transform.position.x - mainViewWidth;
		if (camTarget.transform.position.x < -mainViewWidth) camPosX = camTarget.transform.position.x + mainViewWidth;
		float t = (float) Screen.height / (float) Screen.width;
		float needY = 1.5f + Mathf.Abs(football.transform.position.x - myPlayer.transform.position.x) * t;
		needY = Mathf.Max(needY, football.transform.position.y);
		needY = Mathf.Max(needY, myPlayer.transform.position.y + 1f);
		float deltaY = needY - curUp + 1f;
		camPosY = mainCamera.transform.position.y + deltaY;
		camPosY = Mathf.Clamp(camPosY, initPosition.y, farthestPosition.y);
		float ratio = (camPosY - initPosition.y) / (farthestPosition.y - initPosition.y);
		camPosZ = Mathf.Lerp(initPosition.z, farthestPosition.z, ratio);
		
		//Update camera position if the player has climbed and if the player is too low: Set gameover.
		if (camPosX != 0)
        	newPosX = Mathf.Lerp(mainCamera.transform.position.x, camPosX, Time.deltaTime * 10);
		
		newPosY = Mathf.Lerp(mainCamera.transform.position.y, camPosY, Time.deltaTime * 10);
		newPosZ = Mathf.Lerp(mainCamera.transform.position.z, camPosZ, Time.deltaTime * 10);
		mainCamera.transform.position = new Vector3(newPosX, newPosY, newPosZ);
		
		rayLeft = mainCamera.ScreenPointToRay(new Vector3(0f, Screen.height, 0f));
		if (Physics.Raycast(rayLeft, out hit, 1000f, RayMask)){
			curLeft = hit.point.x;
		}
		rayRight = mainCamera.ScreenPointToRay(new Vector3(Screen.width, Screen.height, 0f));
		if (Physics.Raycast(rayRight, out hit, 1000f, RayMask)){
			curRight = hit.point.x;
		}
		if (curLeft < -edgeWidth){
			newPosX += -edgeWidth - curLeft;
		}
		if (curRight > edgeWidth){
			newPosX -= curRight - edgeWidth;
		}
		mainCamera.transform.position = new Vector3(newPosX, newPosY, newPosZ);
	}
}
