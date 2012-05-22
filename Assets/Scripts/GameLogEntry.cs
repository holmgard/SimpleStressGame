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
	
	public GameLogEntry(System.DateTime tm, int sesNum, GameState gSt, Vector3[] thrPos, Vector3 tarPos, Vector3 playPos, int tgtCaught, int thrHit)
	{
		time = tm;
		sessionNumber = sesNum;
		gameState = gSt;
		threatPositions = thrPos;
		targetPosition = tarPos;
		playerPosition = playPos;
		targetsCaught = tgtCaught;
		threatsHit = thrHit;
	}
	
	public override string ToString ()
	{
		return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}",time,gameState,threatPositions,targetPosition,playerPosition,targetsCaught,threatsHit);
	}
}