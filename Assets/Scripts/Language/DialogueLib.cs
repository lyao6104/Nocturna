/* Date: May 19, 2021
 * Name: L. Yao
 * Desc: Collection of utilities for dialogue.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueLib
{
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
		private static Dictionary<string, DialogueList> loadedDialoguelists = new Dictionary<string, DialogueList>();

		public static void LoadDialogueList(DialogueList toLoad)
		{
			loadedDialoguelists.Add(toLoad.listName, toLoad);
			Debug.Log("Loaded DialogueList " + toLoad.listName);
		}

		public static List<string> GetCitizenDialogue(CitizenScript citizen)
		{
			List<DialogueList> validDialogueLists = new List<DialogueList>();
			List<string> citizenDialogue = new List<string>();
			foreach (DialogueList dialogueList in loadedDialoguelists.Values)
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
	}
}