using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {
	
	Vector3 bottomLeft;
	Vector3 bottomRight;
	Vector3 topLeft;
	Vector3 topRight;
	
	// Use this for initialization
	void Start () {
		
		float dist = (transform.position - Camera.main.transform.position).z;
		
		bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0,0,dist));
		bottomRight = Camera.main.ViewportToWorldPoint(new Vector3(1,0,dist));
		topLeft = Camera.main.ViewportToWorldPoint(new Vector3(0,1,dist));
		topRight = Camera.main.ViewportToWorldPoint(new Vector3(1,1,dist));
	}
	
	// Update is called once per frame
	void Update () {
		DrawBorders();
	}
	
	void OnDrawGizmos(){
	}
	
	void DrawBorders()
	{
		
		//Left
		Debug.DrawLine(bottomLeft,topLeft,Color.red,Mathf.Infinity);
		//Top
		Debug.DrawLine(topLeft,topRight,Color.red,Mathf.Infinity);
		//Right
		Debug.DrawLine(topRight,bottomRight,Color.red,Mathf.Infinity);
		//Bottom
		Debug.DrawLine(bottomRight,bottomLeft,Color.red,Mathf.Infinity);
	}
}
