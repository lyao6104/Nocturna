/* Name: L. Yao
 * Date: May 19, 2021
 * Desc: Loads dialogue into the program.
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DialogueLib;

public class DialogueScript : MonoBehaviour
{
	private void Start()
	{
		TextAsset[] dialogueFiles = Resources.LoadAll("Dialogue", typeof(TextAsset)).Cast<TextAsset>().ToArray();
		foreach(TextAsset dialogueFile in dialogueFiles)
		{
			// This file is just a list of dynamic dialogue tags, so it should be skipped.
			if (dialogueFile.name == "DynamicDialogueList")
			{
				continue;
			}
			// Load actual dialogue
			DialogueList dialogueList = JsonUtility.FromJson<DialogueList>(dialogueFile.text);
			DialogueUtil.LoadDialogueList(dialogueList);
		}
	}
}
