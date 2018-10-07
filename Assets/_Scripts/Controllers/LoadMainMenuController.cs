using UnityEngine;
using System.Collections;

public class LoadMainMenuController : MonoBehaviour {

	public GameObject background;

	// Use this for initialization
	void Start () {
		StartCoroutine(LoadMain());
	}
	
	IEnumerator LoadMain()
	{
		yield return new WaitForSeconds(1.5f);
		background.active = false;
		GlobalManager.LoadSceneDirectly("MainMenu");
		// GlobalManager.LoadScene("MainMenu");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
