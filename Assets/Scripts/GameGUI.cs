using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour {
	
	public GameLogic gl;
	public GUISkin skin;
	
	// Use this for initialization
	void Start ()
	{
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
	
	public float menuWidth = 0.8F;
	public float menuHeight = 0.8F;
	
	void MenuGUI()
	{
		GUI.skin = skin;
		GUILayout.BeginArea(new Rect(Screen.width*(1-menuWidth)*2,Screen.height*(1-menuHeight)*2,Screen.width*menuWidth,Screen.height*menuHeight));
			GUILayout.BeginVertical();
				GUILayout.Label("Empatica state:");
				GUILayout.Box(gl.empConnection.empState.ToString());
				switch(gl.empConnection.empState)
				{
					case EmpaticaConnection.EmpaticaState.fresh:
						if(GUILayout.Button("Connect Empatica",skin.GetStyle("button_greyish")))
						{
							gl.empConnection.ConnectEmpatica();
						}
						GUILayout.Space(20f);
						if(GUILayout.Button("Start the experiment",skin.GetStyle("button_greyish")))
						{
							gl.empConnection.MarkEvent("StartExperiment");
							gl.SetGameState(GameState.initialSurvey);
						}
						break;
					case EmpaticaConnection.EmpaticaState.opening:
						if(GUILayout.Button("Connect Empatica",skin.GetStyle("button_yellow")))
						{
							//gl.empConnection.ConnectEmpatica();
						}
						GUILayout.Space(20f);
						if(GUILayout.Button("Start the experiment",skin.GetStyle("button_yellow")))
						{
							gl.empConnection.MarkEvent("StartExperiment");
							gl.SetGameState(GameState.initialSurvey);
						}
						break;
					case EmpaticaConnection.EmpaticaState.receiving:
						if(GUILayout.Button("Connect Empatica",skin.GetStyle("button_green")))
						{
							//gl.empConnection.ConnectEmpatica();
						}
						if(GUILayout.Button("Start the experiment",skin.GetStyle("button_green")))
						{
							gl.empConnection.MarkEvent("StartExperiment");
							gl.SetGameState(GameState.initialSurvey);
						}
						break;
					case EmpaticaConnection.EmpaticaState.error:
						if(GUILayout.Button("Connect Empatica",skin.GetStyle("button_red")))
						{
							//gl.empConnection.ConnectEmpatica();
						}
						if(GUILayout.Button("Start the experiment",skin.GetStyle("button_red")))
						{
							gl.empConnection.MarkEvent("StartExperiment");
							gl.SetGameState(GameState.initialSurvey);
						}
						break;
					default:
						break;
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
					gl.gameLogger.SaveData();
					gl.surveyLogic.SaveData();
					gl.empConnection.StartSave();
					gl.didManuallySave = true;
				}
			GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
