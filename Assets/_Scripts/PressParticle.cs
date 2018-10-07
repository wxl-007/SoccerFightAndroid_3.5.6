using UnityEngine;
using System.Collections;

public class PressParticle : MonoBehaviour {

	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnParticleCollision (GameObject other) {
		
		if (other.tag == "ball") {
			//GameController.totalHitCount ++;
		}
	}
}
