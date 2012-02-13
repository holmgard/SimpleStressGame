using UnityEngine;
using System.Collections;

public class Target : GenericMover {

	public void Catch()
	{
		Debug.Log(gameObject.name + " got caught");
		transform.position = gl.startPoints[Random.Range(0,gl.startPoints.Length)];
		//targetState = Target.TargetStates.waiting;
	}
}
