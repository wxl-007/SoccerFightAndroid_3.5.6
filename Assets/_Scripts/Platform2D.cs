using UnityEngine;
using System.Collections;

public class Platform2D : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	/*
	void  OnTriggerEnter(Collider other) {
		if (other.tag == "Player" && other.transform.position.y < transform.position.y) {
			collider.isTrigger = true;			
		}else{
			collider.isTrigger = false;				
		}		
	}
	*/

	
	void  OnTriggerExit(Collider other) {
		collider.isTrigger = false;		
		// Debug.Log("Trigger exit");
	}
	
}
