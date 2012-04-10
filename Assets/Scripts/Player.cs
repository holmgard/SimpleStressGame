using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public GameLogic gl;
	public float mouseLag = 0;
	public AudioClip getPointsSound;
	
	// Use this for initialization
	void Start () {
		gl = (GameLogic) FindObjectOfType(typeof(GameLogic));
		Debug.Log("GameLogic: " + gl.ToString());
	}
	
	// Update is called once per frame
	void Update () {
		if(gl.gameState == GameState.playingSesA || gl.gameState == GameState.playingSesB)
			FollowMouse();
	}
	
	void FollowMouse()
	{
		float dist = (transform.position - Camera.main.transform.position).z;
		float leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0,0,dist)).x;
		float rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1,0,dist)).x;
		float bottomBorder = Camera.main.ViewportToWorldPoint(new Vector3(0,0,dist)).z;
		float topBorder = Camera.main.ViewportToWorldPoint(new Vector3(0,1,dist)).z;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		Physics.Raycast(ray,out hit,LayerMask.NameToLayer("Background"));
		
		if(hit.transform != null)
			transform.position = new Vector3(Mathf.Clamp(hit.point.x,leftBorder,rightBorder),1,Mathf.Clamp(hit.point.z,bottomBorder,topBorder));
		//transform.position = new Vector3(Mathf.Clamp(transform.position.x,leftBorder,rightBorder),1,Mathf.Clamp(transform.position.x,bottomBorder,topBorder));
	}
	
	void OnCollisionEnter(Collision collision)
	{
		Target target;
		Threat threat;
		
		target = collision.gameObject.GetComponent<Target>();
		if(target != null)
		{
			gl.empConnection.MarkEvent("CaughtTarget");
			target.Catch(this);
			gl.AddPoints();
			gl.SpawnThreat();
		}
		
		threat = collision.gameObject.GetComponent<Threat>();
		if(threat != null)
		{
			gl.empConnection.MarkEvent("HitThreat");
			
			gl.ThreatCollision();
		}
	}
}
