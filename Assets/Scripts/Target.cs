using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour {
	
	public enum TargetStates
	{
		waiting,active
	}
	
	public Vector3 origin;
	public TargetStates targetState;
	
	public float degreesToRotate = 1f;
	public float movementSpeed = 1;
	
	// Use this for initialization
	void Start ()
	{
		origin = transform.position;
		targetState = Target.TargetStates.waiting;
	}
	
	// Update is called once per frame
	void Update ()
	{
		MoveSpiral();
	}
	
	public void Catch()
	{
		Debug.Log(gameObject.name + " got caught");
		transform.position = origin;
		targetState = Target.TargetStates.waiting;
	}
	
	void MoveSpiral(){
		CheckBoundaries();
		transform.Rotate(Vector3.up,degreesToRotate,Space.Self);
		transform.Translate(transform.forward*Time.deltaTime*movementSpeed);	
		
	}
	
	void CheckBoundaries()
	{
		float dist = (transform.position - Camera.main.transform.position).z;
		float leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0,0,dist)).x;
		float rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1,0,dist)).x;
		float bottomBorder = Camera.main.ViewportToWorldPoint(new Vector3(0,0,dist)).z;
		float topBorder = Camera.main.ViewportToWorldPoint(new Vector3(0,1,dist)).z;
		
		if(transform.position.x <= leftBorder || transform.position.x >= rightBorder)
			transform.Rotate(Vector3.up,RandomAngle(),Space.Self);
		if(transform.position.z >= topBorder || transform.position.z <= bottomBorder)
			transform.Rotate(Vector3.up,RandomAngle(),Space.Self);
	}
	
	float RandomAngle()
	{
		Random.seed = (int) Time.time;
		return Random.Range(-120.0F,120.0F);
	}
}
