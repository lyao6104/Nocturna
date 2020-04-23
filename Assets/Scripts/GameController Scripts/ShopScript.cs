/* Name: L. Yao
 * Date: February 27, 2020
 * Desc: The town shop is where citizens can buy things, and is also where all the items that people sell go to. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopScript : MonoBehaviour
{
	public List<Item> shopStocks; // The shop's inventory, essentially.
	public int maxItems; // How many items the shop can contain.

	private GameControllerScript gc;

	private void Start()
	{
		gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
	}

	public void SellToShop(Item toSell)
	{
		shopStocks.Add(toSell);
	}

	public void BuyFromShop(CitizenScript buyer, string itemType, int maxCost = int.MaxValue)
	{
		maxCost = Mathf.Clamp(maxCost, 0, buyer.money); // Make sure that the buyer can actually afford stuff
		// Pick an item to be bought
		List<Item> potentialProducts = new List<Item>();
		foreach (Item item in shopStocks)
		{
			if (item.type.Contains(itemType) && item.value <= maxCost)
			{
				potentialProducts.Add(item);
				//Debug.Log(item.name + " is a potential product for " + buyer.myName);
			}
		}
		if (potentialProducts.Count < 1) // For whatever reason the buyer can't buy anything
		{
			return;
		}
		//Debug.Log(buyer.myName + "'s potentialProducts: " + potentialProducts.Count);
		Item toBuy = potentialProducts[Random.Range(0, potentialProducts.Count)];
		// Add the item to the buyer's inventory
		buyer.inventory.Add(toBuy);
		shopStocks.Remove(toBuy);
		buyer.GetPaid(-toBuy.value, true);
		gc.LogMessage(buyer.myName + " has bought 1 " + toBuy.name + " from the shops for " + toBuy.value + " Gold.", "LGray");
		Debug.Log(buyer.myName + " has bought 1 " + toBuy.name + " from the shops for " + toBuy.value + " Gold.");
		potentialProducts.Clear();
	}

	public void ClearExcess(bool newestFirst = false)
	{
		if (shopStocks.Count <= maxItems)
		{
			return;
		}

		int numExcess = shopStocks.Count - maxItems;
		if (newestFirst)
		{
			shopStocks = shopStocks.GetRange(0, maxItems);
		}
		else
		{
			
			shopStocks.RemoveRange(0, numExcess);
		}
		Debug.Log("Clearing " + numExcess + " items from the shop");
	}
}
