/* Name: L. Yao
 * Date: March 4, 2020
 * Desc: Leatherworkers buy animal hides from the shop (presumably supplied by hunters), turn it into leather, and use that leather to create various items. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueLib;

/* Dialogue Keys
 * madeGoodItem: Created a very good item.
 * madeBadItem: Created a very bad item.
 * keepItem: Leatherworker keeps what they made.
 * sellItem: Leatherworker sells what they made.
 */

public class JobLeatherWorker : ProfessionScript
{
	public Item leatherProduct; // Assumed to be defined in the inspector
	public List<Equippable> possibleCreations; // What the leatherworker can make out of leather
	public int craftingCostS, craftingCostM, craftingCostL; // How much leather it takes to make something small, medium, or large.
	public int valuePerLeatherProduct; // The number of times each pelt's value can be divided by this number, is the amount of leather a pelt produces.

	private List<Item> currentlyTanning = new List<Item>(); // A list of hides that are currently tanning. Gets cleared at the start of each day.
	private Dictionary<EquipmentSlot, string> equipmentSizes = new Dictionary<EquipmentSlot, string>();
	private Dictionary<string, int> craftingCosts = new Dictionary<string, int>();

	protected override void Init()
	{
		base.Init();

		// Small items
		equipmentSizes[EquipmentSlot.Waist] = "S";
		equipmentSizes[EquipmentSlot.Shoulders] = "S";
		equipmentSizes[EquipmentSlot.Hands] = "S";

		// Medium items
		equipmentSizes[EquipmentSlot.Head] = "M";
		equipmentSizes[EquipmentSlot.Feet] = "M";

		// Large items
		equipmentSizes[EquipmentSlot.Chest] = "L";
		equipmentSizes[EquipmentSlot.Legs] = "L";
		// Leatherworkers cannot make Jewellery, Tools, or Weapons, so there is no need to add them here.

		// Set up crafting cost dictionary. The public variables are only used to set this up.
		craftingCosts["S"] = craftingCostS;
		craftingCosts["M"] = craftingCostM;
		craftingCosts["L"] = craftingCostL;

		// Each leatherworker starts with 5-10 leather pieces for balance reasons.
		int startingLeather = Random.Range(5, 11);
		for (int i = 0; i < startingLeather; i++)
		{
			me.inventory.Add(new Item(leatherProduct));
		}
	}

	public override void DoJob()
	{
		base.DoJob();

		int leatherCount = me.CountItemsByType("Leather");
		if (leatherCount <= craftingCosts["L"]) // Not much point in storing excessive amounts of leather. This should also save some (probably a miniscule amount but still) memory.
		{
			MakeLeather();
		}
		if (leatherCount > 0)
		{
			MakeProduct();
		}
	}
	
	// Leatherworkers make leather from animal hides. The amount they make depends on their skill, and they receive the finished product a day later.
	// IRL, this takes longer, but let's keep it simple here. 
	private void MakeLeather()
	{
		// First, empty the currentlyTanning list and receive leather.
		for (int i = 0; i < currentlyTanning.Count; i++)
		{
			me.inventory.Add(new Item(leatherProduct));
		}
		if (currentlyTanning.Count > 0)
		{
			gc.LogMessage(me.commonName + " gathers " + currentlyTanning.Count + " pieces of leather from the tanning racks.", "LGray");
		}
		currentlyTanning.Clear();

		// Then, determine how many hides this leatherworker can tan today.
		// Each leatherworker can tan at least 1 hide, with up to 4 additional hides based on their skill.
		int tanningAmt = 1 + Mathf.RoundToInt(4 * (me.GetEffectiveJobSkill() / 100f));
		List<Item> hides = me.GetItemsByType(new string[] { "Animal Hide" });
		if (tanningAmt > hides.Count)
		{
			tanningAmt = hides.Count;
		}
		for (int i = 0; i < tanningAmt; i++)
		{
			int numLeatherReceived = hides[i].value / valuePerLeatherProduct; // Should be integer division
			Mathf.Clamp(numLeatherReceived, 1, 3); // Each hide is worth between 1 and 3 leather.
			me.inventory.Remove(hides[i]);
			for(int j = 0; j < numLeatherReceived; j++)
			{
				currentlyTanning.Add(hides[i]);
			}
		}
		if (tanningAmt > 0)
		{
			gc.LogMessage(me.commonName + " sets out " + tanningAmt + " animal hides to tan.", "LGray");
		}
	}

	private void MakeProduct()
	{
		// Check whether or not there is enough leather to make at least a small object, and make an item using leather.
		int leatherCount = me.CountItemsByType("Leather");
		if (leatherCount < craftingCosts["S"])
		{
			return;
		}
		Equippable toMake = possibleCreations[Random.Range(0, possibleCreations.Count)];
		int leatherCost = craftingCosts[equipmentSizes[toMake.equipmentSlot]];
		while (leatherCount < leatherCost)
		{
			toMake = possibleCreations[Random.Range(0, possibleCreations.Count)];
			leatherCost = craftingCosts[equipmentSizes[toMake.equipmentSlot]];
		}

		// Remove leather from inventory
		List<Item> leatherToRemove = me.GetItemsByType(new string[] { "Leather" }).GetRange(0, leatherCost); // Get a list of leather objects to remove from the inventory
		foreach (Item item in leatherToRemove)
		{
			me.inventory.Remove(item);
		}

		// Determine the quality of the resulting item based on the worker's skill
		int lowerRange = 20, upperRange = 30;
		int skillModifier = Random.Range(me.GetEffectiveJobSkill() - lowerRange, me.GetEffectiveJobSkill() + upperRange) - 50;
		float skillMultiplier = 1f + skillModifier / 100f;
		// So, in effect, rolls < 50 result in a malus to quality, rolls > 50 result in a bonus.

		// Make the item and modify based on the skill multiplier
		Equippable newItem = new Equippable(toMake);
		if (newItem.jobBonus >= 0)
		{
			newItem.jobBonus = Mathf.RoundToInt(newItem.jobBonus * skillMultiplier);
		}
		else
		{
			newItem.jobBonus = Mathf.RoundToInt(newItem.jobBonus / skillMultiplier);
		}
		if (newItem.combatBonus >= 0)
		{
			newItem.combatBonus = Mathf.RoundToInt(newItem.combatBonus * skillMultiplier);
		}
		else
		{
			newItem.combatBonus = Mathf.RoundToInt(newItem.combatBonus / skillMultiplier);
		}
		newItem.value = Mathf.RoundToInt(newItem.value * skillMultiplier);
		int totalItemQuality = newItem.jobBonus + newItem.combatBonus + newItem.value;
		int baseQuality = toMake.jobBonus + toMake.combatBonus + toMake.value;

		gc.LogMessage(me.myName + " has created a " + newItem.name + ".", "LGray");

		// Modify description based on overall quality
		if (totalItemQuality > baseQuality)
		{
			newItem.desc += " This item was skillfully made, and is of a high quality.";
			me.Speak(DialogueUtil.GetProfessionDialogueLine(jobName, "madeGoodItem", me.species));
		}
		else if (totalItemQuality < baseQuality)
		{
			newItem.desc += " This item was crudely made, and is generally of a lower quality.";
			me.Speak(DialogueUtil.GetProfessionDialogueLine(jobName, "madeGoodItem", me.species));
		}
		if (newItem.jobBonus > 0)
		{
			newItem.desc += " +" + newItem.jobBonus + " Job skill.";
		}
		else if (newItem.jobBonus < 0)
		{
			newItem.desc += newItem.jobBonus + " Job skill.";
		}
		if (newItem.combatBonus > 0)
		{
			newItem.desc += " +" + newItem.combatBonus + " Combat skill.";
		}
		else if (newItem.combatBonus < 0)
		{
			newItem.desc += newItem.combatBonus + " Combat skill.";
		}


		// If the worker can equip the crafted item, they keep the item, but only if its jobBonus is higher than its combatBonus and they can afford to not sell it.
		if (me.money > me.alwaysDoJobThreshold && me.GetEquippedBySlot(newItem.equipmentSlot).Count < me.GetMaxEquipment(newItem.equipmentSlot) && newItem.jobBonus > newItem.combatBonus)
		{
			// Add finished item to worker's inventory.
			me.inventory.Add(newItem);
			me.Speak(newItem.LocalizeString(DialogueUtil.GetProfessionDialogueLine(jobName, "keepItem", me.species)));
			return;
		}

		// Otherwise, they should sell the item.
		float sellModifier = 1.2f + (Random.Range(0, me.GetEffectiveJobSkill()) - 50) / 100f; // Sell price should be higher here for balance reasons.
		me.Speak(newItem.LocalizeString(DialogueUtil.GetProfessionDialogueLine(jobName, "sellItem", me.species)));
		me.GetPaid(Mathf.RoundToInt(newItem.value * sellModifier), true);
		gc.GetComponent<ShopScript>().SellToShop(newItem);
		gc.LogMessage(me.myName + " has sold a " + newItem.name + " to the shops for " + newItem.value + " Gold.", "LGray");
	}
}
