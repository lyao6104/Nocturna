/* Name: L. Yao
 * Date: November 19, 2019
 * Desc: Hunters hunt various creatures for Gold, and might die depending on how dangerous that creature is and their skill. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueLib;

[System.Serializable]
public class Huntable
{
	public string name, noise;
	public int danger, minLoot, maxLoot;
	public List<Item> possibleLoot;

	/// <summary>
	/// Localize a given string, replacing all dynamic dialogue tags with the appropriate text.
	/// </summary>
	/// <param name="msg">A string containing dynamic dialogue tags to be replaced.</param>
	/// <returns>The localized message, as a string.</returns>
	public string LocalizeString(string msg)
	{
		msg = msg.Replace("%huntableName%", name);
		msg = msg.Replace("%huntableNoise%", string.Format("*{0}*", noise));
		return msg;
	}
}

/* Dialogue Keys
 * huntSuccess: Successful hunt.
 * huntFail1: Failed a hunt but didn't die.
 * huntFail2: Failed a hunt but didn't die.
 * sellLoot: Selling loot at market.
 */

public class JobHunter : ProfessionScript
{
	//public List<string> huntablesNames;
	//public List<int> huntablesDanger;
	//public List<string> huntablesNoise; // What noise a creature makes when it attacks the hunter.
	public List<Huntable> huntables;

	private List<Item> toSell = new List<Item>();

	protected override void Init()
	{
		base.Init();
	}

	public override void DoJob()
	{
		base.DoJob();

		// Pick a random huntable creature
		Huntable toHunt = huntables[Random.Range(0, huntables.Count)];
		//KeyValuePair<string, int> toHunt = new KeyValuePair<string, int>(toHunt.name, huntablesDanger[i]);

		gc.LogMessage(me.name + " is hunting a " + toHunt.name + ".", "LGray");
		int failThreshold = 20 + me.GetEffectiveJobSkill() - toHunt.danger; // Rolls of at least this number will result in a failure
		int deathThreshold = failThreshold + 40; // Rolls of at least this number result in a death.
		int huntRoll = Random.Range(0, 100); // Rolls from 0 to 99, so if failThreshold >= 100, it always succeeds.

		if (huntRoll < failThreshold) // Success
		{
			// OLD CODE
			/* // Hunter gets between 3 to 7 gold plus an amount determined by their jobSkill and how dangerous their prey was.
			int payoff = Random.Range(3, 8) + (me.GetEffectiveJobSkill() + toHunt.danger) / 15;
			gc.LogMessage(me.name + " has caught a " + toHunt.name + " worth " + payoff + " Gold.", "LGray");
			me.Speak("A fine catch.");
			me.GetPaid(payoff, true); */

			gc.LogMessage(me.myName + " has caught a " + toHunt.name + ".", "LGray");
			me.Speak(DialogueUtil.GetProfessionDialogueLine(jobName, "huntSuccess", me.species));
			// Loot the creature
			int lootAmt = Random.Range(toHunt.minLoot, toHunt.maxLoot + 1);
			for (int i = 0; i < lootAmt; i++)
			{
				Item loot = new Item(toHunt.possibleLoot[Random.Range(0, toHunt.possibleLoot.Count)]);
				gc.LogMessage(me.commonName + " receives a " + loot.name + " from the " + toHunt.name + ".", "LGray");
				me.inventory.Add(loot);
				if (loot.type.Contains("Food") && (me.CountItemsByType("Food") > me.prefMinFood || me.money <= me.alwaysDoJobThreshold)) // Sell excess food, or any food you receive if necessary.
				{
					toSell.Add(loot);
				}
			}

			// Sell hides and keep the rest.
			//int payoff = 0;
			foreach (Item item in me.inventory)
			{
				if (item.type.Contains("Animal Hide") || item.type.Contains("Cloth")) // Spiders might drop silk, which is cloth
				{
					//payoff += item.value;
					toSell.Add(item);
				}
			}
			int payoff = 0;
			foreach (Item item in toSell)
			{
				payoff += item.value;
				me.inventory.Remove(item);
				gc.GetComponent<ShopScript>().SellToShop(item);
			}
			if (payoff > 0)
			{
				me.Speak(DialogueUtil.GetProfessionDialogueLine(jobName, "sellLoot", me.species));
				me.GetPaid(payoff, true);
				gc.LogMessage(me.myName + " has sold pelts and meats worth " + payoff + " Gold.", "LGray");
			}
			toSell.Clear();
		}
		else if (huntRoll < deathThreshold) // Failure, but not a death
		{
			int msgRoll = Random.Range(0, 2);
			if (msgRoll == 0)
			{
				me.Speak(toHunt.LocalizeString(DialogueUtil.GetProfessionDialogueLine(jobName, "huntFail1", me.species)));
			}
			else
			{
				me.Speak(toHunt.LocalizeString(DialogueUtil.GetProfessionDialogueLine(jobName, "huntFail2", me.species)));
			}
		}
		else // Creature kills the hunter
		{
			gc.LogMessage(toHunt.LocalizeString("%huntableName%: %huntableNoise%"), "DGreen");
			gc.LogMessage("The " + toHunt.name + " has mortally wounded " + me.name + "!", "Yellow");
			me.Kill();
		}
	}
}
