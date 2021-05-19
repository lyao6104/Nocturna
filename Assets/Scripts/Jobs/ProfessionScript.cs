/* Name: L. Yao
 * Date: November 19, 2019
 * Desc: A new implementation of professions, where each profession is a class that inherits from this base class */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProfessionDialogue
{
	public string species;
	public List<string> dialogue;
}

public class ProfessionScript : MonoBehaviour
{
	public string jobName;
	public ValidTargetTypes validTargetTypes;
	//public List<ProfessionDialogue> genericDialogue; // Anything here gets added to the list of possible speaking dialogue.
	public List<UtilitiesLib.StringIntPair> desiredItemCounts; // Everything here gets added to me.desiredItemCounts upon initialization (same-key entries won't be merged)

	protected GameControllerScript gc;
	protected CitizenScript me;

	private void Start()
	{
		gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
		me = GetComponentInParent<CitizenScript>();
		Init();
	}
	
	// Each different job type has its own initialization code.
	protected virtual void Init()
	{
		me.validTargetTypes = validTargetTypes;

		// Loop through profession-specific dialogue and add it to the citizen's dialogue list.
		// Not needed anymore
		/*
		foreach (ProfessionDialogue dialogueList in genericDialogue)
		{
			if (dialogueList.species == "Any" || dialogueList.species == me.species)
			{
				foreach (string line in dialogueList.dialogue)
				{
					me.dialogue.Add(line);
				}
			}
		}
		*/

		// Loop through profession-specific desired item counts and add it to the citizen's desiredItemCounts.
		foreach (UtilitiesLib.StringIntPair itemCount in desiredItemCounts)
		{
			me.desiredItemCounts.Add(itemCount);
		}
	}

	/// <summary>
	/// Localize a given string, replacing all dynamic dialogue tags with the appropriate text.
	/// Must be overridden by individual professions.
	/// </summary>
	/// <param name="msg">A string containing dynamic dialogue tags to be replaced.</param>
	/// <returns>The localized message, as a string.</returns>
	public virtual string LocalizeString(string msg)
	{
		return msg;
	}

	public virtual void DoJob()
	{
		if (me.isDead)
		{
			return;
		}
	}
}
