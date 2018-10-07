/*
Maintaince Logs:
2011-11-30    Xu Mingzhao    Initial version. Controll rogue AI.
2011-12-26    Xu Mingzhao    Add Network support.
*/
using UnityEngine;
using System.Collections;

public class RogueAI : MonoBehaviour {

	public GameObject football;
	
	private Controller2D controller2D;
	
	private float moveLastTime = 0.0f;
	private float moveInterval = 0.2f;
	
	private float jumpLastTime = 0.0f;
	private float jumpInterval = 0.0f;
		
	private float shootLastTime = 0.0f;
	private float shootInterval = 0.0f;
	
	private Vector3 playerPos, footballPos, footballVec, newFootballPos;
	
	void Awake ()
	{
		if (GameController.Instance.IsNetWorking && !Network.isServer)
			enabled = false;
	}
	
	// Use this for initialization
	void Start () 
	{
		controller2D =  GetComponent<Controller2D>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		playerPos = transform.position;
		footballPos = football.transform.position;
		footballVec = football.rigidbody.velocity;
		
		// simple forecast footballpos.
		newFootballPos = footballPos + footballVec * 0.4f;
		if (newFootballPos.y < 2.5f || newFootballPos.x > -8f && newFootballPos.x < 8f)
			newFootballPos -= - new Vector3(0f, 1.0f, 0f);
		if (newFootballPos.y < 0.3f) newFootballPos.y = 0.3f - newFootballPos.y;
		
		AIMove();
		AIJump();
		AIShoot();
	}
	

	
	public void AIMove()
	{
		if (Time.time - moveLastTime < moveInterval) return;
		
		int moveAmount = 0;
		
		if (footballPos.x < playerPos.x - 0.5f) moveAmount = -1;
		if (footballPos.x > playerPos.x + 0.5f) moveAmount = 1;
		if (Mathf.Abs(footballPos.x - playerPos.x) < 2f)
		if (Random.Range(0f,1f) < 0.5f)
			moveAmount = Random.Range(-1,2);
		
		if (moveAmount != 0) moveLastTime = Time.time;
		controller2D.DoMoveAmount(moveAmount);
	}
		
	
	public void AIJump()
	{
		if (Time.time - jumpLastTime < jumpInterval) return;
		bool doJump = true;
		
		if (doJump)
		{
			jumpLastTime = Time.time;
			controller2D.DoJump();
		}
	}
	
	
	public void AIShoot()
	{
		if (Time.time - shootLastTime < shootInterval) return;
		bool doShoot = false;

		if ( doShoot == true )
		{
			Debug.Log("AIShoot");
			controller2D.DoShoot();
			shootLastTime = Time.time;
		}
		return;
	}	
}
