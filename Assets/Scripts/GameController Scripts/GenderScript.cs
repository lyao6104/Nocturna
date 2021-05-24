/* Name: L. Yao
 * Date: May 24, 2021
 * Desc: This is where genders are loaded into the program.
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GendersLib;

public class GenderScript : MonoBehaviour
{
	private void Start()
	{
		TextAsset[] genderListFiles = Resources.LoadAll("Genders", typeof(TextAsset)).Cast<TextAsset>().ToArray();
		foreach (TextAsset file in genderListFiles)
		{
			SpeciesGenders genderList = JsonUtility.FromJson<SpeciesGenders>(file.text);
			GendersUtil.LoadSpeciesGenders(genderList);
		}
	}
}
