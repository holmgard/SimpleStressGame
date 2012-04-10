using UnityEngine;
using System.Collections;

public class Target : GenericMover {
	
	public float safetyDistanceSpawning = 4.0F;
	
	public void Catch(Player player)
	{	
		Vector3[] spawnPoints = gl.startPoints;
		Vector3 spawnPoint;
		
		spawnPoint = spawnPoints[Random.Range(0,3)];
		Mathf.Clamp(safetyDistanceSpawning,0.0F,4.0F);
		while(Vector3.Distance(player.gameObject.transform.position,spawnPoint) < safetyDistanceSpawning)
		{
			spawnPoint = spawnPoints[Random.Range(0,3)];
		}
		
		Debug.Log(gameObject.name + " got caught");
		transform.position = spawnPoint;
	}
}
