/* Name: L. Yao
 * Date: February 26, 2020
 * Desc: This file contains definitions for common item types. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public Herb(Herb copyFrom) : base(copyFrom)
	{
		healPower = copyFrom.healPower;
	}
}

[System.Serializable]
public class FarmAnimal : Item
{
	public Item[] possibleProduce;

	public FarmAnimal(FarmAnimal copyFrom) : base(copyFrom)
	{
		possibleProduce = copyFrom.possibleProduce;
	}

	public Item MakeProduce()
	{
		Item toCreate = new Item(possibleProduce[Random.Range(0, possibleProduce.Length)]);
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
	public EquipmentSlot equipmentSlot;
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
public static class CommonItems
{
	// Common foods
	public static Item basicRation = new Item("Ration", "Rations", new List<string>() { "Food" }, "Well-preserved food stored in a can.", 1);

	// Common equipment
	public static Equippable basicTools = new Equippable("Basic Tools", "Basic Tools", new List<string>() { "Equippable", "Tool" }, EquipmentSlot.Tool, 2, 2, 25, "A set of basic tools usable by any profession. Breaks easily. +2 Job and Combat skill.", 3);
}