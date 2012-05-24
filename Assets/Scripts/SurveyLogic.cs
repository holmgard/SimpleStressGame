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
			GUI.skin = surveySkin;
			InitialSurvey();
			break;
		case GameState.preferenceSurvey:
			GUI.skin = surveySkin;
			ForcedChoiceFourAltSurvey();
			break;
		default:
			break;
		}
	}
	
	
	public float menuWidth = 0.6F;
	public float menuHeight = 0.85F;
	public float surveyItemVerticalSpacing = 5.0f;
	
	void InitialSurvey()
	{
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
					surveyData.participantNumber = GUILayout.TextField(surveyData.participantNumber);
				GUILayout.EndHorizontal();
				GUILayout.Space(surveyItemVerticalSpacing);
				GUILayout.BeginHorizontal();
					GUILayout.Label("What is your gender?");
					surveyData.genderSelected = GUILayout.SelectionGrid(surveyData.genderSelected,surveyData.genderOptions,2);
				GUILayout.EndHorizontal();
				GUILayout.Space(surveyItemVerticalSpacing);
				GUILayout.BeginHorizontal();
					GUILayout.Label("What is your age?");
					surveyData.age = GUILayout.TextField(surveyData.age);
				GUILayout.EndHorizontal();
				GUILayout.Space(surveyItemVerticalSpacing);
				GUILayout.BeginHorizontal();
					GUILayout.Label("How often do you play computer games?");
					surveyData.gameExperienceSelected = GUILayout.SelectionGrid(surveyData.gameExperienceSelected,surveyData.gameExperienceOptions,2);
				GUILayout.EndHorizontal();
				GUILayout.Space(surveyItemVerticalSpacing);
				GUILayout.BeginHorizontal();
					GUILayout.Label("Are you, as far as you know, colorblind?");
					surveyData.colorBlindInt = GUILayout.SelectionGrid(surveyData.colorBlindInt,surveyData.colorBlindOptions,2);
				GUILayout.EndHorizontal();
				GUILayout.Space(surveyItemVerticalSpacing);
				GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if(GUILayout.Button("Start", surveySkin.GetStyle("button_green"),GUILayout.Height(30f)))
					{
						gl.StartNewGame();
					}
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	int forcedChoiceChallengeSelected = 0;
	string[] forcedChoiceChallengeOptions = {"The first game was most challenging","The second game was most challeging","They were equally challenging","None of them were challenging"};
	
	string[] forcedChoiceStressOptions = {"The first game was most stressful","The second game was most stressful","They were equally stressful","None of them were stressful"};	
	int forcedChoiceStressSelected = 0;
	
	public float prefMenuWidth = 0.3155F;
	public float prefMenuHeight = 0.3F;
	//public float surveyItemVerticalSpacing = 5.0f;
	
	/*void ForcedChoiceFourAltSurvey()
	{	
		GUI.skin = preferenceSkin;
		GUILayout.BeginArea(new Rect(Screen.width*(1-prefMenuWidth)/2,Screen.height*(1-prefMenuHeight)/2,Screen.width*prefMenuWidth,Screen.height*prefMenuHeight),surveySkin.box);
		//GUILayout.BeginArea(new Rect(Screen.width*0.5F-Screen.width*0.8F*0.5F,Screen.height*0.5F-Screen.height*0.8F*0.5F,Screen.width*0.8F,Screen.height*0.8F));
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical();
					GUILayout.Label("Which of the two games you just played did you find the most stressful?\nPlease choose from the options below. Click an option to choose it.");
					forcedChoiceStressSelected = GUILayout.SelectionGrid(forcedChoiceStressSelected,forcedChoiceStressOptions,1);
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			if(GUILayout.Button("Go to next game"))
			{
				EndPreferenceSurvey();
				gl.StartNewGame();
			}
		GUILayout.EndArea();
	}*/
	
	bool first = false;
	bool second = false;
	bool equal = false;
	bool none = false;
	
	void ForcedChoiceFourAltSurvey()
	{	
		GUI.skin = preferenceSkin;
		GUILayout.BeginArea(new Rect(Screen.width*(1-prefMenuWidth)/2,Screen.height*(1-prefMenuHeight)/2,Screen.width*prefMenuWidth,Screen.height*prefMenuHeight),preferenceSkin.box);
		//GUILayout.BeginArea(new Rect(Screen.width*0.5F-Screen.width*0.8F*0.5F,Screen.height*0.5F-Screen.height*0.8F*0.5F,Screen.width*0.8F,Screen.height*0.8F));
			/*GUILayout.BeginVertical();
				GUILayout.Label("Which of the two games you just played did you find the most challenging?\nPlease choose from the options below:");
				forcedChoiceChallengeSelected = GUILayout.SelectionGrid(forcedChoiceChallengeSelected,forcedChoiceChallengeOptions,1);
			GUILayout.EndVertical();*/
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical();
					GUILayout.Label("Which of the two games you just played did you find the most stressful?\nPlease choose from the options below. Click an option to choose it.");
					//forcedChoiceStressSelected = GUILayout.SelectionGrid(forcedChoiceStressSelected,forcedChoiceStressOptions,1);
					GUILayout.BeginHorizontal();
						first = GUILayout.Toggle(first,"The first game");
						GUILayout.Label(playerImages[gl.expTrial.PCA]);
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						second = GUILayout.Toggle(second,"The second game");
						GUILayout.Label(playerImages[gl.expTrial.PCB]);
					GUILayout.EndHorizontal();
					equal = GUILayout.Toggle(equal,"They were equally stressful");
					none = GUILayout.Toggle(none,"None of them were stressful");
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			int numSelected = 0;
			if(first == true)
			{
				//latestSelection = 0;
				numSelected++;
			}
			if(second == true)
			{
					//latestSelection = 1;
					numSelected++;
			}
			if(equal == true)
			{
					//latestSelection = 2;
					numSelected++;
			}
			if(none == true)
			{
					//latestSelection = 3;
					numSelected++;
			}
			
			if( numSelected > 1 || numSelected < 1)
			{
			
				if(first == true && latestSelection == 0)
					first = false;
				else if(first == true)
					latestSelection = 0;
					
				if(second == true && latestSelection == 1)
					second = false;
				else if(second == true)
					latestSelection = 1;
			
				if(equal == true && latestSelection == 2)
					equal = false;
				else if(equal == true)
					latestSelection = 2;
			
				if(none == true && latestSelection == 3)
					none = false;
				else if(none == true)
					latestSelection = 3;	
			
				if(GUILayout.Button("Please pick exactly one\nClick again to de-select"))
				{
					
				}
			} else
			{
				if(GUILayout.Button("Go to next game"))				
				{
					EndPreferenceSurvey();
					gl.StartNewGame();
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	int latestSelection;
	
	public void StartPreferenceSurvey()
	{
		currentPreferenceSurveyData = new PreferenceSurveyData(gl.expTrial,gl.expTrial.mostRecentSet);
		preferenceSurveyData.Add(currentPreferenceSurveyData);
		currentPreferenceSurveyData.MarkStart();
	}
	
	
	public void EndPreferenceSurvey()
	{
		if(first == true)
			currentPreferenceSurveyData.SetMostStressfullGame("first");
		if(second == true)
			currentPreferenceSurveyData.SetMostStressfullGame("second");
		if(equal == true)
			currentPreferenceSurveyData.SetMostStressfullGame("equal");
		if(none == true)
			currentPreferenceSurveyData.SetMostStressfullGame("none");
		currentPreferenceSurveyData.MarkEnd();
	}
	
	Thread saveThread;
	
	public void SaveData()
	{
		saveThread = new Thread(new ThreadStart(ThreadSaveData));
		saveThread.IsBackground = true;
		saveThread.Start();
	}
	
	public void ThreadSaveData() //TODO: Maybe this could be rewritten to use LINQ and be really cool?
	{
		string fileName = "";
		fileName += DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString() + "InitialSurvey.dat";
		
		System.IO.File.WriteAllText(fileName, surveyData.ToString());
		
		fileName = "";
		fileName += DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + "_" + DateTime.Now.Second.ToString() + "PreferenceSurveys.dat";
		
		string preferenceSurveyString = "";
		foreach(PreferenceSurveyData prefData in preferenceSurveyData)
		{
			preferenceSurveyString += prefData.ToString();
			preferenceSurveyString += "\n";
		}
		
		System.IO.File.WriteAllText(fileName,preferenceSurveyString);
		
		print ("Survey saving done");
		
		saveThread.Abort();
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
	string mostStressFullGame;
	int numberOfSelectionsMade = 0;
	
	public PreferenceSurveyData(ExperimentalTrial trial, ExpSet exSet)
	{
		this.exSet = exSet;
		this.expTrial = trial;
		setNumber = expTrial.totalSets.IndexOf(exSet);
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
	
	public string ToString()
	{
		return string.Format("{0}\t{1}\t{2}\t{3}",setNumber,preferenceSurveyStart,preferenceSurveyEnd,(preferenceSurveyEnd-preferenceSurveyStart).ToString(),mostStressFullGame);
	}
}