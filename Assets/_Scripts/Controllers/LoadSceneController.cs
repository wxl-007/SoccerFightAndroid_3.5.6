using UnityEngine;
using System.Collections;

public class LoadSceneController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(LoadScene());
	}
	
	IEnumerator LoadScene()
	{
		AsyncOperation asy=Application.LoadLevelAsync(GlobalManager.sceneToLoad);
		yield return asy;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
