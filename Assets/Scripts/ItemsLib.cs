/* Name: L. Yao
 * Date: February 26, 2020
 * Desc: This file contains definitions for common item types. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A grouping of related basic items. Mainly for convenience when writing item JSON files.
/// </summary>
[System.Serializable]
public class ItemBundleBasic
{
	public string bundleName; // For readability
	public List<Item> items;
}

/// <summary>
/// A grouping of related equippable items. Mainly for convenience when writing item JSON files.
/// </summary>
[System.Serializable]
public class ItemBundleEquipabble
{
	public string bundleName;
	public List<Equippable> items;
}

[System.Serializable]
public class Item
{
	public string name, pluralName, desc;
	public List<string> type;
	public int value;

	public Item(Item copyFrom)
	{
		name = copyFrom.name;
		pluralName = copyFrom.pluralName;
		type = copyFrom.type;
		desc = copyFrom.desc;
		value = copyFrom.value;
	}

	public Item(string name, string pluralName, List<string> type, string desc = "", int value = 0)
	{
		this.name = name;
		this.pluralName = pluralName;
		this.type = type;
		this.desc = desc;
		this.value = value;
	}

	public bool HasType(string tag)
	{
		return type.Contains(tag);
	}

	/// <summary>
	/// Localize a given string, replacing all dynamic dialogue tags with the appropriate text.
	/// Can be overridden by individual item types.
	/// </summary>
	/// <param name="msg">A string containing dynamic dialogue tags to be replaced.</param>
	/// <returns>The localized message, as a string.</returns>
	public virtual string LocalizeString(string msg)
	{
		msg = msg.Replace("%itemName%", name);
		msg = msg.Replace("%itemNamePlural%", pluralName);
		msg = msg.Replace("%itemDesc%", desc);
		msg = msg.Replace("%itemValue%", value.ToString());
		return msg;
	}
}

[System.Serializable]
public class Herb : Item
{
	public int healPower;
	public int gatherWeight;

	public Herb(Herb copyFrom) : base(copyFrom)
	{
		healPower = copyFrom.healPower;
		gatherWeight = copyFrom.gatherWeight;
	}
}

[System.Serializable]
public class FarmAnimal : Item
{
	public string[] possibleProduce;

	public FarmAnimal(FarmAnimal copyFrom) : base(copyFrom)
	{
		possibleProduce = copyFrom.possibleProduce;
	}

	public Item MakeProduce()
	{
		Item toCreate = new Item(ItemsUtil.GetBasicItem(possibleProduce[Random.Range(0, possibleProduce.Length)]));
		return toCreate;
	}
}

[System.Serializable]
public enum EquipmentSlot { Head, Shoulders, Chest, Hands, Waist, Legs, Feet, Jewellery, Tool, Weapon }

[System.Serializable]
public class Equippable : Item
{
	// breakChance should probably be pretty low since tools have a chance of breaking each day
	public int combatBonus, jobBonus, breakChance;
	public EquipmentSlot equipmentSlot; // In JSON files, this needs to be an integer to be deserialized properly.
	public bool isEquipped;

	public Equippable(Equippable copyFrom) : base(copyFrom)
	{
		combatBonus = copyFrom.combatBonus;
		jobBonus = copyFrom.jobBonus;
		equipmentSlot = copyFrom.equipmentSlot;
		breakChance = copyFrom.breakChance;
		isEquipped = false;
	}

	public Equippable(string name, string pluralName, List<string> type, EquipmentSlot slot, int cBonus = 0, int jBonus = 0, int bChance = 0, string desc = "", int value = 0) : base(name, pluralName, type, desc, value)
	{
		combatBonus = cBonus;
		jobBonus = jBonus;
		breakChance = bChance;
		isEquipped = false;
	}
}

// This is just a collection of various items that might be useful.
// Use JSON instead.
/*
public static class CommonItems
{
	// Common foods
	public static Item basicRation = new Item("Ration", "Rations", new List<string>() { "Food" }, "Well-preserved food stored in a can.", 1);

	// Common equipment
	public static Equippable basicTools = new Equippable("Basic Tools", "Basic Tools", new List<string>() { "Equippable", "Tool" }, EquipmentSlot.Tool, 2, 2, 25, "A set of basic tools usable by any profession. Breaks easily. +2 Job and Combat skill.", 3);
}
*/

public static class ItemsUtil
{
	private static Dictionary<string, Item> loadedItemTemplates = new Dictionary<string, Item>();
	private static Dictionary<string, List<Item>> savedItemCollections = new Dictionary<string, List<Item>>();
	private static List<FarmAnimal> farmAnimals = new List<FarmAnimal>();
	private static List<Herb> herbs = new List<Herb>();

	private static float herbsTotalGatherWeight = 0;

	/// <summary>
	/// Creates and saves a collection of items that are of the given types.
	/// This will loop over every stored item, so use it sparingly.
	/// </summary>
	/// <param name="types">A list of tags that each Item in the collection must have.</param>
	/// <param name="collectionName">A label for the new collection.</param>
	/// <param name="overwrite">Whether or not to overwrite any existing collections with the given name.</param>
	/// <returns>The created list of Items.</returns>
	public static List<Item> CreateItemCollection(string[] types, string collectionName, bool overwrite = false)
	{
		if (!overwrite && savedItemCollections.ContainsKey(collectionName))
		{
			return savedItemCollections[collectionName];
		}

		List<Item> newCollection = new List<Item>();
		foreach (Item item in loadedItemTemplates.Values)
		{
			bool valid = true;
			foreach(string type in types)
			{
				if (!item.HasType(type))
				{
					valid = false;
					break;
				}
			}
			if (valid)
			{
				newCollection.Add(item);
			}
		}
		savedItemCollections[collectionName] = newCollection;
		return newCollection;
	}

	/// <summary>
	/// Retrieves an existing collection of items with the given name.
	/// Create collections with <c>CreateItemCollection()</c>.
	/// </summary>
	/// <param name="collectionName">The name of the saved list.</param>
	/// <returns>The list with the given name, if it exists.</returns>
	public static List<Item> GetItemCollection(string collectionName)
	{
		return savedItemCollections[collectionName];
	}

	public static void LoadBasicItemBundle(ItemBundleBasic toLoad)
	{
		foreach (Item item in toLoad.items)
		{
			LoadItem(item);
		}
	}

	public static void LoadEquipmentBundle(ItemBundleEquipabble toLoad)
	{
		foreach (Equippable item in toLoad.items)
		{
			LoadItem(item);
		}
	}

	public static void LoadItem(Item toLoad)
	{
		loadedItemTemplates.Add(toLoad.name, toLoad);
		if (toLoad.HasType("Farm Animal"))
		{
			farmAnimals.Add((FarmAnimal)toLoad);
		}
		if (toLoad.HasType("Herb"))
		{
			herbs.Add((Herb)toLoad);
			herbsTotalGatherWeight += ((Herb)toLoad).gatherWeight;
		}
	}

	public static Item GetBasicItem(string name)
	{
		return loadedItemTemplates[name];
	}

	public static Herb GetHerb(string name)
	{
		return (Herb)loadedItemTemplates[name];
	}

	public static Herb GetRandomHerb()
	{
		float value = Random.value * herbsTotalGatherWeight;
		for (int i = 0; i < herbs.Count; i++)
		{
			if (value < herbs[i].gatherWeight)
			{
				return herbs[i];
			}
			else
			{
				value -= herbs[i].gatherWeight;
			}
		}
		return herbs[herbs.Count - 1];
	}

	public static FarmAnimal GetFarmAnimal(string name)
	{
		return (FarmAnimal)loadedItemTemplates[name];
	}

	public static FarmAnimal GetRandomFarmAnimal()
	{
		return farmAnimals[Random.Range(0, farmAnimals.Count)];
	}

	public static Equippable GetEquipmentItem(string name)
	{
		return (Equippable)loadedItemTemplates[name];
	}
}