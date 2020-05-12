/* Name: L. Yao
 * Date: February 27, 2020
 * Desc: Farmers plant and harvest crops, and can have animals too? */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DateWeatherSeasonLib;

public class JobFarmer : ProfessionScript
{
	public List<FarmAnimal> possibleAnimals;
	public List<Item> possibleCrops;
	public Date harvestDate;
	public int minHarvestDays, minJobSkillToReduceGrowTime;

	private List<Item> currentlyGrowing;
	private List<FarmAnimal> animals;
	private List<Item> produce;
	private int maxAnimals;
	private int maxHarvestDays;
	//private int minFood = 3; // The farmer will try to keep at least this much food for themselves. -- Replaced by prefMinfood in CitizenScript

	protected override void Init()
	{
		base.Init();

		maxAnimals = Random.Range(1, 3);
		maxHarvestDays = minHarvestDays + 20;
		currentlyGrowing = new List<Item>();
		produce = new List<Item>();
		animals = new List<FarmAnimal>();
		me.GetPaid(Random.Range(5, 15), true); // Give farmers some gold to buy an animal since planting crops takes time.
	}

	public override void DoJob()
	{
		base.DoJob();

		// Look for relevant items in inventory
		foreach (Item item in me.inventory)
		{
			if (item.type.Contains("Farm Animal"))
			{
				animals.Add((FarmAnimal)item);
			}
			else if (item.type.Contains("Produce"))
			{
				produce.Add(item);
			}
		}

		// Buy animals if possible
		for (int i = animals.Count; i < maxAnimals; i++)
		{
			FarmAnimal toBuy = new FarmAnimal(possibleAnimals[Random.Range(0, possibleAnimals.Count)]);
			if (me.money >= toBuy.value + me.alwaysDoJobThreshold) // Make sure the farmer has enough money saved to buy the animal
			{
				gc.LogMessage(me.name + " has bought a " + toBuy.name + " for " + toBuy.value + " Gold.", "LGray");
				me.GetPaid(-toBuy.value, true);
				me.inventory.Add(toBuy);
				animals.Add(toBuy);
			}
		}

		// Get produce from animals
		foreach (FarmAnimal animal in animals)
		{
			int animalHarvestRoll = Random.Range(0, 100);
			if (animalHarvestRoll < me.GetEffectiveJobSkill() + 40) // Anyone with a jobSkill of at least 60 should always succeed at harvesting
			{
				Item newProduce = animal.MakeProduce();
				me.inventory.Add(newProduce);
				produce.Add(newProduce);
				gc.LogMessage(me.name + " got a " + newProduce.name + " from a " + animal.name + ".", "LGray");
			}
		}

		// Plant crops if nothing is growing already, and if it's either Spring or Summer, and NOT Droughtlike or Dry.
		if (currentlyGrowing.Count < 1 && (gc.curSeason == Season.Spring || gc.curSeason == Season.Summer) && gc.curWeather != Weather.Droughtlike && gc.curWeather != Weather.Dry)
		{
			Item cropToGrow = possibleCrops[Random.Range(0, possibleCrops.Count)];
			int cropsToGrow = Random.Range(1, Mathf.CeilToInt(me.GetEffectiveJobSkill() / 10f));
			for (int i = 0; i < cropsToGrow; i++)
			{
				currentlyGrowing.Add(new Item(cropToGrow));
			}
			me.Speak("Let's plant some " + cropToGrow.pluralName + " today.");

			// Time to grow is affected by jobSkill.
			int daysTillHarvest = Random.Range(minHarvestDays, maxHarvestDays+1);
			//Debug.Log("daysTillHarvest: " + daysTillHarvest);
			daysTillHarvest = Mathf.RoundToInt(daysTillHarvest * ((float)minJobSkillToReduceGrowTime / me.GetEffectiveJobSkill()));
			//Debug.Log("timeModifier: " + (float)minJobSkillToReduceGrowTime / me.GetEffectiveJobSkill());
			harvestDate = DateFuncs.GetFutureDate(gc.curDate, daysTillHarvest);
			//Debug.Log("Crops will be ready to harvest on " + DateFuncs.DateToString(harvestDate) + ", which is in " + daysTillHarvest + " days.");
		}

		// Crops get wiped if there's an ongoing drought.
		if (gc.curWeather == Weather.Droughtlike)
		{
			gc.LogMessage("The ongoing drought has withered away " + me.commonName + "'s crops.", "DRed");
			currentlyGrowing.Clear();
		}

		// Harvest if crops are ready
		if (DateFuncs.Equals(gc.curDate, harvestDate))
		{
			me.Speak("Ah, the harvest is in!");
			foreach (Item crop in currentlyGrowing)
			{
				Item toAdd = new Item(crop);
				me.inventory.Add(toAdd);
				produce.Add(toAdd);
			}
			gc.LogMessage(me.name + " has harvested " + currentlyGrowing.Count + " " + currentlyGrowing[0].pluralName + ".", "LGray");
			currentlyGrowing.Clear();
		}

		// Check how much food the farmer has
		int foodAmt = me.CountItemsByType("Food");
		//Debug.Log(me.myName + " has " + foodAmt + " units of food left.");

		// Sell up to 3 crops to market
		int cropsToSell = produce.Count;
		if (cropsToSell > 3)
		{
			cropsToSell = 3;
		}
		if (cropsToSell > 0)
		{
			int sellValue = 0;
			for (int i = 0; i < cropsToSell; i++)
			{
				if (produce[0].type.Contains("Food") && foodAmt <= me.prefMinFood && me.money >= me.alwaysDoJobThreshold)
				{
					break;
				}

				sellValue += produce[0].value;
				gc.LogMessage(me.name + " has sold a " + produce[0].name + ".", "LGray");
				if (produce[0].type.Contains("Food"))
				{
					foodAmt--;
				}
				gc.GetComponent<ShopScript>().SellToShop(produce[0]);
				me.inventory.Remove(produce[0]);
				produce.RemoveAt(0);
			}
			if (sellValue > 0)
			{
				me.Speak("Nice, I sold " + sellValue + " Gold worth of produce today.");
				me.GetPaid(sellValue, true);
			}
			else
			{
				me.Speak("I ain't got no produce to sell today.");
			}
			//Debug.Log(me.myName + " has " + foodAmt + " units of food left.");
		}
		else
		{
			me.Speak("I ain't got no produce to sell today.");
		}
		animals.Clear();
		produce.Clear();
	}
}
