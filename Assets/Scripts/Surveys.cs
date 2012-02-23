using UnityEngine;
using System.Collections;

public class InitialSurvey : MonoBehaviour {
	
	public GameLogic gl;
	
	// Use this for initialization
	void Start () {
		gl = (GameLogic)FindObjectOfType(typeof(GameLogic));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		
	}
}

public class ForcedChoiceFourAltSurvey : MonoBehaviour {
	public GameLogic gl;
	
	// Use this for initialization
	void Start () {
		gl = (GameLogic)FindObjectOfType(typeof(GameLogic));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		
	}
}
