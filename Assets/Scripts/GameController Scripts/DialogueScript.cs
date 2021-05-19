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
		// Load generic dialogue
		TextAsset[] generalDialogueFiles = Resources.LoadAll("Dialogue/Speaking", typeof(TextAsset)).Cast<TextAsset>().ToArray();
		foreach(TextAsset dialogueFile in generalDialogueFiles)
		{
			DialogueList generalDialogueList = JsonUtility.FromJson<DialogueList>(dialogueFile.text);
			DialogueUtil.LoadDialogueList(generalDialogueList);
		}
		// Load species-localized profession dialogue
		TextAsset[] professionDialogueFiles = Resources.LoadAll("Dialogue/Professions", typeof(TextAsset)).Cast<TextAsset>().ToArray();
		foreach (TextAsset dialogueFile in professionDialogueFiles)
		{
			ProfessionDialogueJSON professionDialogueList = JsonUtility.FromJson<ProfessionDialogueJSON>(dialogueFile.text);
			DialogueUtil.LoadDialogueList(professionDialogueList);
		}
	}
}
