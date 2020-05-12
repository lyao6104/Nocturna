/* Name: L. Yao
 * Date: November 5, 2019
 * Desc: The base citizen class that is inherited by different kinds of citizens. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitiesLib;

[System.Serializable]
public class ValidTargetTypes
{
	public List<string> allowedSpecies, allowedProfessions;
	public int minWealth, maxWealth;
	public bool allowDead;
}

public class CitizenScript : MonoBehaviour
{
	public CitizenScript target;
	public List<CitizenScript> validTargets;
	public string myName, commonName, species, gender;
	public List<string> dialogue, possibleGenders; // Gender only affects portraits. Everyone is referred to as "they".
	// jobSkill ranges from 0 to 100 (inclusive), combatSkill ranges from 0 to 200 (inclusive), with 50 being considered average for both.
	// These values can all increase somehow over the course of a citizen's lifespan.
	// taxRate, minTax, validTargetTypes and species should all be assigned in the inspector.
	// validTargetTypes will probably be overwritten depending on the citizen's profession, however.
	public int money, jobSkill, combatSkill, speakChance, doJobChance, taxRate, minTax, alwaysDoJobThreshold;
	public bool isDead;
	public ValidTargetTypes validTargetTypes;
	public Sprite portrait; // Portrait for citizen view.
	public ProfessionScript myJob;
	public GameObject[] allowedJobsPrefabs;
	public List<Item> inventory;
	// desiredItemCounts is essentially a dictionary keeping track of how many items of a particular type this citizen should try to buy.
	// Don't put food in this; that's handled separately.
	public List<StringIntPair> desiredItemCounts; 
	public List<Equippable> currentlyEquipped;
	public int daysBeforeStarvation, prefMinFood;
	public float maxShoppingGoldPercentage; // Percentage of this citizen's savings that they will use on shopping.

	protected GameControllerScript gc;
	protected int daysWithoutFood = 0;
	// The idea here is that humanoid races will just use these values below, but any race that needs these to be changed can do so.
	protected Dictionary<EquipmentSlot, int> maxEquipment = new Dictionary<EquipmentSlot, int>() 
	{
		{EquipmentSlot.Head, 1},
		{EquipmentSlot.Shoulders, 1},
		{EquipmentSlot.Chest, 1},
		{EquipmentSlot.Hands, 1},
		{EquipmentSlot.Waist, 1},
		{EquipmentSlot.Legs, 1},
		{EquipmentSlot.Feet, 1},
		{EquipmentSlot.Jewellery, 3},
		{EquipmentSlot.Tool, 1},
		{EquipmentSlot.Weapon, 1}
	};

	private void Start()
	{
		gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
		//gc.citizens.Add(this);
		Init();
	}

	// Each citizen type will have its own initialization code.
	protected virtual void Init()
	{
		// Each citizen starts with three rations.
		for (int i = 0; i < 3; i++)
		{
			inventory.Add(new Item(CommonItems.basicRation));
		}
		// Each citizen starts out with a set of basic tools.
		inventory.Add(new Equippable(CommonItems.basicTools));

		// Testing stuff
		//List<string> toolType = new List<string>();
		//toolType.Add("Equippable");
		//Equippable bad = new Equippable("bad tool", "bad tools", toolType, EquipmentSlot.Tool, 0, 0, 50);
		//Equippable good = new Equippable("good tool", "good tools", toolType, EquipmentSlot.Tool, 5, 5);
		//inventory.Add(bad);
		//inventory.Add(good);
		//EquipItem(bad);
	}

	// Likewise, each citizen type will have its own upkeep code.
	public virtual void Upkeep()
	{

	}

	// Each citizen needs to eat. This function gets the first food item in the citizen's inventory and eats it.
	// If there is no food, a counter ticks up for each consecutive day that this citizen has no food.
	// If it reaches a certain number, the citizen dies.
	protected virtual void Eat()
	{
		foreach (Item item in inventory)
		{
			if (item.type.Contains("Food"))
			{
				gc.LogMessage(myName + " has eaten a " + item.name + ".", "Orange");
				inventory.Remove(item);
				daysWithoutFood = 0;
				return;
			}
		}
		if (daysBeforeStarvation < 1) // Assume that if daysBeforeStarvation is set to 0 or below, this citizen won't die from starvation.
		{
			return;
		}
		daysWithoutFood++;
		gc.LogMessage(myName + " is starving.", "Yellow");
		if (daysWithoutFood >= daysBeforeStarvation) 
		{
			gc.LogMessage(myName + " has died from starvation.", "DRed");
			Kill(true);
		}
	}

	// Get a random line of dialogue and add it to the event log, substituting in certain words into the dialogue (like names) where necessary.
	protected void Speak()
	{
		string myMsg = myName + ": " + dialogue[Random.Range(0, dialogue.Count)];
		myMsg = myMsg.Replace("%name%", myName);
		myMsg = myMsg.Replace("%commonName%", commonName);
		myMsg = myMsg.Replace("%targetName%", target.myName);
		myMsg = myMsg.Replace("%targetCommonName%", target.commonName);
		myMsg = myMsg.Replace("%season%", gc.curSeason.ToString().ToLower());
		myMsg = myMsg.Replace("%seasonCap%", gc.curSeason.ToString());
		myMsg = myMsg.Replace("%weather%", DateWeatherSeasonLib.ClimateFuncs.WeatherToString(gc.curWeather).ToLower());
		myMsg = myMsg.Replace("%weatherCap%", DateWeatherSeasonLib.ClimateFuncs.WeatherToString(gc.curWeather));

		gc.GetComponent<GameControllerScript>().LogMessage(myMsg, "Cyan");
	}

	// This version of speak allows for other things (other citizens, jobs, etc.) to make this citizen say something.
	public void Speak(string msg)
	{
		string myMsg = myName + ": " + msg;
		myMsg = myMsg.Replace("%name%", myName);
		myMsg = myMsg.Replace("%commonName%", commonName);
		myMsg = myMsg.Replace("%targetName%", target.myName);
		myMsg = myMsg.Replace("%targetCommonName%", target.commonName);
		myMsg = myMsg.Replace("%season%", gc.curSeason.ToString().ToLower());
		myMsg = myMsg.Replace("%seasonCap%", gc.curSeason.ToString());
		myMsg = myMsg.Replace("%weather%", DateWeatherSeasonLib.ClimateFuncs.WeatherToString(gc.curWeather).ToLower());
		myMsg = myMsg.Replace("%weatherCap%", DateWeatherSeasonLib.ClimateFuncs.WeatherToString(gc.curWeather));

		gc.GetComponent<GameControllerScript>().LogMessage(myMsg, "Cyan");
	}

	protected void LogMoney()
	{
		if (!isDead)
		{
			string msg = myName + " has " + money + " Gold left.";
			gc.LogMessage(msg, "White");
		}
	}

	// Mark this citizen as dead so that things that don't work on dead people (or things that dead people shouldn't do) aren't done on or by dead people.
	public virtual void Kill(bool suppressMsg = false)
	{
		if (!suppressMsg)
		{
			gc.LogMessage(myName + " has perished.", "DRed");
		}
		isDead = true;
	}

	// Delete this dead citizen and remove them from the town.
	public virtual void Bury()
	{
		gc.LogMessage(myName + " was buried in the town cemetery.", "LGray");
		gc.citizens.Remove(this);
		Destroy(gameObject);
	}

	// Each citizen must pay taxes each day, based on a percentage of their balance, but the amount they pay
	// must be at least minTax. Different kinds of citizens will probably have different taxRates.
	public void PayTaxes()
	{
		if (!isDead)
		{
			int toPay = Mathf.RoundToInt(money * ((float)taxRate / 100));
			if (toPay < minTax)
			{
				toPay = minTax;
			}
			if (money >= toPay)
			{
				gc.LogMessage(myName + " has paid " + toPay + " Gold in taxes.", "White");
				money -= toPay;
			}
			else
			{
				gc.LogMessage(myName + " has been executed for not paying taxes!", "DRed");
				Kill(true);
			}
		}
	}

	// This citizen gives gold to another
	public void PayPerson(CitizenScript recipient)
	{
		// TODO: Implement
	}

	// Citizen recieves gold, possibly from their profession. Can also be used to get rid of money if amt is negative.
	public void GetPaid(int amt, bool suppresMsg = false)
	{
		money += amt;
		if (!suppresMsg)
		{
			gc.LogMessage(myName + " has received " + amt + " Gold.", "White");
		}
	}

	// Pick a profession from allowed jobs and create a child object with that job script.
	// It's done this way because there isn't really a good way to actually copy a new component at runtime (by default, Unity passes by reference).
	protected void GetJob()
	{
		int i = Random.Range(0, allowedJobsPrefabs.Length);
		myJob = Instantiate(allowedJobsPrefabs[i], transform).GetComponent<ProfessionScript>();
	}

	// Looks through the citizen's inventory and returns the number of items that have a specific type.
	public int CountItemsByType(string tag)
	{
		int count = 0;
		foreach (Item item in inventory)
		{
			if (item.type.Contains(tag))
			{
				count++;
			}
		}
		return count;
	}

	// Looks through inventory and returns a list of items matching a particular tag.
	public List<Item> GetItemsByType(string[] tags)
	{
		List<Item> toReturn = new List<Item>();
		foreach (Item item in inventory)
		{
			foreach (string tag in tags)
			{
				if (item.type.Contains(tag))
				{
					toReturn.Add(item);
					break;
				}
			}
		}
		return toReturn;
	}

	public List<Equippable> GetEquippedBySlot(EquipmentSlot slot)
	{
		List<Equippable> equipped = new List<Equippable>();
		foreach (Equippable item in currentlyEquipped)
		{
			if (item.equipmentSlot == slot)
			{
				equipped.Add(item);
			}
		}
		return equipped;
	}

	public int GetMaxEquipment(EquipmentSlot slot)
	{
		return maxEquipment[slot];
	}

	public void FindValidTargets()
	{
		foreach (CitizenScript citizen in gc.citizens)
		{
			if (validTargetTypes.allowDead || !citizen.isDead)
			{
				if (validTargetTypes.allowedSpecies.Contains("Any") || validTargetTypes.allowedSpecies.Contains(citizen.species))
				{
					if (validTargetTypes.allowedProfessions.Contains("Any") || validTargetTypes.allowedProfessions.Contains(citizen.myJob.jobName))
					{
						if (validTargetTypes.minWealth <= citizen.money && (validTargetTypes.maxWealth >= citizen.money || validTargetTypes.maxWealth == 0))
						{
							if (!validTargets.Contains(citizen)) // Only add them if they aren't already there
							{
								validTargets.Add(citizen);
							}
							continue;
						}
					}
				}
			}
			// If this section is reached then the citizen must not be a valid target.
			if (validTargets.Contains(citizen))
			{
				validTargets.Remove(citizen);
			}
		}
	}

	public void UnequipItem(Equippable toUnequip)
	{
		if (!currentlyEquipped.Contains(toUnequip))
		{
			Debug.Log(toUnequip.name + " isn't equipped.");
			return;
		}

		currentlyEquipped.Remove(toUnequip);
		toUnequip.isEquipped = false;
		//Debug.Log(toUnequip.name + " was unequipped from " + myName + ".");
		gc.LogMessage(toUnequip.name + " was unequipped from " + myName + ".", "LGray");
	}

	public void EquipItem(Equippable toEquip)
	{
		if (!inventory.Contains(toEquip))
		{
			Debug.LogError(myName + " tried to equip \"" + toEquip.name + "\", which isn't in their inventory.");
			return;
		}
		else if (currentlyEquipped.Contains(toEquip))
		{
			Debug.Log(toEquip.name + " is already equipped.");
			return;
		}

		// Unequip the first equipped item of the same slot if maxEquipment is reached
		if (GetEquippedBySlot(toEquip.equipmentSlot).Count >= maxEquipment[toEquip.equipmentSlot])
		{
			UnequipItem(GetEquippedBySlot(toEquip.equipmentSlot)[0]);
		}

		currentlyEquipped.Add(toEquip);
		toEquip.isEquipped = true;
		//Debug.Log(toEquip.name + " was equipped on " + myName + ".");
		gc.LogMessage(toEquip.name + " was equipped on " + myName + ".", "LGray");
	}

	public void AutoEquip(EquipmentSlot slot)
	{
		List<Item> equippableItems = GetItemsByType(new string[] { "Equippable" });
		List<Equippable> potential = new List<Equippable>();
		foreach (Equippable item in equippableItems)
		{
			if (item.equipmentSlot == slot && !currentlyEquipped.Contains(item))
			{
				potential.Add(item);
			}
		}
		if (potential.Count < 1)
		{
			//Debug.Log(commonName + " has no items that go in the " + slot + " slot.");
			return;
		}

		Equippable bestItem = potential[0];
		for (int i = 1; i < potential.Count; i++)
		{
			int itemScore = potential[i].combatBonus + potential[i].jobBonus;
			if (itemScore > bestItem.combatBonus + bestItem.jobBonus)
			{
				bestItem = potential[i];
			}
		}

		List<Equippable> curItems = GetEquippedBySlot(slot);
		if (curItems.Count < maxEquipment[slot]) // If there's space to equip an item, just equip it
		{
			EquipItem(bestItem);
			return;
		}
		foreach (Equippable currentItem in curItems) // Otherwise, determine whether or not to keep currently equipped items
		{
			if (bestItem.combatBonus + bestItem.jobBonus > currentItem.combatBonus + currentItem.jobBonus)
			{
				UnequipItem(currentItem);
				EquipItem(bestItem);
				return;
			}
		}
		Debug.Log(bestItem.name + " wasn't good enough to be equipped");
	}

	// Run auto-equip on all slots
	public void AutoEquipAll()
	{
		foreach(KeyValuePair<EquipmentSlot, int> slot in maxEquipment)
		{
			for (int i = 0; i < slot.Value; i++)
			{
				AutoEquip(slot.Key);
			}
		}
	}

	public void ToolUpkeep()
	{
		// idea: maybe have maintenance kits that prevent tools from breaking?

		// Loop through equipped items and roll for those items breaking
		List<Equippable> brokenItems = new List<Equippable>();
		foreach (Equippable item in currentlyEquipped)
		{
			int roll = Random.Range(0, 100);
			if (roll < item.breakChance) // tool breaks
			{
				brokenItems.Add(item);
			}
		}
		foreach (Equippable item in brokenItems)
		{
			gc.LogMessage(commonName + "'s " + item.name + " has broken.", "Orange");
			currentlyEquipped.Remove(item);
			inventory.Remove(item); // The garbage collector should be able to clean up the rest after this point
		}
	}

	// Buy items if amounts in desiredItemCounts are not met
	public void DoShopping()
	{
		int goldToSpend = Mathf.RoundToInt(maxShoppingGoldPercentage * money);
		Debug.Log(myName + " will spend up to " + goldToSpend + " Gold on this shopping trip.");

		foreach (StringIntPair itemCount in desiredItemCounts)
		{
			// Get amount of items already owned
			int curCount = CountItemsByType(itemCount.key);

			// Don't buy anything if you don't have money to do so
			if (money <= alwaysDoJobThreshold)
			{
				break;
			}
			// Skip this item type if desired amount is already satisfied
			else if (curCount >= itemCount.val)
			{
				continue;
			}

			// Buy as many items as you can afford. Will run at least once, since it's assumed that the citizen has enough money for at least something if the
			// code has made it this far.
			for (int i = 0; i < itemCount.val - curCount; i++)
			{
				int moneyBefore = money;
				gc.GetComponent<ShopScript>().BuyFromShop(this, itemCount.key, goldToSpend);
				goldToSpend -= moneyBefore - money; // The cost of the item is calculated here.
				Debug.Log(myName + " has " + goldToSpend + " Gold left to spend");

				// Check if there's enough money left to buy more.
				if (goldToSpend < 1)
				{
					break;
				}
			}
		}
	}

	public int GetEffectiveJobSkill()
	{
		int skill = jobSkill;
		foreach (Equippable item in currentlyEquipped)
		{
			skill += item.jobBonus;
		}
		return skill;
	}

	public int GetEffectiveCombatSkill()
	{
		int skill = combatSkill;
		foreach (Equippable item in currentlyEquipped)
		{
			skill += item.combatBonus;
		}
		return skill;
	}
}
