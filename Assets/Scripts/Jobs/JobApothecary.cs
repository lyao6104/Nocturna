/* Name: L. Yao
 * Date: November 19, 2019
 * Desc: Apothecaries/herbalists gather herbs and use them to create medicine for money. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueLib;

/* Dialogue Keys
 * bagFull: Apothecary can't carry more herbs
 * searchSuccess: Apothecary found what they were looking for.
 * searchFailure: Apothecary didn't find the herb they were looking for.
 * noHerbs: Apothecary has no herbs to make potions with.
 */

public class JobApothecary : ProfessionScript
{
	public List<Herb> possibleHerbs;

	private List<Herb> herbs = new List<Herb>(); // Leftover from before citizens had inventories. Might optimize this at some point
	private int timesToGather = 0; // How many times this citizen can attempt to gather herbs;
	private int maxHerbs = 1;

	protected override void Init()
	{
		base.Init();

		// The amount of times this apothecary/herbalist can gather plants is determined by their jobskill, but is at least 1.
		timesToGather = me.GetEffectiveJobSkill() / 20; // e.g. if jobSkill is 55, timesToGather should be 2.
		if (timesToGather < 1)
		{
			timesToGather = 1;
		}
		// The max amount of herbs this citizen can carry is determined by jobSkill, but must be between 1 and 10.
		maxHerbs = Mathf.RoundToInt(me.GetEffectiveJobSkill() / 10);
		maxHerbs = Mathf.Clamp(maxHerbs, 1, 10);
	}

	public override void DoJob()
	{
		base.DoJob();

		int herbCount = 0;
		foreach (Item item in me.inventory)
		{
			if (item.type.Contains("Herb"))
			{
				herbCount++;
			}
		}

		// First, gather herbs
		for (int i = 0; i < timesToGather; i++)
		{
			if (herbCount >= maxHerbs)
			{
				me.Speak(DialogueUtil.GetProfessionDialogueLine(jobName, "bagFull", me.species));
				//Debug.Log("maxHerbs: " + maxHerbs + " -- inventorySize: " + me.inventory.Count + " -- herbCount: " + herbCount);
				break;
			}

			// Determine what the citizen is trying to find, then calculate success chance based on that. The more potent the herb, the rarer it is.
			Herb toFind = new Herb(possibleHerbs[Random.Range(0, possibleHerbs.Count)]);
			int successChance = 60;
			if (toFind.healPower > 20) // Herbs with a healpower > 20 are considered "rare" and are more difficult to find.
			{
				successChance -= toFind.healPower;
			}
			// Success chance is influenced by jobskill. jobSkill > 50 makes it easier to find things, < 50 makes it more difficult.
			successChance += Mathf.RoundToInt((me.GetEffectiveJobSkill() - 50) * 0.8f);

			gc.LogMessage(me.myName + " searches the woods for some " + toFind.name + ".", "LGray");

			// See if they're successful
			int searchRoll = Random.Range(0, 100);
			if (searchRoll < successChance)
			{
				me.Speak(DialogueUtil.GetProfessionDialogueLine(jobName, "searchSuccess", me.species));
				me.inventory.Add(toFind);
				herbCount++;
			}
			else
			{
				me.Speak(toFind.LocalizeString(DialogueUtil.GetProfessionDialogueLine(jobName, "searchFailure", me.species)));
			}
		}

		// If the citizen has at least 1 herb, a potion is made and the citizen gets paid. Potions can be made from up to timesToGather herbs.
		// Roll for # of herbs to use
		int herbsToUse = Random.Range(1, timesToGather+1);
		foreach (Item item in me.inventory)
		{
			if (item.type.Contains("Herb"))
			{
				herbsToUse--;
				herbs.Add((Herb)item);
			}
			if (herbsToUse < 1)
			{
				break;
			}
		}
		if (herbs.Count > 0)
		{
			// Make the potion
			float potionPower = 0;
			foreach (Herb herb in herbs)
			{
				potionPower += herb.healPower;
				gc.LogMessage(me.commonName + " adds a " + herb.name + " to the brew.", "LGray");
				me.inventory.Remove(herb);
				//Debug.Log("Added herb has healpower of " + herb.healPower);
				//herbs.RemoveAt(0);
			}
			// Add some additional healpower based on jobSkill
			int additionalPower = (me.GetEffectiveJobSkill() - 50) / 10;
			potionPower += additionalPower;

			// Base value of the potion is 3 gold, and increases as potionPower passes certain thresholds.
			int potionValue = 3;
			string potionType = "Crude";
			if (potionPower >= 15)
			{
				potionValue += 2;
				potionType = "Basic";
			}
			if (potionPower >= 25)
			{
				potionValue += 2;
				potionType = "Strong";
			}
			if (potionPower >= 40)
			{
				potionValue += 3;
				potionType = "Potent";
			}
			// A potent potion is worth 10 gold.
			//Debug.Log(me.myName + "'s Potion: \npotionPower " + potionPower + "\npotionValue " + potionValue + "\nadditionalPower: " + additionalPower);
			gc.LogMessage(me.myName + " has brewed a " + potionType + " Healing Potion, earning " + potionValue + " gold.", "LGray");
			me.GetPaid(potionValue, true);
		}
		else
		{
			me.Speak(DialogueUtil.GetProfessionDialogueLine(jobName, "noHerbs", me.species));
		}
		herbs.Clear();
	}
}
