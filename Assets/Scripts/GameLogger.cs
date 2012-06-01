using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class GameLogger : MonoBehaviour {
	
	GameLogic gameLogic;
	
	List<GameLogEntry> gameLog;
	GameObject target;
	GameObject[] threats;
	GameObject player;
	
	public bool logging = false;
	
	private Thread saveGameLogThread;
	
	// Use this for initialization
	void Start ()
	{
		gameLogic = (GameLogic)GameObject.FindObjectOfType(typeof(GameLogic));
		gameLog = new List<GameLogEntry>();
	}
	
	public void ToggleLogging(){
		logging = !logging;
	}
	
	void FixedUpdate ()
	{
		//Debug.Log("Gameloggger Fixed update");
		target = GameObject.FindGameObjectWithTag("Target");
		threats = GameObject.FindGameObjectsWithTag("Threat");
		player = GameObject.FindGameObjectWithTag("Player");
		
		if(gameLogic.gameState == GameState.playingSesA || gameLogic.gameState == GameState.playingSesB)
		{
			//Debug.Log("First if");
			if(target != null && threats != null && player != null)
			{
				//Debug.Log("Second if");
				Vector3 targetPosition = target.transform.position;
				Vector3[] threatPositions = new Vector3[threats.Length];
				Vector3 playerPosition = player.transform.position;
			
				for(int i = 0; i < threats.Length; i++)
				{
					threatPositions[i] = threats[i].transform.position;
				}
				
				//Debug.Log("GameLogger gamelogging!");
				
				gameLog.Add(new GameLogEntry(DateTime.Now, gameLogic.gamesPlayed, gameLogic.gameState, threatPositions, targetPosition, playerPosition, gameLogic.targetsCaugth, gameLogic.threatsHit, Input.mousePosition));
				//Debug.Log(gameLog[gameLog.Count-1].ToString());
			}
		}
	}
	
	public void SaveData()
	{
		saveGameLogThread = new Thread(new ThreadStart(ThreadSaveGameLog));
		saveGameLogThread.IsBackground = true;
		saveGameLogThread.Start();
	}
	
	public void ThreadSaveGameLog() //TODO: Maybe this could be rewritten to use LINQ and be really cool?
	{
		print ("Saving gamelog...");
		string fileName = "";
				fileName += DateTime.Now.Day.ToString();
				fileName += DateTime.Now.Month.ToString();
				fileName += DateTime.Now.Year.ToString();
				fileName += "_";
				fileName += DateTime.Now.Hour.ToString();
				fileName += DateTime.Now.Minute.ToString();
				fileName += DateTime.Now.Second.ToString();
				fileName += "_GameLog";
				fileName += ".dat";
		
		string gameLogData = "";
		foreach(GameLogEntry gameLogEntry in gameLog)
		{
			//print ("Building gamelog output...");
			gameLogData += gameLogEntry.ToString();
			gameLogData += "\n";
		}
		//print ("Writing gamelog file...");
		System.IO.File.WriteAllText(fileName,gameLogData);
		
		print ("GameLog saving done");
		
		saveGameLogThread.Abort();
	}
}
