/* Name: L. Yao
 * Date: November 9, 2019
 * Desc: A small script attached to infocards to show information about a citizen. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfocardScript : MonoBehaviour
{
	public CitizenScript me;

	public TMPro.TMP_Text textBox;
	public Image portrait, portraitBackground;

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		//yield return new WaitForEndOfFrame(); // me gets assigned by the object that instantiates this one, so we have to wait for it to do so.
		// As it turns out, Unity has already assigned the CitizenScript by the time this code runs, but I'm keeping this here in case that behaviour
		// is dependant on how fast your computer is.

		//Debug.Log("Infocard has initialized");

		portrait.sprite = me.portrait;
		bool validColour = ColorUtility.TryParseHtmlString(me.myGender.colourCode, out Color bgColour);
		if (validColour)
		{
			portraitBackground.color = bgColour;
		}
		textBox.text = "Name: " + me.myName + "\nSpecies: " + me.species + "\nProfession: " + me.myJob.jobName + "\nMoney: " + me.money + " Gold";
	}

	public void ViewDetails()
	{
		GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>().ToggleCitizenDetails(me);
	}
}
