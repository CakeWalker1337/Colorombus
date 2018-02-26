﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// User interface manager script.
/// </summary>
public class UIManagerScript : MonoBehaviour {


	private GameObject mainMenu;
	private GameObject optionsMenu;
	private GameObject pauseMenu;
	private GameObject gameOverMenu;
	private GameObject secondChanceMenu;
	private GameObject creditsMenu;
	private GameObject shopMenu;
	private GameObject infoMenu;
	private GameObject howToPlayMenu;

	private GameObject adError;

	private List<GameObject> howToPlayPages;
	/*
	 	-1 - None
		0 - Main
		1 - Options
		2 - Pause
		3 - Game Over
		4 - Second Chance
		5 - Credits
		6 - Shop
		7 - Info
		8 - How to Play
	*/
	private static int currentMenu;
	private static int currentPage;
	private bool isReturnerRun = false;
	private bool exitRequested = false;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start(){
		if (SceneManager.GetActiveScene ().name.Equals ("menu_scene")) {
			if (!GameController.IsInitialized())	
				GameController.InitGlobalSettings ();

			optionsMenu = GameObject.Find ("OptionsMenu");
			mainMenu = GameObject.Find ("MainMenu");
			creditsMenu = GameObject.Find ("CreditsMenu");
			shopMenu = GameObject.Find ("ShopMenu");
			infoMenu = GameObject.Find ("InfoMenu");

			Image img = GameObject.Find("MusicButtonImage").GetComponent<Image>();
			img.sprite = (GameController.Music == 1)? Dump.musicOnSprite : Dump.musicOffSprite;

			img = GameObject.Find("SoundButtonImage").GetComponent<Image>();
			img.sprite = (GameController.Sound == 1)? Dump.soundOnSprite : Dump.soundOffSprite;

			img = GameObject.Find("EffectsButtonImage").GetComponent<Image>();
			img.sprite = (GameController.Effects == 1)? Dump.effectsOnSprite : Dump.effectsOffSprite;

			img = (Image) GameObject.Find("BlockDesignButtonImage").GetComponent<Image>();
			img.sprite = (GameController.Design == GameController.DESIGN_NEW)? Dump.newRectSprite : Dump.oldRectSprite;


			mainMenu.GetComponent<Animator> ().Play ("open");
			optionsMenu.SetActive (false);
			creditsMenu.SetActive (false);
			shopMenu.SetActive (false);
			infoMenu.SetActive (false);
			currentMenu = 0;
		} 
		else if (SceneManager.GetActiveScene ().name.Equals ("endless_level")) {
			pauseMenu = GameObject.Find ("PauseMenu");
			gameOverMenu = GameObject.Find ("GameOverMenu");
			secondChanceMenu = GameObject.Find ("SecondChanceMenu");
			shopMenu = GameObject.Find ("ShopMenu");
			infoMenu = GameObject.Find ("InfoMenu");
			howToPlayMenu = GameObject.Find ("HowToPlay");

			howToPlayPages = new List<GameObject> ();
			GameObject buf;
			for (var i = 1; i <= 3; i++) {
				buf = GameObject.Find ("HowToPlay_" + i.ToString ());
				howToPlayPages.Add (buf);
				buf.SetActive (false);
			}
			currentPage = -1;
			ChangePage ();


			if (GameController.IsFirstGame == 0) {
				HowToPlayButton_Callback ();
				GameController.IsFirstGame = 1;
				GameController.SaveFirstGame ();
			}
			else howToPlayMenu.SetActive (false);

			Image img = GameObject.Find("MusicButtonImage").GetComponent<Image>();
			img.sprite = (GameController.Music == 1)? Dump.musicOnSprite : Dump.musicOffSprite;

			img = GameObject.Find("SoundButtonImage").GetComponent<Image>();
			img.sprite = (GameController.Sound == 1)? Dump.soundOnSprite : Dump.soundOffSprite;

			img = GameObject.Find("EffectsButtonImage").GetComponent<Image>();
			img.sprite = (GameController.Effects == 1)? Dump.effectsOnSprite : Dump.effectsOffSprite;
			adError = GameObject.Find ("AdError");
			

			currentMenu = -1;
			
			shopMenu.SetActive (false);
			infoMenu.SetActive (false);
			pauseMenu.SetActive (false);
			gameOverMenu.SetActive (false);
			secondChanceMenu.SetActive (false);
		}

		GameController.SetUIManager (this);
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update(){
		/*
	 	-1 - None
		0 - Main
		1 - Options
		2 - Pause
		3 - Game Over
		4 - Second Chance
		5 - Credits
		6 - Shop
		7 - Info
		8 - How to Play
	*/
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (currentMenu == -1) {
				PauseMenuButton_Callback ();
			}
			if (currentMenu == 0) {
				GameController.SaveUserData ();
				Application.Quit ();
			} else if (currentMenu == 1) {
				optionsMenu.SetActive (false);
				mainMenu.SetActive (true);
				currentMenu = 0;
				WaitDelay ();
			} else if (currentMenu == 2) {
				ClosePauseMenu ();
			} else if (currentMenu == 3) {
				CloseGameOverMenu ();
			} else if (currentMenu == 4) {
				CloseSecondChanceMenu ();
				currentMenu = -1;
			} else if (currentMenu == 5) {
				creditsMenu.SetActive (false);
				mainMenu.SetActive (true);
				currentMenu = 0;
				WaitDelay ();

			}
			else if (currentMenu == 6) {
				shopMenu.SetActive (false);
				if (mainMenu == null)
					currentMenu = 3;
				else
					currentMenu = 0;
				WaitDelay ();
			}
			else if (currentMenu == 7) {
				infoMenu.SetActive (false);
				currentMenu = 6;
				WaitDelay ();
			}
			else if (currentMenu == 8) {
				howToPlayMenu.SetActive (false);
			}

			if (exitRequested)
				Application.Quit ();
			else
				StartCoroutine (AppExit_Timer());
		} 
		else if (Input.GetKeyDown (KeyCode.Home) || Input.GetKeyDown(KeyCode.Menu)) 
		{
			GameController.SaveUserData ();
		}
	}

	/// <summary>
	/// Sets the exit timer for app.
	/// </summary>
	/// <returns>The exit timer.</returns>
	IEnumerator AppExit_Timer(){
		if (!exitRequested) {
			yield return new WaitForSeconds (0.1f);
			exitRequested = true;
			yield return new WaitForSeconds (0.4f);
			exitRequested = false;
		}
	}

	// Main Menu
	public void PlayButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		SceneManager.LoadScene ("endless_level");

	}

	/// <summary>
	/// The options button callback.
	/// </summary>
	public void OptionsButton_Callback(){
		mainMenu.SetActive (false);
		optionsMenu.SetActive (true);
		currentMenu = 1;
		GameController.TryPlaySound (SoundPool.Button);
	}

	/// <summary>
	/// The credits button callback.
	/// </summary>
	public void CreditsButton_Callback(){
		mainMenu.SetActive (false);
		currentMenu = 5;
		creditsMenu.SetActive (true);
		GameController.TryPlaySound (SoundPool.Button);
	}
	///

	//Pause Menu
	/// <summary>
	/// The pause menu button callback.
	/// </summary>
	public void PauseMenuButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		pauseMenu.SetActive (true);
		pauseMenu.GetComponent<Animator> ().Play ("open");
		StartCoroutine (PauseMenuOpen_Timer ());
	}

	/// <summary>
	/// The pause menu open timer.
	/// </summary>
	/// <returns>The menu open timer.</returns>
	IEnumerator PauseMenuOpen_Timer(){
		yield return new WaitForSeconds (0.33f);
		currentMenu = 2;
	}

	/// <summary>
	/// The Continue button callback.
	/// </summary>
	public void ContinueButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		GameController.SaveUserData ();
		ClosePauseMenu ();
	}

	/// <summary>
	/// Closes the pause menu.
	/// </summary>
	public void ClosePauseMenu(){
		pauseMenu.GetComponent<Animator> ().Play ("close");
		StartCoroutine (PauseMenuClose_Timer ());
	}

	/// <summary>
	/// Pauses the menu close timer.
	/// </summary>
	/// <returns>The menu close timer.</returns>
	IEnumerator PauseMenuClose_Timer(){
		yield return new WaitForSeconds (0.33f);
		pauseMenu.SetActive (false);
		currentMenu = -1;
	}
	///

	//Game Over Menu
	//

	/// <summary>
	/// Opens the game over menu.
	/// </summary>
	public void OpenGameOverMenu(){
		gameOverMenu.SetActive (true);
		GameObject.Find ("TextScore").GetComponent<Text> ().text = "Score: " + GameController.Score.ToString();
		GameController.BestScore = Mathf.Max (GameController.BestScore, GameController.Score);
		GameObject.Find ("TextBest").GetComponent<Text> ().text = "Best: " + GameController.BestScore.ToString();
		GameObject.Find ("TextTurns").GetComponent<Text> ().text = "Turns: " + GameController.Turn.ToString();
		GameController.SaveUserData ();
		gameOverMenu.GetComponent<Animator> ().Play ("open");
		currentMenu = 3;
	}

	/// <summary>
	/// Games the over retry button callback.
	/// </summary>
	public void GameOverRetryButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		CloseGameOverMenu ();
	}

	/// <summary>
	/// Closes the game over menu.
	/// </summary>
	public void CloseGameOverMenu(){
		gameOverMenu.GetComponent<Animator> ().Play ("close");
		StartCoroutine (GameOverMenuClose_Timer ());
	}

	/// <summary>
	/// Games the over menu close timer.
	/// </summary>
	/// <returns>The over menu close timer.</returns>
	IEnumerator GameOverMenuClose_Timer(){
		yield return new WaitForSeconds (1.0f);
		gameOverMenu.SetActive (false);
		currentMenu = -1;
		GameController.StartGame ();
	}
	///

	//Second Chance Menu
	/// <summary>
	/// Opens the second chance menu.
	/// </summary>
	public void OpenSecondChanceMenu(){
		GameController.IsSecondChanceUsed = 1;
		secondChanceMenu.SetActive (true);


		if (!GameController.IsAdLoaded ()) {
			DisableButton (GameObject.Find ("SecondChanceButton_ShowAd"));
			adError.SetActive (true);
		} else {
			EnableButton (GameObject.Find ("SecondChanceButton_ShowAd"));
			adError.SetActive (false);
		}

		secondChanceMenu.GetComponent<Animator> ().Play ("open");
		currentMenu = 4;
		StartCountdown ();
	}

	/// <summary>
	/// the Second chance show ad button callback.
	/// </summary>
	public void SecondChanceShowAdButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		if (GameController.ShowSecondChanceVideoAd ()) {
			CloseSecondChanceMenu ();
		} else {
			ShowFailAd ();
		}
	}

	/// <summary>
	/// the Second chance close button callback.
	/// </summary>
	public void SecondChanceCloseButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		CloseSecondChanceMenu ();
		OpenGameOverMenu ();
	}

	/// <summary>
	/// Closes the second chance menu.
	/// </summary>
	public void CloseSecondChanceMenu(){
		currentMenu = -1;
		adError.SetActive (false);
		secondChanceMenu.GetComponent<Animator> ().Play ("close");
		StartCoroutine (SecondChanceMenuClose_Timer ());
	}

	/// <summary>
	/// The second chance menu close timer.
	/// </summary>
	/// <returns>The chance menu close timer.</returns>
	IEnumerator SecondChanceMenuClose_Timer(){
		yield return new WaitForSeconds (1.0f);
		secondChanceMenu.SetActive (false);
	}

	/// <summary>
	/// Starts the countdown.
	/// </summary>
	private void StartCountdown(){
		Text timerText = GameObject.Find ("Countdown").GetComponent<Text>();
		timerText.text = "10";
		StartCoroutine (CountdownTimer (10, timerText));
	}

	/// <summary>
	/// The Countdoun timer.
	/// </summary>
	/// <returns>The timer.</returns>
	/// <param name="remain">Remain.</param>
	/// <param name="timerText">Timer text.</param>
	IEnumerator CountdownTimer(int remain, Text timerText){
		yield return new WaitForSeconds (1.0f);
		if (currentMenu == 4) {
			remain--;
			if (remain < 1) {
				CloseSecondChanceMenu ();
				OpenGameOverMenu ();
			} else {
				timerText.text = remain.ToString();
				StartCoroutine (CountdownTimer (remain, timerText));
			}
		}
	}

	///

	//HowToPlay Menu
	///
	public void HowToPlayButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		currentMenu = 8;
		howToPlayMenu.SetActive(true);

		StartCoroutine (HowToPlayMenuOpen_Timer ());
	}

	public void HowToPlayCloseButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		currentPage = -1;
		ChangePage ();
		howToPlayMenu.GetComponent<Animator> ().Play ("close");
		StartCoroutine (HowToPlayMenuClose_Timer ());
	}

	IEnumerator HowToPlayMenuClose_Timer(){
		yield return new WaitForSeconds (1.0f);
		howToPlayMenu.SetActive (false);
		currentMenu = -1;
	}

	IEnumerator HowToPlayMenuOpen_Timer(){
		yield return new WaitForSeconds (1.0f);
		currentPage = 1;
		ChangePage ();
	}

	public void HowToPlayNextButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);

		if (currentPage == 3)
			currentPage = 1;
		else
			currentPage++;
		ChangePage ();
	}

	public void HowToPlayPrevButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);

		if (currentPage == 1)
			currentPage = 3;
		else
			currentPage--;
		ChangePage ();
	}

	private void ChangePage(){
		for (var i = 0; i < 3; i++) {
			if (i == currentPage - 1)
				howToPlayPages [i].SetActive (true);
			else
				howToPlayPages [i].SetActive (false);
		}
	}

	//Shop Menu
	///
	public void ShopBackButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		shopMenu.SetActive (false);

		if (mainMenu != null) {
			currentMenu = 0;
			mainMenu.SetActive (true);
		} else {
			currentMenu = 3;
			gameOverMenu.SetActive (true);
		}
	}

	public void ShopBuyBombButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		GameController.Coins -= GameController.GetSkillPrice (Skills.Bomb, GameController.GetSkillLevel (Skills.Bomb));
		GameController.UpSkillLevel (Skills.Bomb);
		UpdateShopMenu ();
		GameController.SaveUserData ();
	}

	public void ShopBuyLaserButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		GameController.Coins -= GameController.GetSkillPrice (Skills.Electric, GameController.GetSkillLevel (Skills.Electric));
		GameController.UpSkillLevel (Skills.Electric);
		UpdateShopMenu ();
		GameController.SaveUserData ();
	}

	public void ShopBuySkullButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		GameController.Coins -= GameController.GetSkillPrice (Skills.Skull, GameController.GetSkillLevel (Skills.Skull));
		GameController.UpSkillLevel (Skills.Skull);
		UpdateShopMenu ();
		GameController.SaveUserData ();
	}

	public void ShopBuyDoubleCoinsButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		GameController.Coins -= GameController.GetSkillPrice (Skills.Double, GameController.GetSkillLevel (Skills.Double));
		GameController.UpSkillLevel (Skills.Double);
		UpdateShopMenu ();
		GameController.SaveUserData ();
	}

	public void ShopBombInfoButton_Callback (){
		GameController.TryPlaySound (SoundPool.Button);
		OpenInfoMenu (Skills.Bomb);
	}

	public void ShopLaserInfoButton_Callback (){
		GameController.TryPlaySound (SoundPool.Button);
		OpenInfoMenu (Skills.Electric);
	}

	public void ShopSkullInfoButton_Callback (){
		GameController.TryPlaySound (SoundPool.Button);
		OpenInfoMenu (Skills.Skull);
	}

	public void ShopDoubleInfoButton_Callback (){
		GameController.TryPlaySound (SoundPool.Button);
		OpenInfoMenu (Skills.Double);
	}

	public void ShopBuyCoinsForAdButton_Callback(){
		GameController.ShowBuyCoinsVideoAd ();
	}

	///

	//Info Menu
	///

	public void InfoCloseButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		CloseInfoMenu ();
	}

	public void OpenInfoMenu(Skills skill){
		infoMenu.SetActive (true);
		FillInfo (skill);
		infoMenu.GetComponent<Animator> ().Play ("open");
		currentMenu = 7;
	}

	public void CloseInfoMenu (){
		infoMenu.GetComponent<Animator> ().Play ("close");
		StartCoroutine (InfoMenuClose_Timer ());
	}

	IEnumerator InfoMenuClose_Timer(){
		yield return new WaitForSeconds (1.0f);
		infoMenu.SetActive (false);
		currentMenu = 6;
	}

	/// <summary>
	/// Fills the info in description of items.
	/// </summary>
	/// <param name="skill">Skill.</param>
	public void FillInfo(Skills skill){
		switch (skill) {

		case Skills.Bomb:
			GameObject.Find ("InfoTitle").GetComponent<Text> ().text = "Bomb";
			GameObject.Find ("InfoLevel").GetComponent<Text> ().text = "LVL " + GameController.GetSkillLevel (Skills.Bomb) + "/4";
			GameObject.Find ("InfoImage").GetComponent<Image> ().sprite = 
				(GameController.GetSkillLevel (Skills.Bomb) != 4) ? Dump.bombSprite : Dump.superBombSprite;
			GameObject.Find ("Description").GetComponent<Text> ().text = "Explodes blocks in 2 block radiuses";
			GameObject.Find ("Lvl0Description").GetComponent<Text> ().text = "Drop chance - 0.5%";
			GameObject.Find ("Lvl1Description").GetComponent<Text> ().text = "Drop chance - 1%";
			GameObject.Find ("Lvl2Description").GetComponent<Text> ().text = "Drop chance - 3%";
			GameObject.Find ("Lvl3Description").GetComponent<Text> ().text = "Drop chance - 5%";
			GameObject.Find ("Lvl4Description").GetComponent<Text> ().text = 
				"Adds 50% chance to replace Bomb with Super Bomb (explodes blocks in 3 block radiuses)";
			break;

		case Skills.Electric:
			GameObject.Find ("InfoTitle").GetComponent<Text> ().text = "Laser";
			GameObject.Find ("InfoLevel").GetComponent<Text> ().text = "LVL " + GameController.GetSkillLevel (Skills.Electric) + "/4";
			GameObject.Find ("InfoImage").GetComponent<Image> ().sprite = 
				(GameController.GetSkillLevel (Skills.Electric) != 4) ? Dump.electricSprite : Dump.superElectricSprite;
			GameObject.Find ("Description").GetComponent<Text> ().text = "Burns blocks on horizontal and vertical line in 3 block radiuses";
			GameObject.Find ("Lvl0Description").GetComponent<Text> ().text = "Drop chance - 0.5%";
			GameObject.Find ("Lvl1Description").GetComponent<Text> ().text = "Drop chance - 1%";
			GameObject.Find ("Lvl2Description").GetComponent<Text> ().text = "Drop chance - 2%";
			GameObject.Find ("Lvl3Description").GetComponent<Text> ().text = "Drop chance - 3%";
			GameObject.Find ("Lvl4Description").GetComponent<Text> ().text = 
				"Adds 50% chance to replace Laser with Super Laser (burns all blocks on lines)";
			break;

		case Skills.Skull:
			GameObject.Find ("InfoTitle").GetComponent<Text> ().text = "Skull";
			GameObject.Find ("InfoLevel").GetComponent<Text> ().text = "LVL " + GameController.GetSkillLevel (Skills.Skull) + "/4";
			GameObject.Find ("InfoImage").GetComponent<Image> ().sprite = 
				(GameController.GetSkillLevel (Skills.Skull) != 4) ? Dump.skullSprite : Dump.superSkullSprite;
			GameObject.Find ("Description").GetComponent<Text> ().text = "Destroys blocks with same color";
			GameObject.Find ("Lvl0Description").GetComponent<Text> ().text = "Drop chance - 0.05%";
			GameObject.Find ("Lvl1Description").GetComponent<Text> ().text = "Drop chance - 0.1%";
			GameObject.Find ("Lvl2Description").GetComponent<Text> ().text = "Drop chance - 0.3%";
			GameObject.Find ("Lvl3Description").GetComponent<Text> ().text = "Drop chance - 0.5%";
			GameObject.Find ("Lvl4Description").GetComponent<Text> ().text = 
				"Adds 50% chance to replace Skull with Super Skull (destroys all blocks on the field)";
			break;

		case Skills.Double:
			GameObject.Find ("InfoTitle").GetComponent<Text> ().text = "X2 Bonus";
			GameObject.Find ("InfoLevel").GetComponent<Text> ().text = "LVL " + GameController.GetSkillLevel (Skills.Double) + "/4";
			GameObject.Find ("InfoImage").GetComponent<Image> ().sprite = Dump.doubleMultSprite;
			GameObject.Find ("Description").GetComponent<Text> ().text = "Gives double score points on 10 turns";
			GameObject.Find ("Lvl0Description").GetComponent<Text> ().text = "Drop chance - 0.25%";
			GameObject.Find ("Lvl1Description").GetComponent<Text> ().text = "Drop chance - 0.5%";
			GameObject.Find ("Lvl2Description").GetComponent<Text> ().text = "Drop chance - 1%";
			GameObject.Find ("Lvl3Description").GetComponent<Text> ().text = "Drop chance - 1.5%";
			GameObject.Find ("Lvl4Description").GetComponent<Text> ().text = 
				"Bonus affects on coins";
			break;

		}
	}


	//Other Callbacks
	///
	public void SoundButton_Callback(){
		GameController.SwitchSoundSettings ();
		GameController.TryPlaySound (SoundPool.Button);

		Image img = GameObject.Find("SoundButtonImage").GetComponent<Image>();
		img.sprite = (GameController.Sound == 1)? Dump.soundOnSprite : Dump.soundOffSprite;
	}

	public void MusicButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		GameController.SwitchMusicSettings ();
		GameController.CorrectMusic ();

		Image img = (Image) GameObject.Find("MusicButtonImage").GetComponent<Image>();
		img.sprite = (GameController.Music == 1)? Dump.musicOnSprite : Dump.musicOffSprite;

	}

	public void EffectsButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		GameController.SwitchEffectsSettings ();

		Image img = (Image) GameObject.Find("EffectsButtonImage").GetComponent<Image>();
		img.sprite = (GameController.Effects == 1)? Dump.effectsOnSprite : Dump.effectsOffSprite;
	}

	public void DesignButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		GameController.SwitchDesignSettings ();

		Image img = (Image) GameObject.Find("BlockDesignButtonImage").GetComponent<Image>();
		img.sprite = (GameController.Design == GameController.DESIGN_NEW)? Dump.newRectSprite : Dump.oldRectSprite;
	}

	public void BackButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		GameController.SaveUserData ();
		optionsMenu.SetActive (false);
		creditsMenu.SetActive (false);
		mainMenu.SetActive (true);
		currentMenu = 0;
	}

	public void RateButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		//Open Rate URL
		Application.OpenURL("https://play.google.com/store/apps/details?id=com.tenxgames.colorumbus");
	}

	public void HomeButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		GameController.IsGameStarted = 0;
		GameController.SaveUserData ();
		currentMenu = 0;
		GameController.ShowInterstitialAd ();

		SceneManager.LoadScene ("menu_scene");
	}

	public void ShopButton_Callback(){
		GameController.TryPlaySound (SoundPool.Button);
		//Shop
		currentMenu = 6;
		shopMenu.SetActive(true);
		UpdateShopMenu ();
	}

	private void DisableButton(GameObject button){
		button.GetComponent<Button>().interactable = false;
		button.GetComponent<Image>().color = new Color (0.7647f, 0.7647f, 0.7647f);
	}

	private void EnableButton(GameObject button){
		button.GetComponent<Button>().interactable = true;
		button.GetComponent<Image>().color = new Color (0.6588f, 0.9333f, 0.6823f);
	}

	public void ShowFailAd(){
		adError.SetActive (true);
	}

	/// <summary>
	/// Updates the skill info.
	/// </summary>
	/// <param name="skill">Skill.</param>
	public void UpdateSkillInfo(Skills skill){
		int price;
		switch (skill) {

		case Skills.Bomb:
			
			GameObject.Find ("BombLevel").GetComponent<Text> ().text = "LVL " + GameController.GetSkillLevel (Skills.Bomb)
			+ "/4";
			GameObject.Find ("BombImage").GetComponent<Image> ().sprite = 
				(GameController.GetSkillLevel (Skills.Bomb) != 4) ? Dump.bombSprite : Dump.superBombSprite;
			price = GameController.GetSkillPrice (Skills.Bomb, GameController.GetSkillLevel (Skills.Bomb));
			ApplyPrice (price, GameObject.Find ("Button_BuyBomb"), GameObject.Find ("BombPrice").GetComponent<Text> ());
			break;

		case Skills.Electric:
			GameObject.Find ("LaserLevel").GetComponent<Text> ().text = "LVL " + GameController.GetSkillLevel (Skills.Electric)
			+ "/4";
			GameObject.Find ("LaserImage").GetComponent<Image> ().sprite = 
				(GameController.GetSkillLevel (Skills.Electric) != 4) ? Dump.electricSprite : Dump.superElectricSprite;
			price = GameController.GetSkillPrice (Skills.Electric, GameController.GetSkillLevel (Skills.Electric));
			ApplyPrice (price, GameObject.Find ("Button_BuyLaser"), GameObject.Find ("LaserPrice").GetComponent<Text> ());
			break;

		case Skills.Skull:
			GameObject.Find ("SkullLevel").GetComponent<Text>().text = "LVL " + GameController.GetSkillLevel(Skills.Skull)
				+ "/4";
			GameObject.Find ("SkullImage").GetComponent<Image> ().sprite = 
				(GameController.GetSkillLevel (Skills.Skull) != 4) ? Dump.skullSprite : Dump.superSkullSprite;
			price = GameController.GetSkillPrice (Skills.Skull, GameController.GetSkillLevel (Skills.Skull));
			ApplyPrice (price, GameObject.Find ("Button_BuySkull"), GameObject.Find ("SkullPrice").GetComponent<Text> ());
			break;

		case Skills.Double:
			GameObject.Find ("DoubleCoinsLevel").GetComponent<Text>().text = "LVL " + GameController.GetSkillLevel(Skills.Double)
				+ "/4";
			price = GameController.GetSkillPrice (Skills.Double, GameController.GetSkillLevel (Skills.Double));
			ApplyPrice (price, GameObject.Find ("Button_BuyDoubleCoins"), GameObject.Find ("DoubleCoinsPrice").GetComponent<Text> ());
			break;
		}
	}

	/// <summary>
	/// Updates the shop balance.
	/// </summary>
	private void UpdateShopBalance(){
		GameObject.Find ("BalanceText").GetComponent<Text>().text = GameController.Coins.ToString();
	}

	/// <summary>
	/// Updates the shop menu.
	/// </summary>
	public void UpdateShopMenu(){
		if (GameController.IsAdLoaded ())
			EnableButton (GameObject.Find ("Button_CoinsForAd"));
		else
			DisableButton (GameObject.Find ("Button_CoinsForAd"));
		
		UpdateShopBalance ();

		UpdateSkillInfo (Skills.Bomb);
		UpdateSkillInfo (Skills.Electric);
		UpdateSkillInfo (Skills.Skull);
		UpdateSkillInfo (Skills.Double);
	}

	/// <summary>
	/// Applies the price.
	/// </summary>
	/// <param name="price">Price.</param>
	/// <param name="button">Button.</param>
	/// <param name="text">Text.</param>
	private void ApplyPrice(int price, GameObject button, Text text){
		if (price == -1) {
			DisableButton (button);
			text.text = "MAX";
		} else if (price > GameController.Coins) {
			DisableButton (button);
			text.text = price.ToString ();
		} else {
			EnableButton (button);
			text.text = price.ToString ();
		}
	}

	public void WaitDelay(){	
		if (isReturnerRun == false)
			isReturnerRun = true;
			StartCoroutine (Waiter ());
	}

	IEnumerator Waiter(){
		yield return new WaitForSeconds (0.1f);
		isReturnerRun = false;
	}
}
