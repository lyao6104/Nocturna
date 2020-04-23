/* Name: L. Yao
 * Date: November 9, 2019
 * Desc: This script handles the citizen view of the game. It is meant to show a list of all living citizens and some info about them. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CitizenViewScript : MonoBehaviour
{
	public GameObject infoCardTemplate;
	public Transform contentTransform;

	public TMPro.TMP_Text citizenCountText;
	public int numCitizensEver;

	private GameControllerScript gc;
	private int livingCitizenCounter = 0;

	private void OnEnable()
	{
		if (gc == null)
		{
			gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
		}
		foreach (CitizenScript citizen in gc.citizens)
		{
			if (!citizen.isDead)
			{
				GameObject newInfoCard = Instantiate(infoCardTemplate, contentTransform);
				newInfoCard.GetComponent<InfocardScript>().me = citizen;
				livingCitizenCounter++;
			}
		}
		citizenCountText.text = "There are currently " + livingCitizenCounter + " citizens living in the town.\n\nTo date, there have been " + numCitizensEver + " citizens of the town in total.";
	}

	private void OnDisable()
	{
		// Get rid of all the generated infocards when disabled.
		foreach(Transform child in transform.GetComponentsInChildren<Transform>())
		{
			if (child.tag == "CitizenView")
			{
				Destroy(child.gameObject);
			}
		}
		livingCitizenCounter = 0;
	}
}
