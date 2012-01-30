using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour {
	
	public GameLogic gl;
	public GUISkin skin;
	
	// Use this for initialization
	void Start () {
		gl = GameLogic.Instance();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		switch(gl.gameState)
		{
		case GameState.menu:
			MenuGUI();
			break;
		case GameState.playing:
			Screen.showCursor = false;
			PlayingGUI();
		break;
		case GameState.gameover:
			GameOverGUI();
		break;
		default:
			break;
		}
	}
	
	public int menuWidth;
	public int menuHeight;
	void MenuGUI()
	{
		
	}
	
	void PlayingGUI()
	{
		
	}
	
	void GameOverGUI()
	{
		
	}
}
