using UnityEngine;
using System.Collections;

public enum GameState{
		menu,playing,gameover
}

public class GameLogic : MonoBehaviour {
	
	private static GameLogic instance;
	public GameState gameState;
	public int points = 0;
	
	public Collider backGroundPlane;
	public float yForCharacters = 1;
	public Vector3[] startPoints;
	
	public GameObject threatPrefab;
	public GameObject targetPrefab;
	
	public static GameLogic Instance()
	{
		return instance;
	}
	
	public void SetGameState(GameState gs)
	{
		gameState = gs;
	}
	
	public void StartNewGame()
	{
		points = 0;
		int pos1 = Random.Range(0,3);
		int pos2 = Random.Range(0,3);
		while(pos1 == pos2)
		{
			pos2 = Random.Range(0,3);
		}
		Instantiate(targetPrefab,startPoints[pos1],Quaternion.identity);
		Instantiate(threatPrefab,startPoints[pos2],Quaternion.identity);
		
		this.SetGameState(GameState.playing);
	}
	
	public void GameOver()
	{
		Object[] targets = FindObjectsOfType(typeof(Target));
		Object[] threats = FindObjectsOfType(typeof(Threat));
		
		foreach(Target obj in targets)
		{
			Destroy(obj.gameObject);
		}
		
		foreach(Threat obj in threats)
		{
			Destroy(obj.gameObject);	
		}
		
		this.SetGameState(GameState.gameover);
	}
	
	public void SpawnThreat()
	{
		Instantiate(threatPrefab,startPoints[Random.Range(0,3)],Quaternion.identity);
	}
	
	// Use this for initialization
	void Start () {
		if(GameLogic.instance == null)
		{
			GameLogic.instance = this;
		}
		gameState = GameState.menu;
		
		startPoints = new Vector3[4];
		
		startPoints[0] = new Vector3(-4,1,3);
		startPoints[1] = new Vector3(4,1,3);
		startPoints[2] = new Vector3(4,1,-3);
		startPoints[3] = new Vector3(-4,1,-3);
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
	
	public void AddPoints()
	{
		points++;
	}
}
