/* Name: L. Yao
 * Date: November 5, 2019
 * Desc: A generic human citizen. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NamesLib;
using GendersLib;

public class HumanCitizen : CitizenScript
{
	protected override void Init()
	{
		myGender = GendersUtil.AssignGender(species);
		portrait = gc.GetComponent<PortraitScript>().GetPortrait("Human", myGender);
		NocturnaName nameData = NamesUtil.GetHumanName(myGender.name);
		myName = nameData.fullName;
		commonName = nameData.commonName;
		gameObject.name = myName;
		money = Random.Range(8, 13); // Humans start with between 8 and 12 gold.
		jobSkill = Random.Range(50, 71); // Humans aren't really bad at stuff, but won't really excel at anything either.
		combatSkill = Random.Range(50, 61);
		speakChance = Random.Range(60, 101); // Let's say humans are rather talkative here.
		doJobChance = Random.Range(60, 101); // How often a citizen does their job.
		GetJob();
		// Some values for Humans: taxRate = 15, minTax = 2, species = "Human"

		//inventory.Add(new Item("Test item", "Test items", "Test", "A test item."));

		gc.LogMessage("A human named " + name + " has moved into the town.", "LGray");

		base.Init();
	}

	public override void Upkeep()
	{
		validTargets = gc.citizens;
		target = gc.GetTarget(this);

		if (!isDead) 
		{
			int speakRoll = Random.Range(0, 100);
			if (speakRoll < speakChance)
			{
				Speak();
			}

			DoShopping();

			int jobRoll = Random.Range(0, 100);
			if (jobRoll < doJobChance || money <= alwaysDoJobThreshold)
			{
				myJob.DoJob();
			}
		}

		if (!isDead)
		{
			// Buy food if:
			// - Food is running low and the citizen has enough money saved up, OR
			// - The citizen has been starving for a while
			if ((CountItemsByType("Food") < prefMinFood && money > alwaysDoJobThreshold) || daysWithoutFood > daysBeforeStarvation / 2)
			{
				gc.GetComponent<ShopScript>().BuyFromShop(this, "Food", money - alwaysDoJobThreshold); // Citizens really shouldn't be wasting money on expensive food if they can't afford it.
			}
			Eat();
			ToolUpkeep();
			AutoEquipAll();
			PayTaxes();
			LogMoney();
		}
	}
}
