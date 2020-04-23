/* Name: L. Yao
 * Date: February 26, 2020
 * Desc: Modification of InfocardScript for Itemcards. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemcardScript : MonoBehaviour
{
	public Item me;
	public TMPro.TMP_Text basicInfo;
	public TMPro.TMP_Text description;
	public TMPro.TMP_Text myTypes;

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

		string types = "";
		for (int i = 0; i < me.type.Count-1; i++)
		{
			types += me.type[i] + ", ";
		}
		types += me.type[me.type.Count - 1];

		basicInfo.text = "Name: " + me.name + " - Value: " + me.value + " Gold";
		description.text = me.desc;
		myTypes.text = types;
		if (me.type.Contains("Equippable"))
		{
			Equippable meAsEquippable = (Equippable)me;
			Color color = GetComponent<Image>().color;
			if (meAsEquippable.isEquipped)
			{	
				color.r = 0.75f;
				color.g = 1.00f;
				color.b = 0.80f;
			}
			else
			{
				color.r = 1.00f;
				color.g = 0.65f;
				color.b = 0.50f;
			}
			GetComponent<Image>().color = color;
			//Debug.Log("Changing colour");
		}
	}
}
