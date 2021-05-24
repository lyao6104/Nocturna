/* Name: L. Yao
 * Date: February 26, 2020
 * Desc: Script for viewing details about a citizen. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CitizenDetailsScript : MonoBehaviour
{
	public GameObject infoCardTemplate;
	public Transform contentTransform;

	public TMPro.TMP_Text citizenDetailsText;
	public TMPro.TMP_Text citizenNameText;
	public Image portrait, portraitBackground;
	public CitizenScript me;

	private GameControllerScript gc;
	

	private void OnEnable()
	{
		if (gc == null)
		{
			gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
		}
		foreach (Item item in me.inventory)
		{
			GameObject newInfoCard = Instantiate(infoCardTemplate, contentTransform);
			newInfoCard.GetComponent<ItemcardScript>().me = item;
		}
		citizenDetailsText.text = "Profession:\n" + me.myJob.jobName + "\n\nSpecies: " + me.species + "\n\nGold: " + me.money + " Gold";
		citizenNameText.text = me.myName;
		bool validColour = ColorUtility.TryParseHtmlString(me.myGender.colourCode, out Color bgColour);
		if (validColour)
		{
			portraitBackground.color = bgColour;
		}
		portrait.sprite = me.portrait;
	}

	private void OnDisable()
	{
		// Get rid of all the generated infocards when disabled.
		foreach (Transform child in transform.GetComponentsInChildren<Transform>())
		{
			if (child.tag == "CitizenView")
			{
				Destroy(child.gameObject);
			}
		}
	}
}
