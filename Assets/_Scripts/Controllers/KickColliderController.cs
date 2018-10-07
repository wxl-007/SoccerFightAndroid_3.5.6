/*
Maintaince Logs:

2011-11-02    Xu Mingzhao Initial version.  Controll Kick action.
2011-11-07    Xu Mingzhao change use collider to kick to use trigger to kick.
2011-12-15    XuMingzhao    Add Sound Controll.
*/


using UnityEngine;
using System.Collections;

public class KickColliderController : MonoBehaviour {

	public float kickPower = 5.0f;	
	public Controller2D controller;
	public bool isKicking;
	public GameObject kickEffect;
	private float lastKickTime;

	void OnTriggerStay (Collider other) 
	{
		if (!isKicking || Time.time - lastKickTime < 0.5f || other.rigidbody.isKinematic) return;
		lastKickTime = Time.time;
		if (controller.attackDirection == Direction.right)
			other.gameObject.SendMessage("hitPlayer", 0, SendMessageOptions.DontRequireReceiver);
		else
			other.gameObject.SendMessage("hitPlayer", 1, SendMessageOptions.DontRequireReceiver);
		Destroy(Instantiate(kickEffect,other.transform.position,Quaternion.identity),1f);
		kickPower = Random.Range( controller.competence.kickPower - controller.competence.kickPowerDelta, 
		                         controller.competence.kickPower + controller.competence.kickPowerDelta );
		
		Rigidbody body = other.rigidbody;
		
		Vector3 pushDir = body.transform.position - transform.position;
		if (body.transform.position.y < 0.6f)
		{
			pushDir.y = 0.45f;
		}
		pushDir /= pushDir.magnitude;
		if ( pushDir.y < -0.1f) pushDir.y = -0.1f;
		if ( pushDir.y > 0.45f) pushDir.y = 0.45f;
		SoundController.Instance.play_ball_kick();
		body.velocity = pushDir * kickPower * controller.movement.walkSpeed;
	}
	
}
