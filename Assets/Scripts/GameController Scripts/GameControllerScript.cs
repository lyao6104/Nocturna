/* Name: L. Yao
 * Date: November 4, 2019
 * Desc: Handles all town-related stuff as well as things related to the UI and simulation */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DateWeatherSeasonLib;

[System.Serializable]
public class InfoBar
{
	public GameObject DateLabel, WeatherSeasonLabel;
}

public class GameControllerScript : MonoBehaviour
{
	public GameObject pauseMenu, eventLog;
	public GameObject evtLogTxtTemplate;
	public List<string> citizenTypesKey = new List<string>(); // Make sure this is the same size as the values list
	public List<GameObject> citizenTypesVal = new List<GameObject>();
	public List<CitizenScript> citizens = new List<CitizenScript>();
	public int maxCitizens;
	public InfoBar infoBar;
	public int maxEvtLogMessages;

	public bool isPaused, scrollToBottomOnNewDay;
	public Date curDate;
	public Weather curWeather;
	public Season curSeason;

	public Selectable[] disableOnPauseUI;
	public GameObject defaultUIGroup, citizenViewUIGroup, citizenDetailViewUIGroup;
	public GameObject curDayTextObject; // The TMP object containing the current day's events.

	private Dictionary<string, string> colourTable = new Dictionary<string, string>();
	private Dictionary<string, GameObject> citizenTypes = new Dictionary<string, GameObject>();
	private string dayTextBuffer = "";
	private bool simulating = false;
	private bool hasDeadCitizens = false;

	// Start is called before the first frame update
	private void Start()
	{
		Application.targetFrameRate = 60;

		// We need to initialize a text object first so things actually show up in the event log. This is also done at the end of each day to reduce clutter.
		GameObject newDay = Instantiate(evtLogTxtTemplate, eventLog.transform);
		curDayTextObject = newDay;
		curDayTextObject.GetComponent<TMPro.TMP_Text>().SetText("");

		// There's a weird bug that if you exit and re-open the simulation, days don't pass properly until you pause and unpause.
		// To avoid that issue, I'm just going to pause and unpause once when the simulation starts.
		PauseUnpause(true);
		PauseUnpause(true);
		
		
		if (!isPaused)
		{
			pauseMenu.SetActive(false);
		}
		else
		{
			pauseMenu.SetActive(true);
		}

		// Set up the dictionary of citizen types
		for (int i = 0; i < citizenTypesKey.Count; i++)
		{
			citizenTypes[citizenTypesKey[i]] = citizenTypesVal[i];
		}

		UpdateInfoBar();
		InitializeColourTable();

		// Populate the town up to maxCitizens
		for (int i = 0; i < maxCitizens; i++)
		{
			citizens.Add(SpawnCitizen());
		}
		TestingStuff();
		StartCoroutine(EndDay());
	}

	// The contents of this function should be empty unless I'm testing things.
	private void TestingStuff()
	{
		//Item legs = new Item("cool leg", "cool legs", new List<string>() { "Debug" }, "cool leg", 3);
		//for (int i = 0; i < 10; i++)
		//{
		//	GetComponent<ShopScript>().SellToShop(new Item(legs));
		//}
	}

	// Update is called once per frame
	private void Update()
	{
		if (Input.GetButtonDown("Cancel"))
		{
			PauseUnpause();
		}
	}

	// Add colours to the colourTable for use elsewhere.
	private void InitializeColourTable()
	{
		colourTable["White"] = "#ffffff"; // Default
		colourTable["Black"] = "#000000";
		colourTable["LGray"] = "#cccccc"; // System messages and actions
		colourTable["Red"] = "#ff0000"; 
		colourTable["DRed"] = "#881100"; // "Bad" things
		colourTable["Blue"] = "#0000ff"; // End of day
		colourTable["Cyan"] = "#42e3e0"; // Human speech
		colourTable["Green"] = "#00ff00";
		colourTable["DGreen"] = "#004c00"; // Dialogue for wild animals/hostile creatures
		colourTable["Yellow"] = "#ffea04"; // Warnings
		colourTable["Orange"] = "#ff9000"; // Consumption of resources/tools
		colourTable["Peach"] = "#ffcd69"; // New day
	}

	public void UpdateInfoBar()
	{
		// Change the label to match the current date
		infoBar.DateLabel.GetComponent<TMPro.TMP_Text>().SetText(DateFuncs.DateToString(curDate));
		// Change the season and weather label
		string weatherSeasonLabel = string.Format("{0} - {1}", curSeason, ClimateFuncs.WeatherToString(curWeather));
		infoBar.WeatherSeasonLabel.GetComponent<TMPro.TMP_Text>().SetText(weatherSeasonLabel);
	}

	// When the game is paused (either by pressing the menu button or whatever the user bound the Cancel key to),
	// The simulation stops and the user is unable to interact with certain things.
	public void PauseUnpause(bool hideMsg = false)
	{
		if (isPaused)
		{
			pauseMenu.SetActive(false);
			isPaused = false;
			Time.timeScale = 1;
			if (!simulating)
			{
				foreach (Selectable sel in disableOnPauseUI)
				{
					sel.interactable = true;
				}
			}
			if (!hideMsg)
			{
				LogMessage("Game unpaused.", "LGray");
			}
			
		}
		else
		{
			pauseMenu.SetActive(true);
			isPaused = true;
			Time.timeScale = 0;
			// So the user can't press any buttons they shouldn't be able to while they're in the menu.
			foreach (Selectable sel in disableOnPauseUI)
			{
				sel.interactable = false;
			}
			if (!hideMsg)
			{
				LogMessage("Game paused.", "LGray");
			}
		}
	}

	public void ToggleCitizenView()
	{
		if (defaultUIGroup.activeInHierarchy)
		{
			defaultUIGroup.SetActive(false);
			citizenViewUIGroup.SetActive(true);
		}
		else
		{
			defaultUIGroup.SetActive(true);
			citizenViewUIGroup.SetActive(false);
		}
	}

	public void ToggleCitizenDetails(CitizenScript me)
	{
		if (citizenViewUIGroup.activeInHierarchy)
		{
			citizenViewUIGroup.SetActive(false);
			citizenDetailViewUIGroup.GetComponent<CitizenDetailsScript>().me = me;
			citizenDetailViewUIGroup.SetActive(true);
		}
		else
		{
			citizenViewUIGroup.SetActive(true);
			citizenDetailViewUIGroup.GetComponent<CitizenDetailsScript>().me = null;
			citizenDetailViewUIGroup.SetActive(false);
		}
	}
	public void ToggleCitizenDetails() // For returning to citizen view
	{
		citizenViewUIGroup.SetActive(true);
		citizenDetailViewUIGroup.GetComponent<CitizenDetailsScript>().me = null;
		citizenDetailViewUIGroup.SetActive(false);
	}

	/// <summary>
	/// Used by other scripts to flag that there has been a death.
	/// </summary>
	public void NotifyDeath()
	{
		hasDeadCitizens = true;
	}

	// Output a message of a colour found in colourTable to the event log
	public void LogMessage(string msg, string colour, bool useColourTable = true)
	{
		string actualColour = colour;
		if (useColourTable)
		{
			actualColour = colourTable[colour];
		}
		//TMPro.TMP_Text msgComponent = curDayTextObject.GetComponent<TMPro.TMP_Text>();
		dayTextBuffer += string.Format("<color={1}>{0}</color>\n", msg, actualColour);
	}

	// Output a blank line to the event log
	public void LogSpace()
	{
		//TMPro.TMP_Text msgComponent = curDayTextObject.GetComponent<TMPro.TMP_Text>();
		dayTextBuffer += "\n";
	}

	// Create a citizen object and add its CitizenScript to the town.
	private CitizenScript SpawnCitizen(string type = "random")
	{
		citizenViewUIGroup.GetComponent<CitizenViewScript>().numCitizensEver++;

		if (type != "random")
		{
			GameObject newCitizen = Instantiate(citizenTypes[type]);
			return newCitizen.GetComponent<CitizenScript>();
		}
		// Create a random citizen if a specific one isn't asked for
		else
		{
			// List containing all the citizen prefabs
			List<GameObject> prefabsTemp = new List<GameObject>();
			foreach (GameObject obj in citizenTypes.Values)
			{
				prefabsTemp.Add(obj);
			}
			GameObject newCitizen = Instantiate(prefabsTemp[Random.Range(0, prefabsTemp.Count)]);
			return newCitizen.GetComponent<CitizenScript>();
		}
	}

	// Get a valid target for a citizen.
	public CitizenScript GetTarget(CitizenScript me)
	{
		CitizenScript target;
		bool isValid = false;
		do
		{
			target = citizens[Random.Range(0, citizens.Count)];
			if (me.validTargets.Contains(target))
			{
				isValid = true;
			}
			else
			{
				isValid = false;
			}
		} while (target == me || !isValid);
		return target;
	}

	// Runs at the end of each day
	public IEnumerator EndDay(float waitTime = 0f)
	{
		yield return new WaitForSeconds(waitTime);

		yield return new WaitForEndOfFrame(); // All of these WaitForEndOfFrames are so that the LogSpaces show up properly.

		curDate = DateFuncs.NextDay(curDate);
		curWeather = ClimateFuncs.GetNextWeather(curWeather, curDate);
		curSeason = ClimateFuncs.GetSeason(curDate);

		LogSpace();

		// Get rid of dead citizens
		if (hasDeadCitizens)
		{
			bool buriedSomeone = false;
			for (int i = 0; i < citizens.Count; i++)
			{
				if (citizens[i].isDead)
				{
					citizens[i].Bury();
					buriedSomeone = true;
					i--;
				}
			}
			if (buriedSomeone)
			{
				LogSpace();
			}
			hasDeadCitizens = false;
		}

		// A new citizen moves in each day if there's space.
		if (citizens.Count < maxCitizens)
		{
			citizens.Add(SpawnCitizen());
			yield return new WaitForEndOfFrame();
			LogSpace();
		}

		yield return new WaitForEndOfFrame();

		UpdateInfoBar();
		LogMessage("A new day has begun.\n ", "Peach");

		// Do upkeep for living citizens
		foreach (CitizenScript citizen in citizens)
		{
			if (!citizen.isDead)
			{
				citizen.Upkeep();
				LogSpace();
			}
		}
		LogMessage("The day has ended.", "Blue");
		
		TMPro.TMP_Text msgComponent = curDayTextObject.GetComponent<TMPro.TMP_Text>();
		msgComponent.text = dayTextBuffer;
		dayTextBuffer = "";

		// Clear excess items from shop, oldest items go first.
		GetComponent<ShopScript>().ClearExcess();

		// The responsible thing to do would be clearing old event messages after a certain point,
		// otherwise it will quickly eat up all of the computer's memory.
		int excessMessages = eventLog.transform.childCount - maxEvtLogMessages;
		for (int i = 0; i < excessMessages; i++)
		{
			if (eventLog.transform.childCount > maxEvtLogMessages)
			{
				Destroy(eventLog.transform.GetChild(i).gameObject);
			}
		}

		GameObject newDay = Instantiate(evtLogTxtTemplate, eventLog.transform);
		curDayTextObject = newDay;
		curDayTextObject.GetComponent<TMPro.TMP_Text>().SetText("");

		Canvas.ForceUpdateCanvases();
		if (scrollToBottomOnNewDay)
		{
			eventLog.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0;
		}
	}

	// This function simulates a certain number of days, one after the other.
	public void SimDays(int numDays)
	{
		for (int i = 0; i < numDays; i++)
		{
			StartCoroutine(EndDay(i / 4f)); // This should give each EndDay enough time to finish before the next one starts.
		}
		StartCoroutine(DisableUIForSeconds(numDays / 4f)); // Disallow user from interacting with certain UI elements until finished simulating.
	}

	// Finds the number of days it would take to reach the current day one month from now, and simulates that many days.
	public void SimMonth()
	{
		Date futureDate = DateFuncs.GetFutureDate(curDate, 0, 0, 1);
		SimDays(DateFuncs.DaysUntil(curDate, futureDate));
	}

	// Disables UI elements in disableOnPauseUI for a certain amount of time.
	private IEnumerator DisableUIForSeconds(float duration)
	{
		foreach (Selectable sel in disableOnPauseUI)
		{
			sel.interactable = false;
		}
		simulating = true;
		yield return new WaitForSeconds(duration);
		simulating = false;
		foreach (Selectable sel in disableOnPauseUI)
		{
			sel.interactable = true;
		}
	}

	public void ExitToMenu(bool doSave = false)
	{
		// Load menu scene, save if the user wants to
		//StopAllCoroutines();
		SceneManager.LoadScene("MainMenuScene");
	}
}
