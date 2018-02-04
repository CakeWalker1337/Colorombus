using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManagerScript : MonoBehaviour {

	private GameObject mainMenu;
	private GameObject optionsMenu;
	private GameObject pauseMenu;
	private GameObject gameOverMenu;
	private GameObject secondChanceMenu;
	/*
	 	-1 - None
		0 - Main
		1 - Options
		2 - Pause
		3 - Game Over
		4 - Second Chance
	*/
	private int currentMenu;


	void Start()
	{
		if (SceneManager.GetActiveScene ().name.Equals ("menu_scene")) 
		{
			if (GameController.NotInitialized())	
				GameController.InitGlobalSettings ();

			optionsMenu = GameObject.Find ("OptionsMenu");
			mainMenu = GameObject.Find ("MainMenu");

			Text text = GameObject.Find ("MusicButton_Text").GetComponent<Text> ();
			text.text = (GameController.Music == 1)?"Music: On":"Music: Off";

			text = GameObject.Find ("SoundsButton_Text").GetComponent<Text> ();
			text.text = (GameController.Sounds == 1)?"Sounds: On":"Sounds: Off";

			optionsMenu.SetActive (false);
			currentMenu = 0;
		} 
		else if (SceneManager.GetActiveScene ().name.Equals ("endless_level")) 
		{
			pauseMenu = GameObject.Find ("PauseMenu");
			gameOverMenu = GameObject.Find ("GameOverMenu");
			secondChanceMenu = GameObject.Find ("SecondChanceMenu");

			currentMenu = -1;
			pauseMenu.SetActive (false);
			gameOverMenu.SetActive (false);
			secondChanceMenu.SetActive (false);
		}
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			if (currentMenu == 0) {
				Application.Quit ();
				GameController.SaveUserData ();
			} else if (currentMenu == 1) {
				optionsMenu.SetActive (false);
				mainMenu.SetActive (true);
				currentMenu = 0;
			}
		} 
		else if (Input.GetKeyDown (KeyCode.Home) || Input.GetKeyDown(KeyCode.Menu)) 
		{
			GameController.SaveUserData ();
		}
	}

	// Main Menu
	public void PlayButton_Callback()
	{
		GameController.TryPlaySound (SoundPool.Button);
		SceneManager.LoadScene ("endless_level");

	}

	public void OptionsButton_Callback()
	{
		mainMenu.SetActive (false);
		optionsMenu.SetActive (true);
		GameController.TryPlaySound (SoundPool.Button);
	}

	public void ExitButton_Callback()
	{
		GameController.SaveUserData ();
		GameController.TryPlaySound (SoundPool.Button);
		Application.Quit();
	}
	///

	//Options Menu
	//
	public void SoundsButton_Callback()
	{
		GameController.SwitchSoundSettings ();
		Text text = GameObject.Find ("SoundsButton_Text").GetComponent<Text>();
		text.text = (GameController.Sounds == 0)?"Sounds: Off":"Sounds: On";

		GameController.TryPlaySound (SoundPool.Button);
	}

	public void MusicButton_Callback()
	{
		GameController.TryPlaySound (SoundPool.Button);

		GameController.SwitchMusicSettings ();
		GameController.CorrectMusic ();

		Text text = GameObject.Find ("MusicButton_Text").GetComponent<Text>();
		text.text = (GameController.Music == 0)?"Music: Off":"Music: On";

	}

	public void OptionsBackButton_Callback()
	{
		optionsMenu.SetActive (false);
		mainMenu.SetActive (true);
		GameController.TryPlaySound (SoundPool.Button);
	}
	///

	//Pause Menu
	//
	public void PauseMenuButton_Callback()
	{
		GameController.TryPlaySound (SoundPool.Button);
		pauseMenu.SetActive (true);
		currentMenu = 2;
	}

	public void PauseContinueButton_Callback()
	{
		GameController.TryPlaySound (SoundPool.Button);
		pauseMenu.SetActive (false);
		currentMenu = -1;
	}

	public void PauseHomeButton_Callback()
	{
		GameController.TryPlaySound (SoundPool.Button);
		SceneManager.LoadScene ("menu_scene");
		currentMenu = 0;
	}

	public void PauseSoundButton_Callback()
	{
		GameController.SwitchSoundSettings ();
		GameController.TryPlaySound (SoundPool.Button);
		//Change Sprite
	}

	public void PauseMusicButton_Callback()
	{
		GameController.SwitchMusicSettings ();
		GameController.CorrectMusic ();

		//GameObject.Find ("");
		//Change Sprite
	}
	///

	//Game Over Menu
	//

	public void GameOverRetryButton_Callback()
	{
		//RetryCode
	}

	public void GameOverRateButton_Callback()
	{
		//Open Rate URL
		//Application.OpenURL("URL");
	}

	public void GameOverHomeButton_Callback()
	{
		GameController.TryPlaySound (SoundPool.Button);
		SceneManager.LoadScene ("menu_scene");
		currentMenu = 0;
	}

	public void GameOverShopButton_Callback()
	{
		//Shop
	}
	///

	//Second Chance Menu
	//

	public void SecondChanceShowAdButton_Callback()
	{
		GameController.ShowAd ();
	}

	public void SecondChanceCloseButton_Callback()
	{
		secondChanceMenu.SetActive (false);

	}
	///

}
