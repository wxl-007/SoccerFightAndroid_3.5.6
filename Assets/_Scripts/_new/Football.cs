/*
Maintaince Logs:
2011-10-21    Waigo    Initial version.  Keep the football not going out the bounds.
2011-10-21    XuMingzhao Change levelBounds to public value.
			  			 Add Goal judge.
2011-11-01    XuMingzhao Add lastHitPlayerID and get prop controll.
2011-11-03    XuMingzhao Add throw the football to the sky function.
2011-12-15    XuMingzhao    Add Sound Controll.
*/


using UnityEngine;
using System.Collections;

public class Football : MonoBehaviour {
	
	public Rect levelBounds = new Rect(-6, -0.1f,12, 10);  // Same as level attribute's bounds.
	public Vector3 throwToSkyForce = new Vector3(0f, 15f, 0f);
	public float continuumHitIntervalTime = 0.1f;
	public int hitCountLimit = 3;
	public GameObject tail;
		
	private Vector3 fixedPos = Vector3.zero;
	private int lastHitPlayerID;
	private float timeLastHit;
	private int hitCount;
	private bool hasHitPlayer1,hasHitPlayer2;
	//public LayerMask kicklayer = -1;

	void FixedUpdate()
	{
		fixedPos = transform.position;

		if (transform.position.y < levelBounds.y) fixedPos.y =  levelBounds.y;
		if (transform.position.x < levelBounds.x) fixedPos.x =  levelBounds.x;
		if (transform.position.y > levelBounds.y + levelBounds.height) fixedPos.y =  levelBounds.y + levelBounds.height;
		if (transform.position.x > levelBounds.x + levelBounds.width) fixedPos.x =   levelBounds.x + levelBounds.width;
		fixedPos.z = 0;
		transform.position = fixedPos;
	}
	
	public void Reset(Vector3 initBallPosition)
	{
		// Debug.Log("footballreset");
		tail.active = false;
		rigidbody.isKinematic = true;
		transform.position = initBallPosition;
		
	}
	
	public void StartGame()
	{
		tail.active = true;
		rigidbody.isKinematic = false;
		int rand = Random.Range(-2,3);
		float force = 0;
		switch(rand)
		{
		case -2:
			force = Random.Range(-150f, -120f);
			break;
		case -1:
			force = Random.Range(-70f, -40f);
			break;
		case 0:
			force = Random.Range(-10f, 10f);
			break;
		case 1:
			force = Random.Range(40f, 70f);
			break;
		case 2:
			force = Random.Range(120f, 150f);
			break;
		}
		if (!GameController.Instance.IsNetWorking || Network.isServer)
			rigidbody.AddForce(force, Random.Range(250f,500f), 0);
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "_GoalLeft")
		{
			GameController.Instance.SendMessage("GetScore",0,SendMessageOptions.DontRequireReceiver);  //0 means left
		}
		if (other.tag == "_GoadRight")
		{
			GameController.Instance.SendMessage("GetScore",1,SendMessageOptions.DontRequireReceiver);  //1 means right
		}
		if (other.tag == "_Prop")
		{
			PropsController.Instance.GetProp(other.gameObject.name, lastHitPlayerID);
			if (GameController.Instance.IsNetWorking)
				networkView.RPC("NetworkDestroy", RPCMode.Others, other.gameObject.name, lastHitPlayerID, other.gameObject.transform.position);
			PropsController.Instance.DestroyPropIcon(other.gameObject.transform.position);
		}
	}
	
	[RPC]
	void NetworkDestroy(string name, int ID, Vector3 pos)
	{
		PropsController.Instance.GetProp(name, ID);
		PropsController.Instance.DestroyPropIcon(pos);
	}
	
	IEnumerator ThrowToSky()
	{
		collider.isTrigger = true;
		rigidbody.velocity = throwToSkyForce;
		yield return new WaitForSeconds(0.3f);
		collider.isTrigger = false;
	}
	
	public void hitPlayer(int playerNum)
	{
		GameController.Instance.GetEnergy(playerNum);
		lastHitPlayerID = playerNum;
	}
	
	void OnCollisionEnter (Collision collisionInfo)
	{
		if ( collisionInfo.gameObject.tag == "_Player0" ) 
		{
			SoundController.Instance.play_ball_heading();
			hitPlayer(0);
		}
		else if ( collisionInfo.gameObject.tag == "_Player1" ) 
		{
			SoundController.Instance.play_ball_heading();
			hitPlayer(1);
		}
		if ( collisionInfo.collider.tag == "_Boundary") rigidbody.velocity /= 2f;
	}
	
	void OnCollisionStay (Collision collisionInfo)
	{
		if ( collisionInfo.gameObject.tag == "_Player0" || collisionInfo.gameObject.tag == "_Player1" )
		{
			if ( collisionInfo.gameObject.tag == "_Player0" ) hasHitPlayer1 = true;
			if ( collisionInfo.gameObject.tag == "_Player1" ) hasHitPlayer2 = true;
			if ( Time.time - timeLastHit < continuumHitIntervalTime ) 
				hitCount++;
			else 
			{	
				hitCount=0;
				hasHitPlayer1 = hasHitPlayer2 = false;
			}
			timeLastHit = Time.time;
			if (hitCount >= hitCountLimit && hasHitPlayer1 && hasHitPlayer2) 
			{	
				StartCoroutine(ThrowToSky());
			}
		}
		else 
		{	
			hitCount = 0;
			hasHitPlayer1 = hasHitPlayer2 = false;
		}
	}
}
