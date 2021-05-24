/* Name: L. Yao
 * Date: May 19, 2021
 * Desc: This is where items are loaded into the program.
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemsScript : MonoBehaviour
{
	private void Start()
	{
		// Load item bundles
		TextAsset[] itemBundleTAs = Resources.LoadAll("Items/Bundles/Base", typeof(TextAsset)).Cast<TextAsset>().ToArray();
		foreach(TextAsset file in itemBundleTAs)
		{
			var bundle = JsonUtility.FromJson<ItemBundleBasic>(file.text);
			ItemsUtil.LoadBasicItemBundle(bundle);
		}
		itemBundleTAs = Resources.LoadAll("Items/Bundles/Equipment", typeof(TextAsset)).Cast<TextAsset>().ToArray();
		foreach (TextAsset file in itemBundleTAs)
		{
			var bundle = JsonUtility.FromJson<ItemBundleEquipabble>(file.text);
			ItemsUtil.LoadEquipmentBundle(bundle);
		}

		// Load individual items
		LoadItems<Item>("Items/Base");
		LoadItems<Equippable>("Items/Equipment");
		LoadItems<Herb>("Items/Herbs");
		LoadItems<FarmAnimal>("Items/FarmAnimals");
	}

	private void LoadItems<T>(string dir) where T : Item
	{
		TextAsset[] itemTextAssets = Resources.LoadAll(dir, typeof(TextAsset)).Cast<TextAsset>().ToArray();
		foreach (TextAsset itemFile in itemTextAssets)
		{
			T item = JsonUtility.FromJson<T>(itemFile.text);
			ItemsUtil.LoadItem(item);
		}
		Debug.Log(string.Format("Loaded {0} {1}s.", itemTextAssets.Length, typeof(T)));
	}
}
