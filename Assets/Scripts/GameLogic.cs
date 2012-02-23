using UnityEngine;
using System.Collections;

public enum GameState{
	menu,
	initialSurvey,
	preferenceSurvey,
	playing,
	gameover
}

public class GameLogic : MonoBehaviour {
	
	private static GameLogic instance;
	public GameState gameState;
	public int points = 0;
	
	public Collider backGroundPlane;
	public float yForCharacters = 1;
	public Vector3[] startPoints;
	
	public Player player;
	public GameObject threatPrefab;
	public GameObject targetPrefab;
	
	public float safetyDistanceSpawning = 4.0F;
	
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
	
	Vector3 spawnPointThreats;
	public void SpawnThreat()
	{
		Mathf.Clamp(safetyDistanceSpawning,0.0F,4.0F);
		
		spawnPointThreats = startPoints[Random.Range(0,3)];
		while(Vector3.Distance(player.gameObject.transform.position,spawnPointThreats) < safetyDistanceSpawning)
		{
			spawnPointThreats = startPoints[Random.Range(0,3)];
		}
		
		Debug.Log(Vector3.Distance(player.gameObject.transform.position,spawnPointThreats).ToString());
		Instantiate(threatPrefab,startPoints[Random.Range(0,3)],Quaternion.identity);
	}
	
	// Use this for initialization
	void Start () {
		if(GameLogic.instance == null)
		{
			GameLogic.instance = this;
		}
		gameState = GameState.menu;
		
		player = (Player)FindObjectOfType(typeof(Player));
		
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
		
		//for(int i = 0; i<startPoints.Length; i++)
		//	Debug.Log("SP: " + i + " " + (Vector3.Distance(player.gameObject.transform.position,spawnPoint) < safetyDistanceSpawning).ToString() + " " + Vector3.Distance(player.gameObject.transform.position,spawnPoint).ToString());
	}
	
	public void AddPoints()
	{
		points++;
	}
}
