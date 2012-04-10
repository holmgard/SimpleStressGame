using UnityEngine;
using System.Collections;

public class SurveyLogic : MonoBehaviour {
	
	GameLogic gl;
	SurveyData surveyData;
	public Texture2D tutorialImage;
	
	// Use this for initialization
	void Start ()
	{
		gl = (GameLogic)FindObjectOfType(typeof(GameLogic));
		surveyData = new SurveyData();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	void OnGUI(){
		switch(gl.gameState)
		{
		case GameState.initialSurvey:
			InitialSurvey();
			break;
		case GameState.preferenceSurvey:
			ForcedChoiceFourAltSurvey();
			break;
		default:
			break;
		}
	}
	
	void InitialSurvey()
	{
		GUILayout.BeginArea(new Rect(Screen.width*0.5F-Screen.width*0.8F*0.5F,Screen.height*0.5F-Screen.height*0.8F*0.5F,Screen.width*0.8F,Screen.height*0.8F));
			GUILayout.Label("Thank you for participating in this experimental game.\nThe rules of the game are explained in the image below. If you have any questions, please ask your experimental instructor.");
			if(tutorialImage != null)
			{
				GUILayout.Box(tutorialImage);
			}
			GUILayout.Label("Also, before we begin, we would like to ask you a few questions about yourself.\nPlease fill out the following questionnaire and press \"Start\" to start the experiment and game.");
			GUILayout.BeginHorizontal();
				GUILayout.Label("Participant number:");
				surveyData.participantNumber = GUILayout.TextField(surveyData.participantNumber);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				GUILayout.Label("Gender:");
				surveyData.gender = GUILayout.TextField(surveyData.gender);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				GUILayout.Label("Age:");
				surveyData.age = GUILayout.TextField(surveyData.age);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				GUILayout.Label("Game experience");
				surveyData.gameExperienceSelected = GUILayout.SelectionGrid(surveyData.gameExperienceSelected,surveyData.gameExperienceOptions,4);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
				GUILayout.Label("Are you, as far as you know, colorblind?");
				surveyData.colorBlindInt = GUILayout.SelectionGrid(surveyData.colorBlindInt,surveyData.colorBlindOptions,2);
			GUILayout.EndHorizontal();
			if(GUILayout.Button("Start"))
			{
				gl.StartNewGame();
			}
		GUILayout.EndArea();
	}
	
	int forcedChoiceChallengeSelected = 0;
	string[] forcedChoiceChallengeOptions = {"The first game was most challenging","The second game was most challeging","They were equally challenging","None of them were challenging"};
	
	int forcedChoiceStressSelected = 0;
	string[] forcedChoiceStressOptions = {"The first game was most stressful","The second game was most stressful","They were equally stressfull","None of them were stressful"};	
	
	void ForcedChoiceFourAltSurvey()
	{
		GUILayout.BeginArea(new Rect(Screen.width*0.5F-Screen.width*0.8F*0.5F,Screen.height*0.5F-Screen.height*0.8F*0.5F,Screen.width*0.8F,Screen.height*0.8F));
			GUILayout.BeginVertical();
				GUILayout.Label("Which of the two games you just played did you find the most challenging?\nPlease choose from the options below:");
				forcedChoiceChallengeSelected = GUILayout.SelectionGrid(forcedChoiceChallengeSelected,forcedChoiceChallengeOptions,1);
			GUILayout.EndVertical();
			GUILayout.BeginVertical();
				GUILayout.Label("Which of the two games you just played did you find the most stressful?\nPlease choose from the options below. Click an option to choose it.");
				forcedChoiceStressSelected = GUILayout.SelectionGrid(forcedChoiceStressSelected,forcedChoiceStressOptions,1);
			GUILayout.EndVertical();
			GUILayout.Space(50);
			if(GUILayout.Button("Go to next game"))
			{
				gl.StartNewGame();
			}
		GUILayout.EndArea();
	}
}

public class SurveyData
{
	System.DateTime id;
	public string participantNumber = "";
	public string gender = "";
	public string age = "";
	
	public string gameExperience = "";
	public string[] gameExperienceOptions = {"Daily","Weekly","Monthly","Yearly"};
	public int gameExperienceSelected = 0;
	
	public string colorBlind = "";
	public string[] colorBlindOptions = {"No","Yes"};
	public int colorBlindInt = 0;
	
	public SurveyData()
	{
		id = System.DateTime.Now;
	}
	
	
	public override string ToString()
	{
		return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}",id,participantNumber,gender,gameExperience,colorBlind);
	}
}