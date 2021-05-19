/* Date: May 19, 2021
 * Name: L. Yao
 * Desc: Collection of utilities for dialogue.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitiesLib;

namespace DialogueLib
{
	public class ProfessionLocalizedDialogue
	{
		public string listName;
		public Dictionary<string, string> dialogue = new Dictionary<string, string>();
	}

	[System.Serializable]
	public class ProfessionDialogueJSON
	{
		public string listName;
		public List<StringPair> dialogue;
	}

	[System.Serializable]
	public class DialogueList
	{
		public string listName;
		public List<string> allowedSpecies;
		public List<string> allowedProfessions;
		public List<string> dialogue;
	}

	public static class DialogueUtil
	{
		private static Dictionary<string, DialogueList> loadedDialogueLists = new Dictionary<string, DialogueList>();
		private static Dictionary<string, ProfessionLocalizedDialogue> loadedProfessionDialogueLists = new Dictionary<string, ProfessionLocalizedDialogue>();

		public static void LoadDialogueList(DialogueList toLoad)
		{
			loadedDialogueLists.Add(toLoad.listName, toLoad);
			Debug.Log("Loaded DialogueList " + toLoad.listName);
		}

		public static void LoadDialogueList(ProfessionDialogueJSON toLoad)
		{
			ProfessionLocalizedDialogue loadedDialogue = new ProfessionLocalizedDialogue
			{
				listName = toLoad.listName
			};
			foreach (StringPair line in toLoad.dialogue)
			{
				loadedDialogue.dialogue[line.key] = line.val;
			}
			loadedProfessionDialogueLists.Add(toLoad.listName, loadedDialogue);
			Debug.Log("Loaded ProfessionLocalizedDialogue " + loadedDialogue.listName);
		}

		public static List<string> GetCitizenDialogue(CitizenScript citizen)
		{
			List<DialogueList> validDialogueLists = new List<DialogueList>();
			List<string> citizenDialogue = new List<string>();
			foreach (DialogueList dialogueList in loadedDialogueLists.Values)
			{
				bool validSpecies = dialogueList.allowedSpecies[0] == "Any" || dialogueList.allowedSpecies.Contains(citizen.species);
				bool validProfession = dialogueList.allowedProfessions[0] == "Any" || dialogueList.allowedProfessions.Contains(citizen.myJob.jobName);
				if (validSpecies && validProfession)
				{
					validDialogueLists.Add(dialogueList);
				}
			}
			foreach (DialogueList dialogueList in validDialogueLists)
			{
				foreach (string line in dialogueList.dialogue)
				{
					citizenDialogue.Add(line);
				}
			}
			return citizenDialogue;
		}

		public static string GetProfessionDialogueLine(string professionName, string key, string species)
		{
			return loadedProfessionDialogueLists[professionName + "LocalizedDialogue"].dialogue[key + "_" + species];
		}
	}
}