using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExperimentalTrial : MonoBehaviour {
	
	public Texture2D[] visualStimuli;
	public AudioClip[] auditiveStimuli;
	public Material[] playerColors;
	
	public List<Stimulus> stimuli;
	
	public List<ExpSet> comparisonSets;
	public List<ExpSet> randomSets;
	public List<ExpSet> totalSets;
	
	public ExpSet mostRecentSet;
	public ExpSession mostRecentSession;
	
	public int lengthOfSets;
	
	// Use this for initialization
	void Start ()
	{
		Debug.Log("Start");
		SetupStimuli();
		SetupComparisonSets();
		SetupRandomSets();
		totalSets = new List<ExpSet>();
		totalSets.AddRange(comparisonSets);
		totalSets.AddRange(randomSets);
		mostRecentSet = totalSets[0];
		mostRecentSession = totalSets[0].sessionA;
		Debug.Log("Generated a total of " + totalSets.Count.ToString() + " sets");
		Debug.Log(GenerateSetReport());
	}
	
	string GenerateSetReport()
	{
		string result = "";
		result += "Report of applied experimental sets for this experimental trial\n";
		result += "----------\n";
		foreach(ExpSet expSet in totalSets)
		{
			result += "Set number: " + totalSets.IndexOf(expSet) + "\n";
			result += "Stimulus A type: " + expSet.sessionA.stimulus.stimType.ToString() + "\n";
			if(expSet.sessionA.stimulus.stimType == Stimulus.StimulusTypes.auditive)
			{
				result += "Stimulus A name: " + expSet.sessionA.stimulus.audio.name + "\n";
			}
			if(expSet.sessionA.stimulus.stimType == Stimulus.StimulusTypes.visual)
			{
				result += "Stimulus A name: " + expSet.sessionA.stimulus.image.name + "\n";
			}
			if(expSet.sessionB.stimulus.stimType == Stimulus.StimulusTypes.auditive)
			{
				result += "Stimulus A name: " + expSet.sessionB.stimulus.audio.name + "\n";
			}
			if(expSet.sessionB.stimulus.stimType == Stimulus.StimulusTypes.visual)
			{
				result += "Stimulus A name: " + expSet.sessionB.stimulus.image.name + "\n";
			}
			result += "-----\n";
		}
		return result;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(randomSets != null)
			lengthOfSets = totalSets.Count;
	}
	
	public ExpSet GetNextExpSet()
	{
		ExpSet temp = mostRecentSet;
		mostRecentSet = totalSets[(totalSets.IndexOf(mostRecentSet)+1)];
		return mostRecentSet;
	}
	
	public ExpSession GetNextExpSession()
	{
		if(mostRecentSession == mostRecentSet.sessionB)
		{
			mostRecentSession = GetNextExpSet().sessionA;
		} else {
			mostRecentSession = mostRecentSet.sessionB;
		}
		return mostRecentSession;
		
	}
	
	void SetupStimuli()
	{
		Debug.Log("SetupStimuli");
		stimuli = new List<Stimulus>();
		for(int i = 0; i < visualStimuli.Length; i++)
		{
			stimuli.Add(new Stimulus(visualStimuli[i]));
		}
		for(int j = 0; j < auditiveStimuli.Length; j++)
		{
			stimuli.Add(new Stimulus(auditiveStimuli[j]));
		}
	}
	
	void SetupComparisonSets()
	{
		Debug.Log("SetupComparisonSets");
		comparisonSets = new List<ExpSet>();
		
		int playerColor = Random.Range(0,2);
		
		foreach(Stimulus curStim in stimuli)
		{
			int PCA = playerColor;
			int PCB = Mathf.Abs(playerColor-1);
			comparisonSets.Add(new ExpSet(new ExpSession(curStim,false,playerColors[PCA]),new ExpSession(curStim,true,playerColors[PCB])));
			Debug.Log("Comp. Set Player Color: " + playerColor);
			playerColor = Random.Range(0,2);
		}
	}
	
	void SetupRandomSets()
	{
		Debug.Log("SetupRandomSets");
		//Prepare list for sets
		randomSets = new List<ExpSet>();
		Debug.Log(randomSets.Count);
		Debug.Log(stimuli.Count);
		Debug.Log(stimuli.Count*( stimuli.Count - 1 ));
		
		int iteration = 0;
		while( randomSets.Count < ( stimuli.Count*( stimuli.Count - 1 ) ) * 2 )
		{
			Debug.Log("Loop");
			Debug.Log("Sets:" + randomSets.Count.ToString());
			
			iteration++;
			Debug.Log("Iterations: " + iteration);
			
			//Pick two random stimuli
			Stimulus stimCand_a = stimuli[Random.Range(0,stimuli.Count)];
			Stimulus stimCand_b = stimuli[Random.Range(0,stimuli.Count)];
			
			//Test that thay are not the same - if they are, run the loop again
			if(stimCand_a.Equals(stimCand_b))
			{
				Debug.Log("Got a twin stimulus set, trying again");
				continue;
			}
			
			int playerColorA = Random.Range(0,playerColors.Length);
			Debug.Log("ColorA: " + playerColorA);
			int playerColorB = Mathf.Abs(playerColorA-1);
			Debug.Log("ColorB: " + playerColorB);
			
			//Make an experimental set and check that it is not a clone of one already in the list of sets
			ExpSet candidate = new ExpSet(new ExpSession(stimCand_a,System.Convert.ToBoolean(Random.Range(0,2)),playerColors[playerColorA]),new ExpSession(stimCand_b,System.Convert.ToBoolean(Random.Range(0,2)),playerColors[playerColorB]));
			bool candidateIsDouble = false;
			foreach(ExpSet curSet in randomSets)
			{
				if(curSet.Equals(candidate))
				{
					Debug.Log("Candidate already in sets, trying again");
					candidateIsDouble = true;
				}
			}
			if(candidateIsDouble)
				continue;
			
			//It it qualifies, add the new set of sessions to the list of sets
			randomSets.Add(candidate);
			Debug.Log("Ses A: " + candidate.sessionA.stimulus.stimType.ToString() );
			Debug.Log("Ses B: " + candidate.sessionB.stimulus.stimType.ToString() );
		}
	}
}

public class ExpSet{
	public ExpSession sessionA;
	public ExpSession sessionB;
	
	public ExpSet(ExpSession sesA, ExpSession sesB)
	{
		sessionA = sesA;
		sessionB = sesB;
	}
	
	public bool Equals(ExpSet other)
	{
		bool result = true;
		
		if(this.sessionA.stimulus == other.sessionA.stimulus)
		{
			result = result && true;
		} else {
			result = false;
		}
		if(this.sessionB.stimulus == other.sessionB.stimulus)
		{
			result = result && true;
		} else {
			result = false;
		}
		if(this.sessionA.noise == other.sessionA.noise)
		{
			result = result && true;
		} else {
			result = false;
		}
		if(this.sessionB.noise == other.sessionB.noise)
		{
			result = result && true;
		} else {
			result = false;
		}
		
		return result;
	}
}

public class ExpSession
{
	public float sessionTimeSeconds = 30F;
	public int targetsCaught = 0;
	public int threathsHit = 0;
	public int points = 0;	
	public bool noise;
	public Material playerColor;
	
	public Stimulus stimulus;
	
	public ExpSession(Stimulus stim, bool noi, Material playCol)
	{
		stimulus = stim;
		noise = noi;
		playerColor = playCol;
	}
}

public class Stimulus{
	
	public enum StimulusTypes {visual,auditive}
	public bool Equals(Stimulus other)
	{
		bool result = true;
		
		if(this.stimType == other.stimType)
		{
			result = result && true;
		}
		else
		{
			result = false;
		}
		if(this.audio == other.audio)
		{
			result = result && true;
		} else {
			result = false;
		}
		if(this.image == other.image){
			result = result && true;
		} else {
			result = false;
		}
		
		return result;
	}
		
	public Texture2D image;	
	public StimulusTypes stimType;
	public AudioClip audio;
	
	public Stimulus(AudioClip aud)
	{
		stimType = StimulusTypes.auditive;
		audio = aud;
	}
	
	public Stimulus(Texture2D img)
	{
		stimType = StimulusTypes.visual;
		image = img;
	}
}