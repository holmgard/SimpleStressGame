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
		case GameState.playingSesA:
			Screen.showCursor = false;
			PlayingGUI();
		break;
		case GameState.waiting:
			Screen.showCursor = true;
			WaitingGUI();
		break;
		case GameState.playingSesB:
			Screen.showCursor = false;
			PlayingGUI();
		break;
		case GameState.preferenceSurvey:
			Screen.showCursor = true;
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
					gl.empConnection.MarkEvent("StarExperiment");
					gl.SetGameState(GameState.initialSurvey);
				}
			GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	void PlayingGUI()
	{
		GUI.Box(new Rect(10,10,100,20),"Points: " + gl.targetsCaugth.ToString());
	}
	
	void WaitingGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width*0.5F-Screen.width*0.8F*0.5F,Screen.height*0.5F-Screen.height*0.8F*0.5F,Screen.width*0.8F,Screen.height*0.8F));
			GUILayout.BeginVertical();
				if(GUILayout.Button("Proceed to next round"))
				{
					gl.empConnection.MarkEvent("SessionStart");
					gl.StartNewGame();
				}
			GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	void GameOverGUI()
	{
		GUILayout.BeginArea(new Rect(Screen.width*0.5F-Screen.width*0.8F*0.5F,Screen.height*0.5F-Screen.height*0.8F*0.5F,Screen.width*0.8F,Screen.height*0.8F));
			GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();	
					GUILayout.Label("Empatica state:");
					GUILayout.Box(gl.empConnection.empState.ToString());
				GUILayout.EndHorizontal();
				if(GUILayout.Button("Stop reading"))
				{
					gl.empConnection.StopEmpatica();
				}
				if(GUILayout.Button("SaveData"))
				{
					gl.empConnection.StartSave();
				}
			GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
