/* Date: May 18, 2021
 * Name: L. Yao
 * Desc: Collection of utilities for getting names. Ported from Nightscape (one of my unreleased projects).
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NamesLib
{
	// Citizens have a full name and a common name. This class is used to represent that.
	[System.Serializable]
	public class NocturnaName
	{
		public string fullName;
		public string commonName;
	}

	[System.Serializable]
	public class WeightedString
	{
		public string value;
		public int weight;
	}

	// Unity can't serialize nested Lists
	[System.Serializable]
	public class NameSegmentList
	{
		public string segmentType;
		public List<string> segments;

		public string this[int key]
		{
			get
			{
				return segments[key];
			}
			set
			{
				segments[key] = value;
			}
		}

		public int Count { get { return segments.Count; } }
	}

	[System.Serializable]
	public class NameList
	{
		public string listName; // This is mainly used for readability purposes.
		public List<NameSegmentList> nameSegments;
		public List<WeightedString> nameFormats;
	}

	public static class NamesUtil
	{
		private static Dictionary<string, NameList> loadedNamelists = new Dictionary<string, NameList>();

		public static void LoadNameList(NameList toLoad)
		{
			loadedNamelists.Add(toLoad.listName, toLoad);
			Debug.Log("Loaded NameList " + toLoad.listName);
		}

		public static NocturnaName GetName(string listName)
		{
			NameList nameList = loadedNamelists[listName];

			// Choose a format
			string chosenFormat = nameList.nameFormats[nameList.nameFormats.Count - 1].value;
			float formatTotalWeights = 0;
			foreach (WeightedString format in nameList.nameFormats)
			{
				formatTotalWeights += format.weight;
			}
			float formatValue = Random.value * formatTotalWeights;
			for (int i = 0; i < nameList.nameFormats.Count; i++)
			{
				if (formatValue < nameList.nameFormats[i].weight)
				{
					chosenFormat = nameList.nameFormats[i].value;
					break;
				}
				else
				{
					formatValue -= nameList.nameFormats[i].weight;
				}
			}

			// Generate a name according to the chosen format
			string generatedName = chosenFormat;
			string generatedCommonName = "Smitty";
			for (int i = 0; i < nameList.nameSegments.Count; i++)
			{
				string segmentCode = "{" + nameList.nameSegments[i].segmentType + "}";
				if (generatedName.Contains(segmentCode))
				{
					string generatedSegment = nameList.nameSegments[i][Random.Range(0, nameList.nameSegments[i].Count)];
					generatedName = generatedName.Replace(segmentCode, generatedSegment);
					// If the segment code contains the string "common", that segment is used as the common name.
					if (segmentCode.Contains("common"))
					{
						generatedCommonName = generatedSegment;
					}
				}
			}
			// The common name is assumed to be the same as the full name if no segments are marked.
			if (generatedCommonName == "Smitty")
			{
				generatedCommonName = generatedName;
			}

			return new NocturnaName { fullName = generatedName, commonName = generatedCommonName };
		}

		public static NocturnaName GetHumanName(string gender)
		{
			return GetName("HumanNames" + gender);
		}
	}
}
