using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour {
	
	public GameLogic gl;
	public GUISkin skin;
	
	// Use this for initialization
	void Start () {
		gl = (GameLogic) FindObjectOfType(typeof(GameLogic));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		switch(gl.gameState)
		{
		case GameState.menu:
			Screen.showCursor = true;
			MenuGUI();
			break;
		case GameState.playing:
			Screen.showCursor = false;
			PlayingGUI();
		break;
		case GameState.gameover:
			Screen.showCursor = true;
			GameOverGUI();
		break;
		default:
			break;
		}
	}
	
	public int menuWidth = 200;
	public int menuHeight = 200;
	
	void MenuGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width*0.5F-Screen.width*0.8F*0.5F,Screen.height*0.5F-Screen.height*0.8F*0.5F,Screen.width*0.8F,Screen.height*0.8F));
			GUILayout.BeginVertical();
				GUILayout.Label("Empatica state:");
				GUILayout.Box(gl.empConnection.empState.ToString());
				if(GUILayout.Button("Connect Empatica"))
				{
					gl.empConnection.ConnectEmpatica();
				}
				if(GUILayout.Button("Start the experiment"))
				{
					gl.SetGameState(GameState.initialSurvey);
				}
			GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	void PlayingGUI()
	{
		GUI.Box(new Rect(10,10,100,20),"Points: " + gl.points.ToString());
	}
	
	void InitialSurveyGUI(){
		
	}
	
	void PreferenceSurveyGUI(){
		
	}
	
	void GameOverGUI()
	{
		GUI.Box(new Rect(0,0,Screen.width,Screen.height),"");
		GUI.Box(new Rect(Screen.width/2-menuWidth/2,Screen.height/2-menuHeight/2,menuWidth,menuHeight),"GAME OVER :P");
		GUI.Box(new Rect(10,10,100,20),"Points: " + gl.points.ToString());
		if(GUI.Button(new Rect(Screen.width/2-menuWidth/2+50,Screen.height/2-menuHeight/2+50,100,100),"Restart Game"))
		{
			gl.SetGameState(GameState.initialSurvey);
		}
	}
}
