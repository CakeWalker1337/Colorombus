using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum SoundPool
{
	PutBlock,
	Combo,
	Button,
	End,
	Background,
	Destroy,
	Bomb,
	Laser,
	Skull
}

public static class GameController {
 
	private static bool isInitialized = false;

	public static int Sound { get; set; }
	public static int Music { get; set; }
	public static int Effects { get; set;}
	public static int BestScore { get; set; }
	public static int Score { get; set; }
	public static int Turn { get; set; }
	public static int IsGameStarted { get; set; }
	public static int IsSecondChanceUsed { get; set; }

	public static AudioSource MusicSource { get; set; }
	public static AudioSource SoundSource { get; set; }

	private static UIManagerScript UIManager;
	private static LevelEditor LevelEditor;
	private static AdManager adManager;

	public static void InitGlobalSettings()
	{
		LoadUserData ();
		Dump.Init ();
		GameObject audioPoint = GameObject.Find ("MusicPoint");

		GameObject.DontDestroyOnLoad (audioPoint);
		MusicSource = audioPoint.GetComponent<AudioSource>();
		MusicSource.clip = Dump.backgroundClip;
		if (Music == 1)
			MusicSource.Play ();


		audioPoint = GameObject.Find ("SoundPoint");
		GameObject.DontDestroyOnLoad (audioPoint);
		SoundSource = audioPoint.GetComponent<AudioSource>(); 
		IsSecondChanceUsed = 0;
		isInitialized = true;
		adManager = new AdManager ();
		adManager.InitAds ();
	}

	public static void SetUIManager (UIManagerScript manager)
	{
		UIManager = manager;
	}

	public static void SetLevelEditor(LevelEditor levelEditor)
	{
		LevelEditor = levelEditor;
	}

	public static void EndGame()
	{
		if (IsSecondChanceUsed == 0) {
			UIManager.OpenSecondChanceMenu ();
		} 
		else {
			IsGameStarted = 0;
			UIManager.OpenGameOverMenu ();
		}
	}


	public static void StartGame()
	{
		IsSecondChanceUsed = 0;
		IsGameStarted = 0;
		LevelEditor.StartGame ();
	}

	public static void ContinueGame()
	{
		LevelEditor.ContinueGame ();
	}

	public static void SwitchSoundSettings()
	{
		Sound = (Sound == 1) ? 0 : 1;
	}

	public static void TryPlaySound(SoundPool sound)
	{
		if (Sound == 1) {
			
			switch (sound) {

			case SoundPool.Button:
				SoundSource.PlayOneShot (Dump.buttonClip);
				break;
			case SoundPool.PutBlock:
				SoundSource.PlayOneShot (Dump.putBlockClip);
				break;
			case SoundPool.Combo:
				SoundSource.PlayOneShot (Dump.comboClip);
				break;
			case SoundPool.End:
				SoundSource.PlayOneShot (Dump.endClip);
				break;
			case SoundPool.Destroy:
				SoundSource.PlayOneShot (Dump.destroyClip);
				break;
			case SoundPool.Bomb:
				SoundSource.PlayOneShot (Dump.bombClip);
				break;
			case SoundPool.Laser:
				SoundSource.PlayOneShot (Dump.laserClip);
				break;
			case SoundPool.Skull:
				SoundSource.PlayOneShot (Dump.skullClip);
				break;
			}
		}
	}

	public static void SwitchMusicSettings()
	{
		Music = (Music == 1) ? 0 : 1;
	}
	
	public static void CorrectMusic()
	{
		if (Music == 1)
			MusicSource.Play ();
		else 
			MusicSource.Stop ();
	}

	public static void SwitchEffectsSettings()
	{
		Effects = (Effects == 1) ? 0 : 1;
	}

	public static void LoadUserData()
	{
		Sound = (PlayerPrefs.HasKey ("sound")) ? PlayerPrefs.GetInt ("sound") : 1;
		Music = (PlayerPrefs.HasKey ("music")) ? PlayerPrefs.GetInt ("music") : 1;
		Effects = (PlayerPrefs.HasKey ("effects")) ? PlayerPrefs.GetInt ("effects") : 1;
		BestScore = (PlayerPrefs.HasKey ("best")) ? PlayerPrefs.GetInt ("best") : 0;
		IsGameStarted = (PlayerPrefs.HasKey ("game_started")) ? PlayerPrefs.GetInt ("game_started") : 0;

	}

	public static void SaveUserData()
	{
		PlayerPrefs.SetInt ("sound", Sound);
		PlayerPrefs.SetInt ("music", Music);
		PlayerPrefs.SetInt ("effects", Effects);
		PlayerPrefs.SetInt ("best", BestScore);
		PlayerPrefs.SetInt ("game_started", IsGameStarted);
		PlayerPrefs.Save ();
	}


	public static string LoadGameData(){
		if (PlayerPrefs.HasKey ("gamedata"))
			return PlayerPrefs.GetString ("gamedata");
		return null;
	}

	public static void SaveGameData(string data){
		PlayerPrefs.SetString ("gamedata", data);
	}

	public static bool IsInitialized()
	{
		return isInitialized;
	}

	public static void ShowVideoAd()
	{
		adManager.TryStartVideo ();
	}

	public static void CloseSecondChanceMenu()
	{
		UIManager.CloseSecondChanceMenu ();
	}

	public static void ShowInterstitialAd()
	{
		adManager.TryStartInterstitial ();
	}

	public static void ShowFailAd()
	{
		UIManager.ShowFailAd ();
	}


}

