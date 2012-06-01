using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class SurveyLogic : MonoBehaviour {
	
	GameLogic gl;
	InitialSurveyData surveyData;
	public List<PreferenceSurveyData> preferenceSurveyData;
	PreferenceSurveyData currentPreferenceSurveyData;
	public Texture2D tutorialImage;
	public GUISkin surveySkin;
	public GUISkin preferenceSkin;
	public Texture2D[] playerImages;
	
	// Use this for initialization
	void Start ()
	{
		gl = (GameLogic)FindObjectOfType(typeof(GameLogic));
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
			GUI.skin = surveySkin;
			InitialSurvey();
			break;
		case GameState.preferenceSurvey:
			GUI.skin = surveySkin;
			ForcedChoiceFourAltSurvey();
			break;
		case GameState.finalPreferenceSurvey:
			FinalPreferenceSurveyGUI();
			break;
		default:
			break;
		}
	}
	
	
	public float menuWidth = 0.6F;
	public float menuHeight = 0.85F;
	public float surveyItemVerticalSpacing = 5.0f;
	
	string pNumber = "";
	
	int genderNumber = 0;
	public string[] genderOptions = {"Female","Male"};
	
	string age = "";
	
	public string[] gameExperienceOptions = {"Almost every day","Every week","At least once a month","Once a year or less"};
	int gameExpNumber = 0;
	
	int colorBlindNumber;
	public string[] colorBlindOptions = {"No","Yes"};
	
	void InitialSurvey()
	{
		GUI.Box(new Rect(0f,0f,1024f,768f),"",surveySkin.GetStyle("BoxBackground"));
		//GUILayout.BeginArea(new Rect(Screen.width*0.5F-Screen.width*0.8F*0.5F,Screen.height*0.5F-Screen.height*0.8F*0.5F,Screen.width*0.8F,Screen.height*0.8F));
		GUILayout.BeginArea(new Rect(Screen.width*(1-menuWidth)/2,Screen.height*(1-menuHeight)/2,Screen.width*menuWidth,Screen.height*menuHeight),surveySkin.box);
			GUILayout.Label("Thank you for participating in this experimental game.\nThe game is about catching the green target and avoiding the red enemies (see below). If you have any questions, please ask your experimenter.");
			if(tutorialImage != null)
			{
				GUILayout.Box(tutorialImage);
			}
			GUILayout.Label("Also, before we begin, we would like to ask you a few questions about yourself.\nPlease fill out the following questionnaire and press \"Start\" to start the experiment and game.");
			GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
					GUILayout.Label("Participant number:");
					pNumber = GUILayout.TextField(pNumber);
				GUILayout.EndHorizontal();
				GUILayout.Space(surveyItemVerticalSpacing);
				GUILayout.BeginHorizontal();
					GUILayout.Label("What is your gender?");
					genderNumber = GUILayout.SelectionGrid(genderNumber,new string[]{"Female","Male"},2);
				GUILayout.EndHorizontal();
				GUILayout.Space(surveyItemVerticalSpacing);
				GUILayout.BeginHorizontal();
					GUILayout.Label("What is your age?");
					age = GUILayout.TextField(age);
				GUILayout.EndHorizontal();
				GUILayout.Space(surveyItemVerticalSpacing);
				GUILayout.BeginHorizontal();
					GUILayout.Label("How often do you play computer games?");
					gameExpNumber = GUILayout.SelectionGrid(gameExpNumber,gameExperienceOptions,2);
				GUILayout.EndHorizontal();
				GUILayout.Space(surveyItemVerticalSpacing);
				GUILayout.BeginHorizontal();
					GUILayout.Label("Are you, as far as you know, colorblind?");
					colorBlindNumber = GUILayout.SelectionGrid(colorBlindNumber,colorBlindOptions,2);
				GUILayout.EndHorizontal();
				GUILayout.Space(surveyItemVerticalSpacing);
				GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if(GUILayout.Button("Start", surveySkin.GetStyle("button_green"),GUILayout.Height(30f)))
					{
						surveyData = new InitialSurveyData(pNumber,genderOptions[genderNumber],age,gameExperienceOptions[gameExpNumber],colorBlindOptions[colorBlindNumber]);
						gl.StartNewGame();
					}
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	string[] forcedChoiceStressOptions = {"The first game was most stressful","The second game was most stressful","They were equally stressful","None of them were stressful"};	
	int forcedChoiceStressSelected = 0;
	
	void ForcedChoiceFourAltSurvey()
	{	
		GUI.skin = preferenceSkin;
		
		GUI.Box(new Rect(0,0,1024f,768f), "", preferenceSkin.GetStyle("BoxBackground"));
		
		GUI.Label(new Rect(250f,200f,522f,50f), "Which of the two games you just played did you find the most stressful?\nPlease choose from the options below. Click an option to choose it.");
		forcedChoiceStressSelected = GUI.SelectionGrid(new Rect(250f,250f,261f,80f*4f),forcedChoiceStressSelected,forcedChoiceStressOptions,1);
		
		GUI.Label(new Rect(520f,250f,261f,80f),playerImages[gl.expTrial.currentExpSet.sessionA.colorIndex]);
		GUI.Label(new Rect(520f,250f+80f,261f,80f),playerImages[gl.expTrial.currentExpSet.sessionB.colorIndex]);
		GUI.Label(new Rect(520f,250f+80f*2,261f,80f),"");
		GUI.Label(new Rect(520f,250f+80f*3,261f,80f),"");
		
		if(GUI.Button(new Rect(250f,575f,522f,50f),"Go to next game"))
		{
			EndPreferenceSurvey();
			gl.StartNewGame();
		}
	}
	
	void FinalPreferenceSurveyGUI()
	{
		GUI.skin = preferenceSkin;
		
		GUI.Box(new Rect(0,0,1024f,768f), "", preferenceSkin.GetStyle("BoxBackground"));
		
		GUI.Label(new Rect(250f,200f,522f,50f), "Which of the two games you just played did you find the most stressful?\nPlease choose from the options below. Click an option to choose it.");
		forcedChoiceStressSelected = GUI.SelectionGrid(new Rect(250f,250f,261f,80f*4f),forcedChoiceStressSelected,forcedChoiceStressOptions,1);
		
		GUI.Label(new Rect(520f,250f,261f,80f),playerImages[gl.expTrial.currentExpSet.sessionA.colorIndex]);
		GUI.Label(new Rect(520f,250f+80f,261f,80f),playerImages[gl.expTrial.currentExpSet.sessionB.colorIndex]);
		GUI.Label(new Rect(520f,250f+80f*2,261f,80f),"");
		GUI.Label(new Rect(520f,250f+80f*3,261f,80f),"");
		
		if(GUI.Button(new Rect(250f,575f,522f,50f),"Go to next game"))
		{
			gl.gameState = GameState.gameover;
		}
	}
		
	public void StartPreferenceSurvey()
	{
		currentPreferenceSurveyData = new PreferenceSurveyData(gl.expTrial,gl.expTrial.mostRecentSet);
		preferenceSurveyData.Add(currentPreferenceSurveyData);
		currentPreferenceSurveyData.MarkStart();
	}
	
	
	public void EndPreferenceSurvey()
	{
		currentPreferenceSurveyData.SetMostStressfullGame(forcedChoiceStressSelected.ToString());
		currentPreferenceSurveyData.MarkEnd();
		Debug.Log("Preference survey ended, numer of saved preferencesurveys: " + preferenceSurveyData.Count.ToString());
	}
	
	Thread initialSurveyThread;
	Thread preferenceSurveysThread;
	
	public void SaveData()
	{
		initialSurveyThread = new Thread(new ThreadStart(ThreadSaveInitialSurvey));
		initialSurveyThread.IsBackground = true;
		initialSurveyThread.Start();
		
		preferenceSurveysThread = new Thread(new ThreadStart(ThreadSavePreferenceSurveys));
		preferenceSurveysThread.IsBackground = true;
		preferenceSurveysThread.Start();
	}
	
	public void ThreadSaveInitialSurvey()
	{
		print ("Starting InitialSurvey save...");
		
		string fileName = "";
				fileName += DateTime.Now.Day.ToString();
				fileName += DateTime.Now.Month.ToString();
				fileName += DateTime.Now.Year.ToString();
				fileName += "_";
				fileName += DateTime.Now.Hour.ToString();
				fileName += DateTime.Now.Minute.ToString();
				fileName += DateTime.Now.Second.ToString();
				fileName += "_InitialSurvey";
				fileName += ".dat";
				
				string surveyString = "";
				surveyString += surveyData.ToString();
				System.IO.File.WriteAllText(fileName,surveyString);
		
		print ("InitialSurvey save done...");
		initialSurveyThread.Abort();
	}
	
	public void ThreadSavePreferenceSurveys()
	{
		print ("Starting preferencesurveys save...");
		
		string fileName = "";
				fileName += DateTime.Now.Day.ToString();
				fileName += DateTime.Now.Month.ToString();
				fileName += DateTime.Now.Year.ToString();
				fileName += "_";
				fileName += DateTime.Now.Hour.ToString();
				fileName += DateTime.Now.Minute.ToString();
				fileName += DateTime.Now.Second.ToString();
				fileName += "_PreferenceSurveys";
				fileName += ".dat";
				
				string preferencesString = "";
				foreach(PreferenceSurveyData prefData in preferenceSurveyData)
				{
					preferencesString += prefData.ToString();
					preferencesString += "\n";
				}
				System.IO.File.WriteAllText(fileName,preferencesString);
		
		print ("Preferences save done...");
		preferenceSurveysThread.Abort();
	}

}

public class InitialSurveyData
{
	System.DateTime id;
	public string participantNumber = "";
	public int genderSelected = 0;
	public string[] genderOptions = {"Female","Male"};
	public string gender = "";
	
	public string age = "";
	
	public string gameExperience = "";
	public string[] gameExperienceOptions = {"Almost every day","Every week","At least once a month","Once a year or less"};
	public int gameExperienceSelected = 0;
	
	public string colorBlind = "";
	public string[] colorBlindOptions = {"No","Yes"};
	public int colorBlindInt = 0;
	
	public InitialSurveyData(string pNumber, string gen, string a, string gameExp, string colorB)
	{
		id = System.DateTime.Now;
		participantNumber = pNumber;
		gender = gen;
		age = a;
		gameExperience = gameExp;
		colorBlind = colorB;
	}
	
	
	public override string ToString()
	{
		return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}",id,participantNumber,gender,age,gameExperience,colorBlind);
	}	
}

public class PreferenceSurveyData
{
	ExperimentalTrial expTrial;
	ExpSet exSet;
	//int setNumber;
	float preferenceSurveyStart;
	float preferenceSurveyEnd;
	string mostStressFullGame;
	int numberOfSelectionsMade = 0;
	
	public PreferenceSurveyData(ExperimentalTrial trial, ExpSet exSet)
	{
		this.exSet = exSet;
		this.expTrial = trial;
		//setNumber = expTrial.totalSets.IndexOf(exSet);
	}
	
	public void SetMostStressfullGame(string gameOption)
	{
		mostStressFullGame = gameOption;
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
	
	public override string ToString()
	{
		return string.Format("{0}\t{1}\t{2}\t{3}\t{4}",expTrial.totalSets.IndexOf(exSet),preferenceSurveyStart,preferenceSurveyEnd,(preferenceSurveyEnd-preferenceSurveyStart).ToString(),mostStressFullGame);
	}
}