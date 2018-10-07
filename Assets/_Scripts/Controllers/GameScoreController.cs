using UnityEngine;
using System.Collections;

public class GameScoreController : MonoBehaviour {

    public static GameScoreController GSC;
    public static int score;	

    private int bestScore = 0;

    void Awake()
    {
        GSC = this;
        score = 0;
        bestScore = PlayerPrefs.GetInt("BestScorePlatforms", 0);
		
    }
	
	void Update(){
		if (score > bestScore)
        {            
			bestScore = score;
        }		
	}
	
    void OnGUI()
    {
        GUILayout.Space(3);
        GUILayout.Label(" Score: " + score);
        GUILayout.Label(" Highscore: " + bestScore);

        if (GameController.gameState == GameState.GameOver)
        {			
			
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
						
			/*
            if (score > bestScore)
            {
                GUI.color = Color.red;
                GUILayout.Label("New personal highscore!");
                GUI.color = Color.white;
            }
			*/
            if (GUILayout.Button("Try again", GUILayout.Width(120), GUILayout.Height(50)))
            {
				Application.LoadLevel("Game");
				
            }
			
			if (GUILayout.Button("Main Menu", GUILayout.Width(120), GUILayout.Height(50)))
            {
                Application.LoadLevel("MainMenu");
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

        }
    }

    public void WriteHighscore()
    {

        PlayerPrefs.SetInt("BestScorePlatforms", bestScore);		
    }
	
}
