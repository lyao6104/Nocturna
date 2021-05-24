/* Name: L. Yao
 * Date: November 9, 2019
 * Desc: A script for setting the portrait of a citizen. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GendersLib;

public class PortraitScript : MonoBehaviour
{
	// Portraits should be 135x130 px
	//public List<string> speciesPortraitCountKeys;
	//public List<int> speciesPortraitCountValues;
	public Sprite fallbackPortrait; // Used when a valid portrait can't be found
	public CitizenScript[] citizenPrefabs;

	private Dictionary<string, List<Sprite>> citizenPortraits = new Dictionary<string, List<Sprite>>();

	private void Start()
	{
		//for (int i = 0; i < speciesPortraitCountKeys.Count; i++)
		//{
		//	speciesPortraitCount[speciesPortraitCountKeys[i]] = speciesPortraitCountValues[i];
		//}

		//foreach(KeyValuePair<string, int> specPortraits in speciesPortraitCount)
		//{
		//	citizenPortraits[specPortraits.Key] = new List<Sprite>();
		//	for (int i = 0; i < specPortraits.Value; i++)
		//	{
		//		string filePath = "Portraits/" + specPortraits.Key + "/" + 
		//		citizenPortraits[specPortraits.Key].Add(Resources.Load<Sprite>())
		//	}
		//}

		// This should read in all of the valid portraits for each species
		foreach (CitizenScript cit in citizenPrefabs)
		{
			foreach (string gender in GendersUtil.GetGenderNames(cit.species))
			{
				int i = 0;
				citizenPortraits[cit.species + gender] = new List<Sprite>();
				while (true)
				{
					string filePath = "Portraits/" + cit.species + "/" + gender + i;
					Sprite toAdd = Resources.Load<Sprite>(filePath);
					if (toAdd == null)
					{
						if (i == 0)
						{
							citizenPortraits[cit.species + gender].Add(fallbackPortrait);
						}
						//Debug.Log(citizenPortraits[cit.species + gender].Count + " portraits loaded for " + cit.species + gender);
						break;
					}
					else
					{
						citizenPortraits[cit.species + gender].Add(toAdd);
					}
					i++;
				}
			}
		}
	}

	public Sprite GetPortrait(string race, Gender gender)
	{
		string genderToUse = gender.name;
		int useOwnGenderRoll = Random.Range(0, 100);
		if (gender.useAnyPortrait && useOwnGenderRoll >= gender.ownGenderPortraitChance)
		{	
			List<string> possiblePortraitLists = GendersUtil.GetGenderNames(race);
			possiblePortraitLists.RemoveAll(x => GendersUtil.GetGenderByName(race, x).portraitsAreExclusive || x.Equals(gender.name));
			int portraitListRoll = Random.Range(0, possiblePortraitLists.Count);
			genderToUse = possiblePortraitLists[portraitListRoll];
		}

		int i = Random.Range(0, citizenPortraits[race + genderToUse].Count);
		//Debug.Log("Portrait is " + citizenPortraits[race + gender][i].name);
		return citizenPortraits[race + genderToUse][i];
	}
}
