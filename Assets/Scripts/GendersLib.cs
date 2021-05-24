/* Date: May 18, 2021
 * Name: L. Yao
 * Desc: Collection of gender-related things.
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GendersLib
{
	// Could potentially rework this later so that genders have multiple possible sets of pronouns?
	[System.Serializable]
	public class PronounData
	{
		public string subjective, objective, possessiveDeterminer, possessive, reflexive, personType;
		public bool pluralWords;
	}

	[System.Serializable]
	public class Gender
	{
		public string name, colourCode;
		public PronounData pronouns;
		// Can this gender use any portraits of their species?
		public bool useAnyPortrait;
		// Are this gender's portraits excluded from the above?
		public bool portraitsAreExclusive;
		// If this gender can use any portrait, how likely are they to use one specifically for their gender?
		public int ownGenderPortraitChance;
	}

	[System.Serializable]
	public class WeightedGender
	{
		public Gender gender;
		public int weight;
	}

	[System.Serializable]
	public class SpeciesGenders
	{
		public string species;
		public List<WeightedGender> possibleGenders;
	}

	public static class GendersUtil
	{
		private static Dictionary<string, List<WeightedGender>> loadedGenders = new Dictionary<string, List<WeightedGender>>();
		private static Dictionary<string, Dictionary<string, Gender>> loadedGendersByName = new Dictionary<string, Dictionary<string, Gender>>();

		public static void LoadSpeciesGenders(SpeciesGenders speciesGenders)
		{
			loadedGenders[speciesGenders.species] = speciesGenders.possibleGenders;
			loadedGendersByName[speciesGenders.species] = new Dictionary<string, Gender>();
			foreach (WeightedGender gender in speciesGenders.possibleGenders)
			{
				loadedGendersByName[speciesGenders.species][gender.gender.name] = gender.gender;
			}
			Debug.Log(string.Format("Loaded {0} genders for the {1} species.", speciesGenders.possibleGenders.Count, speciesGenders.species));
		}

		public static List<string> GetGenderNames(string species)
		{
			return loadedGendersByName[species].Keys.ToList();
		}

		public static Gender GetGenderByName(string species, string genderName)
		{
			return loadedGendersByName[species][genderName];
		}

		public static Gender AssignGender(string species)
		{
			List<WeightedGender> possibleGenders = loadedGenders[species];
			float totalWeights = 0;
			foreach (WeightedGender gender in possibleGenders)
			{
				totalWeights += gender.weight;
			}
			float value = Random.value * totalWeights;
			for (int i = 0; i < possibleGenders.Count; i++)
			{
				if (value < possibleGenders[i].weight)
				{
					return possibleGenders[i].gender;
				}
				else
				{
					value -= possibleGenders[i].weight;
				}
			}
			return possibleGenders[possibleGenders.Count - 1].gender;
		}
	}
}