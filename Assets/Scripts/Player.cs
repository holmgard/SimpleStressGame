using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	GameLogic gl;
	
	public int points = 0;
	public float mouseLag = 0;
	
	// Use this for initialization
	void Start () {
		gl = GameLogic.Instance();
	}
	
	// Update is called once per frame
	void Update () {
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
		target = collision.gameObject.GetComponent<Target>();
		if(target != null)
		{
			target.Catch();
			points++;
		}
	}
}
