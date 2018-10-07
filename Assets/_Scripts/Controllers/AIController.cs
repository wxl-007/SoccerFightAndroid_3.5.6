/*
Maintaince Logs:
2011-10-21    Xu Mingzhao    Add a simple AI , the AI is very stupid.
2011-10-26    Xu Mingzhao    Change AI to a conponent of player.
2011-10-27    Waigo              Redefine AI functions. 
2011-11-03    Xu Mingzhao    Make AI more powerful. Todo: More smothly.
2011-12-05    Xu Mingzhao    Add avoid be stept on.
                             Add skill controll.
2011-12-13    Xu Mingzhao    Update AI.
2011-12-16    Xu Mingzhao    Add AI Level Controll.
*/

using UnityEngine;
using System.Collections;

public class AIController : MonoBehaviour {

	public GameObject football;
	
	private Controller2D controller2D;
	
	private float moveLastTime = 0.0f;
	private float moveInterval = 0.0f;
	
	private float jumpLastTime = 0.0f;
	private float jumpInterval = 1.0f;
		
	private float shootLastTime = 0.0f;
	private float shootInterval = 0.0f;
	
	private float delay = 0f;
	private float posDeltaX;
	
	private Vector3 playerPos, footballPos, footballVec, newFootballPos;
	public Transform oppoPlayerTrans;
	private int alwaysJumpAIType=1,defendAIType=2,lazyAIType=3,GoalKeeperAIType=4;
	private int AIType;
	// Use this for initialization
	void Start () 
	{		
		controller2D =  GetComponent<Controller2D>();
		Debug.Log("AILevel:"+GlobalManager.AILevel);
		switch (GlobalManager.AILevel % 10){
		case 1:
			jumpInterval = 5f;
			shootInterval = 5f;
			delay = 0.5f;
			break;
		case 2:
			jumpInterval = 3.5f;
			shootInterval = 3.5f;
			delay = 0.4f;
			break;
		case 3:
			jumpInterval = 2.5f;
			shootInterval = 1.5f;
			delay = 0.3f;
			break;
		case 4:
			jumpInterval = 1.5f;
			shootInterval = 0.5f;
			delay = 0.2f;
			break;
		case 5:
			jumpInterval = 0f;
			shootInterval = 0f;
			delay = 0f;
			break;
		}
			
		AIType = GlobalManager.AILevel / 10;
	}
	
	// Update is called once per frame
	void Update () 
	{
		playerPos = transform.position;
		footballPos = football.transform.position;
		footballVec = football.rigidbody.velocity;
		posDeltaX = (GlobalManager.oppoAttack - 1f) * 0.1f;
		posDeltaX += footballVec.x * (0.15f);
		// simple forecast footballpos.
		newFootballPos = footballPos + footballVec * 0.4f;
		if (newFootballPos.y < 2.5f || newFootballPos.x > -8f && newFootballPos.x < 8f)
			newFootballPos -= - new Vector3(0f, 1.0f, 0f);
		if (newFootballPos.y < 0.3f) newFootballPos.y = 0.3f - newFootballPos.y;
		
		StartCoroutine(AIMove());
		StartCoroutine(AIJump());
		StartCoroutine(AIShoot());
		StartCoroutine(AISkill());
	}
	

	
	public IEnumerator AIMove()
	{
		if (Time.time - moveLastTime < moveInterval) yield break;
		
		int moveAmount = 0;
		if (footballPos.x < playerPos.x - 1.3f) moveAmount = -1; // can't touch ball.
		else 
		{
			if (footballPos.x < playerPos.x - 0.7f - posDeltaX) //powerful push distance.
			{
				if (footballPos.y > playerPos.y + 2.3f || footballPos.y < playerPos.y - 0.3f)
					moveAmount = 0;
				else if (footballPos.x > playerPos.x - 0.65f - posDeltaX)
						moveAmount = 0;
					else moveAmount = -1;
			}
			else if (footballPos.x < playerPos.x && footballPos.y > playerPos.y - 0.3f && footballPos.y < playerPos.y + 2.3f)
				moveAmount = -1; //if football is in front of AI and AI can touch it.
			else if (footballPos.x > playerPos.x - 0.65f - posDeltaX && footballPos.x < playerPos.x - 0.75f - posDeltaX)
					moveAmount = 0; // To avoid AI shake.
				else moveAmount = 1;
		}
		
		Vector3 posDelta = oppoPlayerTrans.position - playerPos;
		if (playerPos.x < -3f && Mathf.Abs(posDelta.x) < 1f && posDelta.y > 1f) moveAmount = 1;
		// Keep little distance from the players goal
		if (playerPos.x > -6.2f && playerPos.x < -5.8f && footballPos.x < -8.8f && moveAmount == -1) 
			moveAmount = 0;
		else if (playerPos.x < -6f && footballPos.x < -8.8f && moveAmount == -1) 
			moveAmount = 1;
		
		// don't go into goal
		if (playerPos.x < -8.8f && playerPos.x > -9f && footballPos.x < -8.7f)
			moveAmount = 0;
		else if (playerPos.x < -8.9f) 
			moveAmount = 1;
		
		if (AIType == defendAIType)
		{
			if (footballPos.x < 0.9f && footballVec.x > 1.5f)
				moveAmount = 1;
			else if (playerPos.x < 1f && playerPos.x > 0.8f && footballPos.x < 0.9f)
				moveAmount = 0;
			else if (playerPos.x < 0.9f) 
				moveAmount = 1;
		}
		
		if (playerPos.x > 8.8f && playerPos.x < 9f && footballPos.x > 8.7f) 
			moveAmount = 0;
		else if (playerPos.x > 8.9f) 
			moveAmount = -1;
		
		if (AIType == GoalKeeperAIType)
		{
			if (playerPos.x > 8.75f && playerPos.x < 9f) 
				moveAmount = 0;
			else if (playerPos.x > 8.9f) 
				moveAmount = -1;
			else if (playerPos.x < 8.9f)
				moveAmount = 1;
		}
		
		if (AIType == lazyAIType && footballPos.x < playerPos.x - 4f && moveAmount == -1 && football.rigidbody.velocity.magnitude > 1)
			moveAmount = 0;
		
		if (moveAmount != 0) moveLastTime = Time.time;
		controller2D.DoMoveAmount(moveAmount);
	}
		
	
	public IEnumerator AIJump()
	{
		if (AIType != alwaysJumpAIType) {
			if (Time.time - jumpLastTime < jumpInterval) yield break;
		}
		bool doJump = false;
		if (Mathf.Abs(footballPos.x - playerPos.x) < 3f 
		    || Mathf.Abs(newFootballPos.x - playerPos.x) < 3f
		    || Mathf.Abs(playerPos.x - oppoPlayerTrans.position.x) < 2f)
		if (newFootballPos.y > 3f && newFootballPos.y < 5f ) // if can push ball when jump.
		{	
			doJump = true;
		}
		if (playerPos.x < -5f && footballPos.x > playerPos.x - 3f && footballPos.x < playerPos.x) 
			// if near goal, AI should push ball lower.
		{
			if ( newFootballPos.y < 1.5f ) doJump = true;
			else doJump = false;
		}
		
		if (doJump || AIType == alwaysJumpAIType)
		{
			jumpLastTime = Time.time;
			yield return new WaitForSeconds(delay);
			controller2D.DoJump();
		}
	}
	
	
	public IEnumerator AIShoot()
	{
		if (Time.time - shootLastTime < shootInterval) yield break;
		bool doShoot = false;
		//can kick ball now.
		if ( footballPos.x < playerPos.x && footballPos.x > playerPos.x - 2f 
		    && footballPos.y < playerPos.y + 1.3f && footballPos.y > playerPos.y - 0.3f)
			doShoot = true;
		//can kick ball a little time later.
		if ( newFootballPos.x < playerPos.x && newFootballPos.x > playerPos.x - 2f 
		    && newFootballPos.y < playerPos.y + 1.3f && newFootballPos.y > playerPos.y - 0.3f)
			doShoot = true;
		if ( doShoot == true )
		{
			shootLastTime = Time.time;
			yield return new WaitForSeconds(delay);
			controller2D.DoShoot();
		}
	}
	
	public IEnumerator AISkill()
	{
		Vector3 dis = footballPos - playerPos - new Vector3(0f, 1f, 0f);
		if (GameController.Instance.energyNum[1] == 100
		    && dis.sqrMagnitude < 10)
		{
			yield return new WaitForSeconds(delay);
			GameController.Instance.UseSkill(1);
		}
		yield break;
	}
	
}
