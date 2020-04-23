/* Name: L. Yao
 * Date: November 6, 2019
 * Desc: This is where the base NameScript for the various races is stored, as well as some functions relating to them. 
 * Names are generated from fantasynamegenerators.com */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameScript : MonoBehaviour
{
	public CitizenScript[] citizenPrefabs;

	private Dictionary<string, List<string>> nameListsFirst = new Dictionary<string, List<string>>();
	private Dictionary<string, List<string>> nameListsLast = new Dictionary<string, List<string>>();
	private Dictionary<string, List<string>> possibleGenders = new Dictionary<string, List<string>>();

	private void Start()
	{
		foreach (CitizenScript cit in citizenPrefabs)
		{
			possibleGenders[cit.species] = new List<string>(cit.possibleGenders);
		}

		// Set up names for the various races
		ReadNames("Human");
	}

	private void ReadNames(string race) // Make sure word is capitalized.
	{
		// Read in a different namelist for each race and each gender of that race
		for (int curGender = 0; curGender < possibleGenders[race].Count; curGender++)
		{
			TextAsset tempTextFirst, tempTextLast; // A temporary TextAsset for reading in names.
			string[] firstNames, lastNames;

			nameListsFirst[race + possibleGenders[race][curGender]] = new List<string>();
			nameListsLast[race + possibleGenders[race][curGender]] = new List<string>(); // Since last names here also include epithets, there might be different possible ones for each gender.

			tempTextFirst = Resources.Load<TextAsset>("NameLists/" + race + possibleGenders[race][curGender] + "First");
			tempTextLast = Resources.Load<TextAsset>("NameLists/" + race + possibleGenders[race][curGender] + "Last");
			firstNames = tempTextFirst.text.Split('\n'); // Split the TextAssets into arrays of first and last names, separated by each newline character.
			lastNames = tempTextLast.text.Split('\n');
			for (int i = 0; i < firstNames.Length - 1; i++) // It is assumed that the last line is an empty line, since that's how I'm generating the text files
			{
				nameListsFirst[race + possibleGenders[race][curGender]].Add(firstNames[i].Trim());
			}
			for (int i = 0; i < lastNames.Length - 1; i++)
			{
				nameListsLast[race + possibleGenders[race][curGender]].Add(lastNames[i].Trim());
			}
		}
	}

	public string GetName(string race, string gender)
	{
		int iFirst = Random.Range(0, nameListsFirst[race + gender].Count);
		int iLast = Random.Range(0, nameListsLast[race + gender].Count);
		return nameListsFirst[race + gender][iFirst] + " " + nameListsLast[race + gender][iLast];
	}
}