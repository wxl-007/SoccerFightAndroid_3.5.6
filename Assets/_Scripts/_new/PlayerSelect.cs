using UnityEngine;
using System.Collections;

public class PlayerSelect : MonoBehaviour 
{
	public GameObject Player1;
	public GameObject Player2;
	void ChoosePlayer1()
	{
		
	}
	
	void ChoosePlayer2()
	{
		GameController.Instance.playerTrans = Player2.transform;
		Player2.AddComponent(typeof(Controller2D));
	}
	
	void OnGUI()
	{
		float yPos = 5.0f;
		float xPos = 100.0f;
		float height = 30.0f;
		float heightPlus = 35.0f;
		float width = 160.0f;
		if( GUI.Button( new Rect( xPos, yPos, width, height ), "Player0" ) )
		{
			GameController.Instance.SendMessage("SelectPlayer",0,SendMessageOptions.DontRequireReceiver);
			Destroy(gameObject);
		}
		if( GUI.Button( new Rect( xPos, yPos += heightPlus, width, height ), "Player1" ) )
		{
			GameController.Instance.SendMessage("SelectPlayer",1,SendMessageOptions.DontRequireReceiver);
			Destroy(gameObject);
		}
		//SelectPlayer
	}
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
