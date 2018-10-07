using UnityEngine;
using System.Collections;

public class RobotFire : RobotBase {
	
	private float lastShootTime = 0.0f;
	private float shootInterval = 0.2f;
	
	private float effectLiveTime = 2.0f;
	
	private float shootPower = 30.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public override void FireEffect(Vector3 direction)
	{
		if (lastShootTime + shootInterval > Time.time) return;		
		lastShootTime = Time.time;
		
		Vector3 newPos = new Vector3(transform.position.x, transform.position.y+2, transform.position.z);
		GameObject bullet = Instantiate(effectPrefab, newPos, Quaternion.identity) as GameObject;
		Physics.IgnoreCollision(bullet.collider, transform.root.collider);
		
		Debug.Log("direction: " + direction);
		
		direction.x =shootPower * ( (direction.x>0)?1:-1);      // left or right
		direction.y = 0;
		direction.z = 0;
		
		bullet.rigidbody.velocity = transform.TransformDirection(direction);			
		//bullet.rigidbody.velocity = transform.TransformDirection(direction);			
		
		//audio.PlayOneShot(throwClip);
		
		Destroy(bullet, effectLiveTime);
	}
	
}
