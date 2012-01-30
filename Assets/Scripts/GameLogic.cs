using UnityEngine;
using System.Collections;

public enum GameState{
		menu,playing,gameover
}

public class GameLogic : MonoBehaviour {
	
	private static GameLogic instance;
	public GameState gameState;
	
	public Collider backGroundPlane;
	public float yForCharacters = 1;
	
	public static GameLogic Instance()
	{
		return instance;
	}
	
	// Use this for initialization
	void Start () {
		if(GameLogic.instance == null)
		{
			GameLogic.instance = this;
		}
		
		gameState = GameState.playing;
	}
	
	// Update is called once per frame
	void Update () {
		switch(gameState)
		{
		case GameState.menu:
			break;
		case GameState.playing:
			break;
		case GameState.gameover:
			break;
		default:
			break;
		}
	}
}
