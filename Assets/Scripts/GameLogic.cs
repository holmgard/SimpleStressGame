using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState{
	menu,
	initialSurvey,
	playingSesA,
	waiting,
	playingSesB,
	preferenceSurvey,
	gameover
}

public class GameLogic : MonoBehaviour {
	
	public ExperimentalTrial expTrial;
	public SurveyLogic surveyLogic;
	public EmpaticaConnection empConnection;
	public GameConfigurator gameConfig;
	
	private static GameLogic instance;
	public GameState gameState;
	public int targetsCaugth = 0;
	public int threatsHit = 0;
	public int maxNumberOfThreats = 6;
	
	public int gamesPlayed = 0;
	
	public Collider backGroundPlane;
	public float yForCharacters = 1;
	public Vector3[] startPoints;
	
	public Player player;
	public GameObject threatPrefab;
	public GameObject targetPrefab;
	
	public GameObject currentTarget;
	public List<GameObject> currentThreats;
	
	public Stimulus currentStimulus;
	public GameObject stimulusDeliveryPlane;
	
	private float gameStartTime;
	public float gameLengthInSeconds = 30.0f;
	public int numberOfThreatsOnScene = 0;
	public float stimulusDeliveryPointRelative = 0.5f;
	private float stimulusDeliveryTime;
	public bool stimulusDelivered = false;
	public float stimulusExposureTimeInSeconds = 1.0f;
	public bool stimulusRemoved = false;
	
	public float safetyDistanceSpawning = 4.0F;
	
	public GameLogger gameLogger;
	public bool didManuallySave = false;
	
	public AudioSource threatCollisionSource;
	
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
		gameStartTime = Time.time;
		stimulusDelivered = false;
		stimulusRemoved = false;
		
		if(threatCollisionSource.isPlaying)
		{
			threatCollisionSource.Stop();
		}
		
		targetsCaugth = 0;
		int pos1 = Random.Range(0,3);
		int pos2 = Random.Range(0,3);
		while(pos1 == pos2)
		{
			pos2 = Random.Range(0,3);
		}
		currentTarget = (GameObject)Instantiate(targetPrefab,startPoints[pos1],Quaternion.identity);
		currentThreats.Add((GameObject)Instantiate(threatPrefab,startPoints[pos2],Quaternion.identity));
		numberOfThreatsOnScene = currentThreats.Count;
		
		gameConfig.ConfigureSession(expTrial.GetNextExpSession());
		if(gameState == GameState.initialSurvey || gameState == GameState.preferenceSurvey)
			this.SetGameState(GameState.playingSesA);
		else if (gameState == GameState.waiting)
			this.SetGameState(GameState.playingSesB);
	}
	
	public void ThreatCollision()
	{
		threatCollisionSource.PlayOneShot(threatCollisionSource.clip);
		threatsHit++;
	}
	
	public void GameOver()
	{
		gamesPlayed++;
		Debug.Log("Games Played: " + gamesPlayed + "Games remaining: " + (expTrial.totalSets.Count-gamesPlayed).ToString());
		
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
		
		if(gamesPlayed >= expTrial.totalSets.Count)
			this.SetGameState(GameState.gameover);
		if(gameState == GameState.playingSesA)
		{
			this.SetGameState(GameState.waiting);
		}
		else if (gameState == GameState.playingSesB)
		{
			surveyLogic.StartPreferenceSurvey();
			this.SetGameState(GameState.preferenceSurvey);
		}
		
		currentTarget = null;
		currentThreats.Clear();
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
		
		//Debug.Log(Vector3.Distance(player.gameObject.transform.position,spawnPointThreats).ToString());
		currentThreats.Add((GameObject) Instantiate(threatPrefab,startPoints[Random.Range(0,3)],Quaternion.identity) as GameObject);
		numberOfThreatsOnScene = currentThreats.Count;
	}
	
	// Use this for initialization
	void Start () {
		
		Debug.Log("DataPath: " + Application.dataPath);
		//System.Diagnostics.Process.Start(Application.dataPath + "/AffectiveLnk_bridge");
		
		stimulusDeliveryPlane.active = false;
		//stimulusDeliveryPlane.transform.position = new Vector3(stimulusDeliveryPlane.transform.position.x, 1000, stimulusDeliveryPlane.transform.position.z);
		
		threatCollisionSource = Camera.mainCamera.GetComponent<AudioSource>();
		
		if(GameLogic.instance == null)
		{
			GameLogic.instance = this;
		}
		gameState = GameState.menu;
		
		gameLogger = gameObject.AddComponent<GameLogger>();
		
		Debug.Log("Toggling GameLogger");
		gameLogger.ToggleLogging();
		
		player = (Player)FindObjectOfType(typeof(Player));
		expTrial = (ExperimentalTrial)FindObjectOfType(typeof(ExperimentalTrial));
		surveyLogic = (SurveyLogic)FindObjectOfType(typeof(SurveyLogic));
		empConnection = (EmpaticaConnection)FindObjectOfType(typeof(EmpaticaConnection));
		gameConfig = (GameConfigurator)FindObjectOfType(typeof(GameConfigurator));
		
		startPoints = new Vector3[4];
		
		startPoints[0] = new Vector3(-4,1,3);
		startPoints[1] = new Vector3(4,1,3);
		startPoints[2] = new Vector3(4,1,-3);
		startPoints[3] = new Vector3(-4,1,-3);
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch(gameState)
		{
		case GameState.menu:
			break;
		case GameState.initialSurvey:
			break;
		case GameState.playingSesA:
			break;
		case GameState.playingSesB:
			break;
		case GameState.preferenceSurvey:
			break;
		case GameState.gameover:
			break;
		default:
			break;
		}
		
		if( gameState == GameState.playingSesA || gameState == GameState.playingSesB )
		{
			CheckForStimulusDelivery();
		}
		
		if( ( gameState == GameState.playingSesA || gameState == GameState.playingSesB ) && Time.time - gameStartTime > gameLengthInSeconds)
		{
			GameOver();
		}
		
		Vector3[] threatPositions = new Vector3[currentThreats.Count];
		for(int i = 0; i < currentThreats.Count; i++)
		{
			threatPositions[i] = currentThreats[i].transform.position;
		}
		
		/*if(gameState == GameState.playingSesA || gameState == GameState.playingSesB)
			gameLog.Add(new GameLogEntry(System.DateTime.Now,gamesPlayed,gameState,threatPositions,currentTarget.transform.position,player.transform.position,targetsCaugth));*/
		/*if(gameLog != null)
			Debug.Log("GameLog length: " + gameLog.Count.ToString());*/
	}
	
	public void CheckForStimulusDelivery()
	{
		//Debug.Log("Time to stimulus delivery: " + ( (Time.time - gameStartTime - gameLengthInSeconds*stimulusDeliveryPointRelative)*-1 ).ToString() );
		//Debug.Log("Stimulus type: " + currentStimulus.stimType.ToString());
		if(stimulusDelivered && stimulusRemoved)
		{
			return;
		} else if ( stimulusDelivered && !stimulusRemoved ) {
			if( Time.time - stimulusDeliveryTime > stimulusExposureTimeInSeconds )
			{
				Debug.Log("Removing visual stimulus");
				stimulusDeliveryPlane.active = false;
				stimulusRemoved = true;	
			}
		} else if ( ( Time.time - gameStartTime ) > ( gameLengthInSeconds*stimulusDeliveryPointRelative )  ) 
		{
			stimulusDeliveryTime = Time.time;
				
			if(currentStimulus.stimType == Stimulus.StimulusTypes.auditive)
			{
				//TODO - put mark in data set
				Debug.Log("Deploying auditive stimulus");
				threatCollisionSource.PlayOneShot(currentStimulus.audio);
				stimulusRemoved = true;
			}
			if(currentStimulus.stimType == Stimulus.StimulusTypes.visual)
			{
				//TODO - put mark in data set
				Debug.Log("Deploying visual stimulus");
				stimulusDeliveryPlane.renderer.material.mainTexture = currentStimulus.image;
				stimulusDeliveryPlane.active = true;
				//Change background
			}
			
			stimulusDelivered = true;
		}
	}
	
	public void AddPoints()
	{
		threatCollisionSource.PlayOneShot(player.getPointsSound);
		targetsCaugth++;
	}
	
	public void OnApplicationQuit()
	{
		/*if(!didManuallySave)
		{
			gameLogger.SaveData();
			surveyLogic.SaveData();
			empConnection.StartSave();
		}*/
	}
}