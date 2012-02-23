using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExperimentalTrial : MonoBehaviour {
	
	public Texture2D[] visualStimuli;
	public AudioClip[] auditiveStimuli;
	public List<Stimulus> stimuli;
	
	public List<ExpSet> sets;
	
	public int lengthOfSets;
	
	// Use this for initialization
	void Start ()
	{
		Debug.Log("Start");
		SetupStimuli();
		SetupRandomSets();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(sets != null)
			lengthOfSets = sets.Count;
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
	}
	
	void SetupRandomSets()
	{
		Debug.Log("SetupRandomSets");
		//Prepare list for sets
		sets = new List<ExpSet>();
		Debug.Log(sets.Count);
		Debug.Log(stimuli.Count);
		Debug.Log(stimuli.Count*( stimuli.Count - 1 ));
		
		int iteration = 0;
		while( sets.Count < ( stimuli.Count*( stimuli.Count - 1 ) ) * 2 )
		{
			Debug.Log("Loop");
			Debug.Log("Sets:" + sets.Count.ToString());
			
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
			
			//Make an experimental set and check that it is not a clone of one already in the list of sets
			ExpSet candidate = new ExpSet(new ExpSession(stimCand_a,System.Convert.ToBoolean(Random.Range(0,2))),new ExpSession(stimCand_b,System.Convert.ToBoolean(Random.Range(0,2))));
			bool candidateIsDouble = false;
			foreach(ExpSet curSet in sets){
				if(curSet.Equals(candidate))
				{
					Debug.Log("Candidate already in sets, trying again");
					candidateIsDouble = true;
				}
			}
			if(candidateIsDouble)
				continue;
			
			//It it qualifies, add the new set of sessions to the list of sets
			sets.Add(candidate);
		}
	}
}

public class ExpSet{
	ExpSession sessionA;
	ExpSession sessionB;
	
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
	public int points = 0;
	
	public bool noise;
	
	public Stimulus stimulus;
	
	public ExpSession(Stimulus stim, bool noi)
	{
		stimulus = stim;
		noise = noi;
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