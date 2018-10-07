/*
Maintaince Logs:
2011-09-12    Waigo    Initial version.  Logic for the simple bullet.

*/

using UnityEngine;
using System.Collections;

public class EffectBulletSimple : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	void OnCollisionEnter(Collision collision){
		if (collision.collider.CompareTag("Enemy")){
			Debug.Log("Hit Enemy.");
			
		}
		Destroy(gameObject, 0);
	
	}
	
}
