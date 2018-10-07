/*
Maintaince Logs:
2010-11-22    Waigo       Initial version.  Attached to the Players. Can only push objects on pushLayers.
2011-10-21    Waigo       Added kick action. The if (Input.GetKey("space")) should be moved to InputController.cs
2011-10-25    Xu Mingzhao Now use InputController to controll kick action.
						  Add a judge: when player can't controll,can't kick also.
2011-10-28    Xu Mingzhao Add CharacterCompetence support.
2011-11-02    Xu Mingzhao Move kick controll to KickColliderController.
2011-11-03    Xu Mingzhao When football is below player push football down.
2011-11-07    Xu Mingzhao change use collider to kick to use trigger to kick.
*/


using UnityEngine;
using System.Collections;

public class ControllerPushBodies : MonoBehaviour {

	// Script added to a player for it to be able to push rigidbodies around.

	// How hard the player can push
	public float pushPower = 0.5f;
	public float kickPower = 5.0f;	
	public KickColliderController kickcollidercontroller;
	// Which layers the player can push
	// This is useful to make unpushable rigidbodies
	public LayerMask pushLayers = -1;

	// pointer to the player so we can get values from it quickly
	private Controller2D controller;
	private bool isKicking = false;

	
	void Start () 
	{
		controller = GetComponent (typeof(Controller2D)) as Controller2D;
	}

	void OnControllerColliderHit (ControllerColliderHit hit) 
	{
	
		pushPower = Random.Range( controller.competence.pushPower - controller.competence.pushPowerDelta, 
		                         controller.competence.pushPower + controller.competence.pushPowerDelta );
		Rigidbody body = hit.collider.attachedRigidbody;
		// no rigidbody
		if (body == null || body.isKinematic)
			return;
		
		// Only push rigidbodies in the right layers
		LayerMask bodyLayerMask = 1 << body.gameObject.layer;
		if ((bodyLayerMask & pushLayers.value) == 0)
			return;
		
		// Calculate push direction from move direction, we only push objects to the sides
		// never up and down
		Vector3 pushDir = new Vector3 (hit.moveDirection.x, 0.1f, hit.moveDirection.z);
		// We push objects below us to take objects to ground
		if (hit.moveDirection.y < -0.3)
		{
			pushDir = hit.transform.position - transform.position;
			pushDir.y -= 0.5f;
			body.velocity += pushDir * pushPower;
			return;
		}
		// push with move speed but never more than walkspeed
		body.velocity += pushDir * pushPower * Mathf.Min (controller.GetSpeed (), controller.movement.walkSpeed);
	
		
		//controller.SendMessage("DidAttack02", SendMessageOptions.DontRequireReceiver);
		//controller.SendMessage("DidAttack", SendMessageOptions.DontRequireReceiver);
	}
	
	IEnumerator DidAttack()
	{
		yield return new WaitForSeconds(0.2f);
		for (int i=1; i<=20; i++)
		{
			isKicking = true;
			yield return new WaitForSeconds(0.01f);
		}
	}
	
	void FixedUpdate()
	{
		kickcollidercontroller.isKicking = isKicking;
		isKicking = false;
	}
	
	void Update()
	{
		
	}

}
