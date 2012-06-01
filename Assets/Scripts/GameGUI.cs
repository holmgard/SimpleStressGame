using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour {
	
	public GameLogic gl;
	public GUISkin skin;
	public GUISkin proceedSkin;
	
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
		GUI.skin = skin;
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
		case GameState.finalPreferenceSurvey:
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
		GUI.Box(new Rect(0f,0f,1024f,768f),"",skin.GetStyle("BoxBackground"));
		GUILayout.BeginArea(new Rect(Screen.width*(1-menuWidth)*2,Screen.height*(1-menuHeight)*2,Screen.width*menuWidth,Screen.height*menuHeight),skin.box);
			GUILayout.BeginVertical();
				GUILayout.Label("Empatica state:",GUILayout.Width(200f));
				GUILayout.Label(gl.empConnection.empState.ToString(),GUILayout.Width(200f));
				switch(gl.empConnection.empState)
				{
					case EmpaticaState.fresh:
						GUILayout.Label("Enter Empatica port: ");
						gl.empConnection.serverPort = System.Convert.ToInt32(GUILayout.TextField(gl.empConnection.serverPort.ToString(),GUILayout.Width(50f)));
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
					case EmpaticaState.opening:
						if(GUILayout.Button("Connecting empatica",skin.GetStyle("button_yellow")))
						{
							//gl.empConnection.ConnectEmpatica();
						}
						GUILayout.Space(20f);
						if(GUILayout.Button("Wating for connection...",skin.GetStyle("button_yellow")))
						{
							gl.empConnection.MarkEvent("StartExperiment");
							gl.SetGameState(GameState.initialSurvey);
						}
						break;
					case EmpaticaState.receiving:
						if(GUILayout.Button("Empatica is connected",skin.GetStyle("button_green")))
						{
							//gl.empConnection.ConnectEmpatica();
						}
						GUILayout.Space(20f);
						if(GUILayout.Button("Start the experiment",skin.GetStyle("button_green")))
						{
							gl.empConnection.MarkEvent("StartExperiment");
							gl.SetGameState(GameState.initialSurvey);
						}
						break;
					case EmpaticaState.error:
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
		GUI.Box(new Rect(10,10,50,20),"Points: " + (gl.targetsCaugth - gl.threatsHit).ToString());
	}
	
	void WaitingGUI()
	{
		GUI.Box(new Rect(0f,0f,1024f,768f),"",skin.GetStyle("BoxBackground"));
		GUI.skin = proceedSkin;
		GUILayout.BeginArea(new Rect(Screen.width*0.5F-Screen.width*0.3F*0.5F,Screen.height*0.5F-Screen.height*0.3F*0.5F,Screen.width*0.3F,Screen.height*0.3F));
			GUILayout.BeginHorizontal();
				GUILayout.BeginVertical();
					if(GUILayout.Button("Proceed to next round"))
					{
						gl.empConnection.MarkEvent("SessionStart");
						gl.StartNewGame();
					}
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	public float gameOverMenuWidth = 0.3155F;
	public float gameOverMenuHeight = 0.3F;
	void GameOverGUI()
	{
		GUI.Box(new Rect(0f,0f,1024f,768f),"",skin.GetStyle("BoxBackground"));
		GUI.skin = skin;
		//GUILayout.BeginArea(new Rect(Screen.width*0.5F-Screen.width*0.8F*0.5F,Screen.height*0.5F-Screen.height*0.8F*0.5F,Screen.width*0.8F,Screen.height*0.8F));
		GUILayout.BeginArea(new Rect(Screen.width*(1-gameOverMenuWidth)/2,Screen.height*(1-gameOverMenuHeight)/2,Screen.width*gameOverMenuWidth,Screen.height*gameOverMenuHeight),skin.box);
			GUILayout.BeginVertical();
				GUILayout.Label("Empatica state:");
				GUILayout.Label(gl.empConnection.empState.ToString());
				if(GUILayout.Button("Stop reading"))
				{
					gl.empConnection.DisconnectEmpatica();
				}
				if(GUILayout.Button("SaveData"))
				{
					gl.gameLogger.SaveData();
					gl.surveyLogic.SaveData();
					gl.empConnection.SaveData();
					gl.didManuallySave = true;
				}
			GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
