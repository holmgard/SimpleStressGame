using UnityEngine;

public class GameLogEntry{
	public System.DateTime time;
	public int sessionNumber;
	public GameState gameState;
	public Vector3 playerPosition;
	public Vector3 targetPosition;
	public Vector3[] threatPositions;
	public int targetsCaught;
	public int threatsHit;
	public Vector3 mousePosition;
	
	public GameLogEntry(System.DateTime tm, int sesNum, GameState gSt, Vector3[] thrPos, Vector3 tarPos, Vector3 playPos, int tgtCaught, int thrHit, Vector3 mousePos)
	{
		time = tm;
		sessionNumber = sesNum;
		gameState = gSt;
		threatPositions = thrPos;
		targetPosition = tarPos;
		playerPosition = playPos;
		targetsCaught = tgtCaught;
		threatsHit = thrHit;
		mousePosition = mousePos;
	}
	
	public override string ToString ()
	{
		string threatString = "";
		for(int i = 0; i < threatPositions.Length; i++)
		{
			threatString += string.Format("{0}",threatPositions[i]);
			if(threatPositions.Length > 1 && i < threatPositions.Length-1)
			{
				threatString += ";";
			}
		}
		return string.Format("{0:yyy-mm-dd_hh:mm:ss.fff}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}",time,sessionNumber,gameState,threatString,targetPosition,playerPosition,targetsCaught,threatsHit,mousePosition);
	}
}