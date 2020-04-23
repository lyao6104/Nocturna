using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
	// Start is called before the first frame update
	private void Start()
	{
		//SceneManager.UnloadSceneAsync("SimulationScene");
	}

	public void StartGame()
	{
		SceneManager.LoadScene("SimulationScene");
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
