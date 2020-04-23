/* Name: L. Yao
 * Date: November 8, 2019
 * Desc: Handles all of the profession-related stuff for citizens. Should be attached to the game controller. */
// As of November 19, 2019, this script has been deprecated.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A collection of values used in specific professions
[System.Serializable]
public class JobValues
{
	// Huntable creatures and how dangerous they are.
	public List<string> huntablesNames;
	public List<int> huntablesDanger; 
}

public class JobScript : MonoBehaviour
{
	/*
	public List<string> profValidTargetsKeys; // Name of profession
	public List<ValidTargetTypes> profValidTargetsValues; // Valid targets for profession
	private Dictionary<string, ValidTargetTypes> profValidTargets = new Dictionary<string, ValidTargetTypes>();

	// These values are used in specific professions, and are located here so they can be changed from the Unity editor.
	public JobValues jobValues;

	// What professions each species is able to have.
	private Dictionary<string, List<string>> validProfessions = new Dictionary<string, List<string>>();
	private GameControllerScript gc;

	// Start is called before the first frame update
	private void Start()
	{
		gc = GetComponent<GameControllerScript>(); // This script is going to be attached to the game controller object.

		// Set up the dictionary of valid target types for each profession
		for (int i = 0; i < profValidTargetsKeys.Count; i++)
		{
			profValidTargets[profValidTargetsKeys[i]] = profValidTargetsValues[i];
		}

		// Set up valid professions for each species
		validProfessions["Human"] = new List<string>
		{
			"Hunter"
		};
	}

	public void SetJob(CitizenScript citizen)
	{
		// Set their profession
		int jobIndex = Random.Range(0, validProfessions[citizen.species].Count);
		citizen.profession = validProfessions[citizen.species][jobIndex];
		// Update their valid target types
		ValidTargetTypes profVT = profValidTargets[citizen.profession];
		citizen.validTargetTypes.allowDead = profVT.allowDead;
		citizen.validTargetTypes.allowedSpecies = profVT.allowedSpecies;
		citizen.validTargetTypes.allowedProfessions = profVT.allowedProfessions;
		citizen.validTargetTypes.minWealth = profVT.minWealth;
		citizen.validTargetTypes.maxWealth = profVT.maxWealth;
	}

	public void DoJob(CitizenScript citizen)
	{
		if (!citizen.isDead)
		{
			StartCoroutine(citizen.profession, citizen);
		}
	}

	// Hunters hunt various creatures and earn gold. They might die from doing so.
	private IEnumerator Hunter(CitizenScript citizen)
	{ 
		// Pick a random huntable creature
		int i = Random.Range(0, jobValues.huntablesNames.Count);
		KeyValuePair<string, int> toHunt = new KeyValuePair<string, int>(jobValues.huntablesNames[i], jobValues.huntablesDanger[i]);

		gc.LogMessage(citizen.name + " is hunting a " + toHunt.Key + ".", "LGray");
		int failThreshold = citizen.jobSkill - toHunt.Value; // Rolls of at least this number will result in a failure
		int deathThreshold = failThreshold + 40; // Rolls of at least this number result in a death.
		int huntRoll = Random.Range(0, 100); // Rolls from 0 to 99, so if failThreshold >= 100, it always succeeds.

		if (huntRoll < failThreshold) // Success
		{
			// Hunter gets between 3 to 7 gold plus an amount determined by their jobSkill and how dangerous their prey was.
			int payoff = Random.Range(3, 8) + (citizen.jobSkill + toHunt.Value) / 15;
			gc.LogMessage(citizen.name + " has caught a " + toHunt.Key + " worth " + payoff + " Gold.", "LGray");
			citizen.Speak("A fine catch.");
			citizen.GetPaid(payoff, true);
		}
		else if (huntRoll < deathThreshold) // Failure, but not a death
		{
			int msgRoll = Random.Range(0, 2);
			if (msgRoll == 0)
			{
				citizen.Speak("Blasted " + toHunt.Key + "! It got away...");
			}
			else
			{
				citizen.Speak("Damn, that was close. That " + toHunt.Key + " almost got me.");
			}
		}
		else // Creature kills the hunter
		{
			gc.LogMessage(toHunt.Key + ": *Roars*", "DGreen"); // Yes, I am aware that a rabbit or spider roaring is silly.
			gc.LogMessage("The " + toHunt.Key + " has mortally wounded " + citizen.name + "!", "Yellow");
			citizen.Kill();
		}

		yield break;
	} */
}
