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
	
	private Thread saveThread;
	
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
			Debug.Log("First if");
			if(target != null && threats != null && player != null)
			{
				Debug.Log("Second if");
				Vector3 targetPosition = target.transform.position;
				Vector3[] threatPositions = new Vector3[threats.Length];
				Vector3 playerPosition = player.transform.position;
			
				for(int i = 0; i < threats.Length; i++)
				{
					threatPositions[i] = threats[i].transform.position;
				}
				
				Debug.Log("GameLogger gamelogging!");
				
				gameLog.Add(new GameLogEntry(DateTime.Now, gameLogic.gamesPlayed, gameLogic.gameState, threatPositions, targetPosition, playerPosition, gameLogic.targetsCaugth, gameLogic.threatsHit));
			}
		}
	}
	
	public void SaveData()
	{
		saveThread = new Thread(new ThreadStart(ThreadSaveData));
		saveThread.IsBackground = true;
		saveThread.Start();
	}
	
	public void ThreadSaveData() //TODO: Maybe this could be rewritten to use LINQ and be really cool?
	{
		string fileName = "";
		fileName += DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString() + "gameLog.dat";
		
		string gameLogData = "";
		foreach(GameLogEntry gameLogEntry in gameLog)
		{
			gameLogData += gameLogData += gameLogEntry.ToString();
			gameLogData += "\n";
		}
		
		System.IO.File.WriteAllText(fileName,gameLogData);
		
		print ("GameLog saving done");
		
		saveThread.Abort();
	}
}
