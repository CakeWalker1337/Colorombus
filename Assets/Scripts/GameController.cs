using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum Skills{
	Bomb = 0,
	Electric = 1,
	Skull = 2,
	Double = 3
};

public enum SoundPool
{
	PutBlock,
	Coin,
	Button,
	End,
	Background,
	Destroy,
	Bomb,
	Laser,
	Skull
};

public static class GameController {
 
	private static List<int> skills;
	public static int GetSkillLevel(Skills index){
		return skills [(int)index];
	}

	public static bool RemoveSkill(Skills index){
		return skills.Remove((int)index);
	}

	public static void AddSkill(Skills index){
		skills.Add((int)index);
	}

	private static int[][] costs;
	private static int intersistialCalls;

	public static int SkillsCount{get{ return skills.Count;}}

	private static bool isInitialized = false;

	public static int Sound { get; set; }
	public static int Music { get; set; }
	public static int Effects { get; set;}

	public static int BestScore { get; set; }
	public static int Coins { get; set; }
	public static int Score { get; set; }
	public static int Turn { get; set; }
	public static int IsGameStarted { get; set; }
	public static int IsSecondChanceUsed { get; set; }

	public static AudioSource MusicSource { get; set; }
	public static AudioSource SoundSource { get; set; }

	private static UIManagerScript UIManager;
	private static LevelEditor LevelEditor;
	private static AdManager adManager;

	public static int COINS_FOR_AD = 50;

	public static void InitGlobalSettings()
	{
		LoadUserData ();
		Dump.Init ();
		InitSkillCosts ();

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
		intersistialCalls = 0;
		adManager = new AdManager ();
		adManager.InitAds ();
		isInitialized = true;
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
			case SoundPool.Coin:
				SoundSource.PlayOneShot (Dump.coinClip);
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
		Coins = (PlayerPrefs.HasKey ("coins")) ? PlayerPrefs.GetInt ("coins") : 0;
		skills = ParseSkills((PlayerPrefs.HasKey ("skills")) ? PlayerPrefs.GetString ("skills") : "0_0_0_0");
		BestScore = (PlayerPrefs.HasKey ("best")) ? PlayerPrefs.GetInt ("best") : 0;
		IsGameStarted = (PlayerPrefs.HasKey ("game_started")) ? PlayerPrefs.GetInt ("game_started") : 0;

	}

	public static void SaveUserData()
	{
		PlayerPrefs.SetInt ("sound", Sound);
		PlayerPrefs.SetInt ("music", Music);
		PlayerPrefs.SetInt ("effects", Effects);
		PlayerPrefs.SetInt ("best", BestScore);
		PlayerPrefs.SetInt ("coins", Coins);
		PlayerPrefs.SetString ("skills", BuildSkillsString(skills));
		PlayerPrefs.SetInt ("game_started", IsGameStarted);
		PlayerPrefs.Save ();
	}

	public static void SaveCoins(){
		PlayerPrefs.SetInt ("coins", Coins);
	}

	public static List<int> ParseSkills(string str){
		if (str == "")
			return null;
		string[] buf = str.Split('_');
		List<int> res = new List<int> ();
		for (int i = 0; i < buf.GetLength (0); i++)
			res.Add (int.Parse (buf [i]));
		return res;
	}

	public static string BuildSkillsString(List<int> buf){
		StringBuilder sb = new StringBuilder ();
		if (buf.Count == 0)
			return "";
		sb.AppendFormat ("{0}", buf [0]);
		for (int i = 1; i < buf.Count; i++)
			sb.AppendFormat ("_{0}", buf [i]);
		return sb.ToString ();
	}


	public static string LoadGameData(){
		if (PlayerPrefs.HasKey ("gamedata"))
			return PlayerPrefs.GetString ("gamedata");
		return null;
	}

	public static void SaveGameData(string data){
		PlayerPrefs.SetString ("gamedata", data);
	}

	public static float GetLevelValue (Skills skill){
		if (skill == Skills.Bomb) {
			if (GetSkillLevel (Skills.Bomb) == 0)
				return 0.5f;
			if (GetSkillLevel (Skills.Bomb) == 1)
				return 1f;
			if (GetSkillLevel (Skills.Bomb) == 2)
				return 3f;
			if (GetSkillLevel (Skills.Bomb) == 3 || GetSkillLevel (Skills.Bomb) == 4)
				return 5f;			
		}
		if (skill == Skills.Electric) {
			if (GetSkillLevel (Skills.Electric) == 0)
				return 0.5f;
			if (GetSkillLevel (Skills.Electric) == 1)
				return 1f;
			if (GetSkillLevel (Skills.Electric) == 2)
				return 2f;
			if (GetSkillLevel (Skills.Electric) == 3 || GetSkillLevel (Skills.Electric) == 4)
				return 3f;			
		}
		if (skill == Skills.Skull) {
			if (GetSkillLevel (Skills.Skull) == 0)
				return 0.05f;
			if (GetSkillLevel (Skills.Skull) == 1)
				return 0.1f;
			if (GetSkillLevel (Skills.Skull) == 2)
				return 0.3f;
			if (GetSkillLevel (Skills.Skull) == 3 || GetSkillLevel (Skills.Skull) == 4)
				return 0.5f;			
		}
		if (skill == Skills.Double) {
			if (GetSkillLevel (Skills.Double) == 0)
				return 0.25f;
			if (GetSkillLevel (Skills.Double) == 1)
				return 0.5f;
			if (GetSkillLevel (Skills.Double) == 2)
				return 1f;
			if (GetSkillLevel (Skills.Double) == 3 || GetSkillLevel (Skills.Double) == 4)
				return 1.5f;			
		}
		return 0;
	}

	public static void UpSkillLevel(Skills skill)
	{
		skills [(int)skill]++;
	}

	public static void InitSkillCosts()
	{
		costs = new int[4][];
		costs [0] = new int[4];
		costs [1] = new int[4];
		costs [2] = new int[4];
		costs [3] = new int[4];

		//Bomb
		costs [0] [0] = 150; costs [0] [1] = 300; costs [0] [2] = 600; costs[0][3] = 1200;
		//Laser
		costs [1] [0] = 250; costs [1] [1] = 500; costs [1] [2] = 1000; costs[1][3] = 1700;
		//Skull
		costs [2] [0] = 400; costs [2] [1] = 800; costs [2] [2] = 1600; costs[2][3] = 3500;
		//Double coins
		costs [3] [0] = 200; costs [3] [1] = 400; costs [3] [2] = 1000; costs[3][3] = 1500;

	}

	public static int GetSkillPrice(Skills skill, int currentLevel)
	{
		if (currentLevel == 4)
			return -1;

		return costs[(int) skill][currentLevel];
	}


	public static bool IsInitialized()
	{
		return isInitialized;
	}

	public static bool ShowSecondChanceVideoAd()
	{
		if (IsAdLoaded()) {
			adManager.TryStartSecondChanceVideo ();
			return true;
		}

		return false;

		//Non-add mode
		//ContinueGame();
	}

	public static bool ShowBuyCoinsVideoAd()
	{
		if (IsAdLoaded()){
			adManager.TryStartShopCoinsVideo ();
			return true;
		}

		return false;
	}

	public static void CloseSecondChanceMenu()
	{
		UIManager.CloseSecondChanceMenu ();
	}

	public static void ShowInterstitialAd()
	{
		intersistialCalls++;
		if (intersistialCalls == 3) {
			adManager.TryStartInterstitial ();
			intersistialCalls = 0;
		}
	}

	public static void GetCoinsForAd()
	{
		Coins += COINS_FOR_AD;
		UIManager.UpdateShopMenu ();
	}

	public static bool IsAdLoaded()
	{
		return adManager.IsVideoAdLoaded ();
	}


}

