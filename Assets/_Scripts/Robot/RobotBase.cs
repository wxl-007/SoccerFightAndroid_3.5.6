using UnityEngine;
using System.Collections;

public abstract class RobotBase : MonoBehaviour {

	public GameObject effectPrefab;
	
	public abstract void FireEffect(Vector3 direction);
	
}
