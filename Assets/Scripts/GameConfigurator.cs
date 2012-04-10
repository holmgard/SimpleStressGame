using UnityEngine;
using System.Collections;

public class GameConfigurator : MonoBehaviour {
	
	GameLogic gl;
	FractalTexture ft;
	Player pl;
	GameObject whiteNoise;
	
	// Use this for initialization
	void Start () {
		gl = (GameLogic)FindObjectOfType(typeof(GameLogic));
		ft = (FractalTexture)FindObjectOfType(typeof(FractalTexture));
		pl = (Player)FindObjectOfType(typeof(Player));
		whiteNoise = GameObject.Find("WhiteNoise");
		Debug.Log("White Noise: " + whiteNoise.ToString());
	}
	
	// Update is called once per frame
	
	public void ConfigureSession(ExpSession ses)
	{
		ft.enabled = ses.noise;
		if(ses.noise)
			whiteNoise.GetComponent<AudioSource>().Play();
		else
			whiteNoise.GetComponent<AudioSource>().Stop();
		gl.currentStimulus = ses.stimulus;
		pl.renderer.material = ses.playerColor;
		pl.GetComponent<TrailRenderer>().material = ses.playerColor;
	}	
}