using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SurveyLogic : MonoBehaviour {
	
	GameLogic gl;
	InitialSurveyData surveyData;
	public List<PreferenceSurveyData> preferenceSurveyData;
	PreferenceSurveyData currentPreferenceSurveyData;
	public Texture2D tutorialImage;
	
	// Use this for initialization
	void Start ()
	{
		gl = (GameLogic)FindObjectOfType(typeof(GameLogic));
		surveyData = new InitialSurveyData();
		preferenceSurveyData = new List<PreferenceSurveyData>();
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
			GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
					GUILayout.Label("Participant number:");
					surveyData.participantNumber = GUILayout.TextField(surveyData.participantNumber);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
					GUILayout.Label("What is your gender?");
					surveyData.genderSelected = GUILayout.SelectionGrid(surveyData.genderSelected,surveyData.genderOptions,2);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
					GUILayout.Label("What is your age?");
					surveyData.age = GUILayout.TextField(surveyData.age);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
					GUILayout.Label("How often do you play computer games?");
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
			GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	int forcedChoiceChallengeSelected = 0;
	string[] forcedChoiceChallengeOptions = {"The first game was most challenging","The second game was most challeging","They were equally challenging","None of them were challenging"};
	
	int forcedChoiceStressSelected = 0;
	string[] forcedChoiceStressOptions = {"The first game was most stressful","The second game was most stressful","They were equally stressful","None of them were stressful"};	
	
	void ForcedChoiceFourAltSurvey()
	{
		GUILayout.BeginArea(new Rect(Screen.width*0.5F-Screen.width*0.8F*0.5F,Screen.height*0.5F-Screen.height*0.8F*0.5F,Screen.width*0.8F,Screen.height*0.8F));
			/*GUILayout.BeginVertical();
				GUILayout.Label("Which of the two games you just played did you find the most challenging?\nPlease choose from the options below:");
				forcedChoiceChallengeSelected = GUILayout.SelectionGrid(forcedChoiceChallengeSelected,forcedChoiceChallengeOptions,1);
			GUILayout.EndVertical();*/
			GUILayout.BeginVertical();
				GUILayout.Label("Which of the two games you just played did you find the most stressful?\nPlease choose from the options below. Click an option to choose it.");
				forcedChoiceStressSelected = GUILayout.SelectionGrid(forcedChoiceStressSelected,forcedChoiceStressOptions,1);
			GUILayout.EndVertical();
			GUILayout.Space(50);
			if(GUILayout.Button("Go to next game"))
			{
				EndPreferenceSurvey();
				gl.StartNewGame();
			}
		GUILayout.EndArea();
	}
	
	public void StartPreferenceSurvey()
	{
		currentPreferenceSurveyData = new PreferenceSurveyData(gl.expTrial,gl.expTrial.mostRecentSet);
		preferenceSurveyData.Add(currentPreferenceSurveyData);
		currentPreferenceSurveyData.MarkStart();
	}
	
	public void EndPreferenceSurvey()
	{
		currentPreferenceSurveyData.MarkEnd();
	}

}

public class InitialSurveyData
{
	System.DateTime id;
	public string participantNumber = "";
	public int genderSelected = 0;
	public string[] genderOptions = {"Female","Male"};
	public string age = "";
	
	public string gameExperience = "";
	public string[] gameExperienceOptions = {"Almost every day","Every week","At least once a month","Once a year or less"};
	public int gameExperienceSelected = 0;
	
	public string colorBlind = "";
	public string[] colorBlindOptions = {"No","Yes"};
	public int colorBlindInt = 0;
	
	public InitialSurveyData()
	{
		id = System.DateTime.Now;
	}
	
	
	public override string ToString()
	{
		return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}",id,participantNumber,genderSelected,gameExperience,colorBlind);
	}	
}

public class PreferenceSurveyData
{
	ExperimentalTrial expTrial;
	ExpSet exSet;
	int setNumber;
	float preferenceSurveyStart;
	float preferenceSurveyEnd;
	int mostStressFullGame;
	int numberOfSelectionsMade = 0;
	
	public PreferenceSurveyData(ExperimentalTrial trial, ExpSet exSet)
	{
		this.exSet = exSet;
		this.expTrial = trial;
		setNumber = expTrial.totalSets.IndexOf(exSet);
	}
	
	public void SetMostStressfulGame(int gameNum)
	{
		mostStressFullGame = gameNum;
		numberOfSelectionsMade++;
	}
	
	public void MarkStart()
	{
		preferenceSurveyStart = Time.time;
	}
	
	public void MarkEnd()
	{
		preferenceSurveyEnd = Time.time;
	}
	
	public string ToString()
	{
		return string.Format("{0}\t{1}\t{2}\t{3}",setNumber,preferenceSurveyStart,preferenceSurveyEnd,(preferenceSurveyEnd-preferenceSurveyStart).ToString(),mostStressFullGame);
	}
}