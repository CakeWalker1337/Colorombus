using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

 
/// <summary>
/// Color reward.
/// </summary>
public enum ColorReward
{
	Red = 10,
	Orange = 20,
	Yellow = 30,
	Green = 40,
	Blue = 50,
	DarkBlue = 60,
	Purple = 70
};

/// <summary>
/// Destroying animation.
/// </summary>
public enum DestroyingAnimation
{
	None = 0,
	Decreasing = 1,
	Grinding = 2,
	Inferno = 3
};

/// <summary>
/// Level editor.
/// </summary>
public class LevelEditor : MonoBehaviour, IHasTurnEnded{
	private static List<GameObject> slots;
	public const int MAXNUM = 7;
	public const int SUPRESSION_EDGE = 200;
	//              1   2   3   4   5   6
	int[] range = new int[MAXNUM]; // start pos
	public static int[] CurrentColors = new int[8];
	GameObject ScoreBoard, BestScoreBoard, TurnBoard, MultBoard, CoinBoard;
	BrickType currentBonus = BrickType.Standart;
	float currentMult = 1.0f;
	int multTurns = 0;
	Color orange;
	public Color currentColor;
	public int currentTurnStreak = 0;
	public static RectTransform MainGrid, ItemGrid;
	 //Максимальное число новых кубиков на поле
	public static RectTransform MainLayer{ get; set;}
	float CellWidth, CellHeight;
	List<GameObject> TrashCan;

	// Use this for initialization
	void Start () {
		//Dump.Init ();
		GameController.SetLevelEditor(this);
		ScoreBoard = GameObject.Find ("ScoreBoard");
		BestScoreBoard = GameObject.Find ("BestScoreBoard");
		TurnBoard = GameObject.Find ("TurnBoard");
		MultBoard = GameObject.Find ("MultBoard");
		CoinBoard = GameObject.Find ("CoinBoard");
		slots = new List<GameObject>();

		MainGrid = GameObject.Find ("MainGrid").GetComponent<RectTransform> ();
		ItemGrid = GameObject.Find ("ItemGrid").GetComponent<RectTransform> ();
		MainLayer = GameObject.Find ("MainLayer").GetComponent<RectTransform> ();

		TrashCan = new List<GameObject>();

		//Counting the width of elements
		CellWidth = MainLayer.rect.height * 0.08f;
		CellHeight = CellWidth;
		MainGrid.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CellWidth * 6.3f);
		MainGrid.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CellHeight * 8.3f);
		MainGrid.anchoredPosition = new Vector2 (MainLayer.rect.width/2.0f, MainLayer.rect.height/2.0f);

		ItemGrid.gameObject.GetComponent<GridLayoutGroup> ().cellSize = new Vector2 (CellWidth, CellHeight);
		ItemGrid.gameObject.GetComponent<GridLayoutGroup> ().spacing = new Vector2 (CellWidth, 10f);

		//Generating itemslots (in the bottom of screen)
		for (int i = 0; i < 3; i++) {
			GameObject go = new GameObject();
			go.name = "itemslot"+(i+1).ToString();
			go.transform.SetParent (ItemGrid);
			go.AddComponent<RectTransform> ().localScale = Vector3.one;
			var grid = go.AddComponent<GridLayoutGroup>();
			grid.cellSize = new Vector2 (CellWidth, CellHeight);

		}

		//PlayerPrefs.DeleteKey ("gamedata");

		// Generating slots (main grid)
		MainGrid.gameObject.GetComponent<GridLayoutGroup> ().cellSize = new Vector2 (CellWidth, CellHeight);
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 6; j++) {
				GameObject slot = new GameObject();
				slot.name = "b" + ((i * 6) + j).ToString ();
				Button button = slot.AddComponent<Button> ();
				Image im = slot.AddComponent<Image> ();
				im.sprite = Dump.slotSpriteRectangle;
				im.color = new Color (0.914f, 1f, 0.973f);  
				slot.layer = 5;
				button.targetGraphic = im;
				RectTransform t = slot.GetComponent<RectTransform> ();
				GridLayoutGroup gr = slot.AddComponent<GridLayoutGroup> ();
				gr.cellSize = new Vector2 (CellWidth, CellHeight);
				t.SetParent (MainGrid);	
				slot.AddComponent<Slot> ();

				t.localScale = Vector3.one;

				slots.Add(slot);
			}	
		}

		StartGame ();
	}

	/// <summary>
	/// Fills the item grid.
	/// </summary>
	public void FillItemGrid()
	{
		List<int> elements = new List<int>();
		//"smart" counting of colors
		for(int j = 1; j<=7; j++)
		{
			if (CurrentColors [j] > 0)
				elements.Add (j);
		}
		int r = Random.Range (1, 4);
		//generating blocks
		for (int i = 0; i < 3; i++) {
			BrickColor bc = BrickColor.Random;

			if (elements.Count > 0) {
				int index = Random.Range (0, elements.Count);
				bc = (BrickColor)elements [index];
				elements.RemoveAt (index);
			} 
				
			Brick b;
			if (r - 1 == i) {
				Debug.Log (currentBonus.ToString());
				b = new Brick (currentBonus, bc);
				currentBonus = BrickType.Standart;
			}
			else b = new Brick(BrickType.Standart, bc);
			b.brick.AddComponent<CanvasGroup> ();
			b.brick.transform.SetParent (ItemGrid.GetChild(i));
			b.scale = Vector3.one;
			b.animator.Play ("Spawn");
			b.brick.AddComponent<DragHandler> ();
		}
	}

	/// <summary>
	/// Continues the game after first lose (if ad has watched).
	/// </summary>
	public void ContinueGame()
	{
		ClearField ();
		for (int i = 1; i <= 7; i++)
			CurrentColors [i] = 0;
		FillItemGrid ();

		SaveGame ();

		GameController.IsGameStarted = 1;
		GameController.IsSecondChanceUsed = 1;
		GameController.SaveUserData ();
	}

	/// <summary>
	/// Starts the game.
	/// </summary>
	public void StartGame()
	{
		BestScoreBoard.GetComponent<Text> ().text = GameController.BestScore.ToString ();
		CoinBoard.GetComponent<Text> ().text = GameController.Coins.ToString ();
		GameController.Score = 0;
		GameController.Turn = 1;
		GameController.IsSecondChanceUsed = 0;
		currentMult = 1f;
		ClearField ();
		for (int i = 1; i <= 7; i++)
			CurrentColors [i] = 0;
		if (GameController.IsGameStarted == 1) {
			LoadGame ();	
			ScoreBoard.GetComponent<Text> ().text = GameController.Score.ToString ();
			TurnBoard.GetComponent<Text> ().text = GameController.Turn.ToString ();
		} else {
			
			FillItemGrid ();
			SaveGame ();
			ScoreBoard.GetComponent<Text> ().text = GameController.Score.ToString ();
			TurnBoard.GetComponent<Text> ().text = GameController.Turn.ToString ();
			GameController.IsGameStarted = 1;
			GameController.SaveUserData ();
		}
		RecountCurrentMult ();
	}
	/// <summary>
	/// Clears the field.
	/// </summary>
	void ClearField()
	{
		foreach (GameObject sl in slots) {
			if(sl.transform.childCount > 0)
				Destroy(sl.transform.GetChild(0).gameObject);
		}

		foreach (Transform item in ItemGrid) {
			if (item.childCount > 0) {
				Destroy (item.GetChild(0).gameObject);
			}
		}

	}

	void Update () {
		
	}
		
	#region IHasTurnEnded implementation
	/// <summary>
	/// Determines whether this instance has turn ended the specified currentItem.
	/// </summary>
	/// <returns><c>true</c> if this instance has turn ended the specified currentItem; otherwise, <c>false</c>.</returns>
	/// <param name="currentItem">Current item.</param>
	public void HasTurnEnded (GameObject currentItem)
	{
		//Destroying unused elements of item grid
		for (int i = 0; i < ItemGrid.childCount; i++) {
			if (ItemGrid.GetChild (i).childCount > 0) {
				if (ItemGrid.GetChild (i).GetChild (0).childCount > 0) {
					Transform bonus = ItemGrid.GetChild (i).GetChild (0).GetChild(0);
					bonus.SetParent (ScoreBoard.transform);
					Destroy (bonus.gameObject);
				}
				DestroyWithAnimation (ItemGrid.GetChild(i).GetChild(0).gameObject, DestroyingAnimation.Decreasing, false, false);
			}
		}

		//Clears the trashcan (massive of objects, which has worked off)
		for (int i = 0; i < TrashCan.Count; i++) {
			Destroy (TrashCan[i]);
		}
		TrashCan.Clear ();

		// Counting targets (elements which has got a close positions related current dragged element) 
		List<GameObject> targets = new List<GameObject> ();
		GameObject currentSlot = currentItem.transform.parent.gameObject;
		currentTurnStreak = 0;
		currentColor = currentItem.GetComponent<Image> ().color;
		for (int i = 0; i < slots.Count; i++) {
			if ((slots [i].GetComponent<RectTransform>().anchoredPosition - currentSlot.GetComponent<RectTransform>().anchoredPosition).magnitude <= CellWidth * 1.3f && slots[i].transform.childCount>0 && slots[i].transform.GetChild(0).gameObject.GetComponent<Image>().color == currentColor) {
				targets.Add (slots [i]);
			}
		}
		if (targets.Count > 1) {
			//Destroying target elements
			GameController.TryPlaySound (SoundPool.Destroy);
			for (int i = 0; i < targets.Count; i++) {
				if (targets [i].transform.childCount > 0) {
					DestroyWithAnimation (targets [i].transform.GetChild (0).transform.gameObject, DestroyingAnimation.Grinding, true, true);
				}
			}
		}
		else
			GameController.TryPlaySound (SoundPool.PutBlock);
		StartCoroutine (NewGeneration (0.5f));
	}
	#endregion
	/// <summary>
	/// Generates the new generation.
	/// </summary>
	/// <returns>Enumerator.</returns>
	/// <param name="sec">Secs for delay.</param>
	IEnumerator NewGeneration(float sec){
		yield return new WaitForSeconds(sec);
		List<Transform> freeSlots = new List<Transform> ();
		foreach (GameObject go in slots) {
			Transform tr = go.GetComponent<Transform> ();
			if (tr.childCount == 0)
				freeSlots.Add (tr);
		}

		int r = CountTurn(freeSlots.Count);
		if (r > freeSlots.Count)
			r = freeSlots.Count;
		while (r > 0) {
			Brick b = new Brick (GenerateBrickType(), BrickColor.Random);
			int index = Random.Range (0, freeSlots.Count - 1);
			b.brick.transform.SetParent (freeSlots [index]);
			b.animator.Play ("Spawn");
			freeSlots.RemoveAt (index);
			r--;		
		}
		currentBonus = CountBonus ();
		if (freeSlots.Count-r == 0) {
			GameController.EndGame ();
		} 
		FillItemGrid ();
		GameController.Turn+=1;
		if (multTurns > 0)
			multTurns--;
		TurnBoard.GetComponent<Text> ().text = GameController.Turn.ToString();
		RecountCurrentMult ();
		SaveGame ();
	}

	/// <summary>
	/// Generates the type of the brick.
	/// </summary>
	/// <returns>The brick type.</returns>
	BrickType GenerateBrickType(){

		int sum = 0, rand = Random.Range(1, 10000);
		if (rand > 0 && rand <= sum+(int)(GameController.GetLevelValue (Skills.Bomb) * 100)) {
			if (GameController.GetSkillLevel (Skills.Bomb) == 4) {
				if (Random.Range (0, 2) % 2 == 0)
					return BrickType.SuperBomb;
			}
			return BrickType.Bomb;
		}
		sum += (int)(GameController.GetLevelValue (Skills.Bomb) * 100);
		if (rand > sum && rand <= sum+(int)(GameController.GetLevelValue (Skills.Electric) * 100)) {
			if (GameController.GetSkillLevel (Skills.Electric) == 4) {
				if (Random.Range (0, 2) % 2 == 0)
					return BrickType.SuperElectric;
			}
			return BrickType.Electric;
		}
		sum += (int)(GameController.GetLevelValue (Skills.Electric) * 100);
		if (rand > sum && rand <= sum+(int)(GameController.GetLevelValue (Skills.Skull) * 100)) {
			if (GameController.GetSkillLevel (Skills.Skull) == 4) {
				if (Random.Range (0, 2) % 2 == 0)
					return BrickType.SuperSkull;
			}
			return BrickType.Skull;
		}
		sum += (int)(GameController.GetLevelValue (Skills.Skull) * 100);
		if (rand > sum && rand <= sum+(int)(GameController.GetLevelValue (Skills.Double) * 100)) {
			return BrickType.DoubleMult;
		}
		if (rand > sum && rand <= sum + 2000) {
			return BrickType.Coin;
		}
		return BrickType.Standart;
	}

	/// <summary>
	/// Recounts the current mult.
	/// </summary>
	void RecountCurrentMult()
	{
		float doublemult = (multTurns > 0) ? 2.0f : 1.0f;
		currentMult = ((float)(GameController.Turn / 100) * 0.2f + 1.0f)*doublemult;
		Text t = MultBoard.GetComponent<Text> ();
		t.text = "(" + currentMult.ToString("F1") + "x)";
		if (multTurns > 0)
			t.color = new Color (0.93f, 0.23f, 0.23f);
		else
			t.color = new Color (0.4f, 0.4f, 0.4f);
	}

	/// <summary>
	/// Destroies the with animation.
	/// </summary>
	/// <param name="obj">Object for destroying.</param>
	/// <param name="dat">Destroying animation type (see enum).</param>
	/// <param name="isRewarded">If set to <c>true</c> is rewarded.</param>
	/// <param name="isInfluenceOnBonus">If set to <c>true</c> is influence on bonus.</param>
	void DestroyWithAnimation(GameObject obj, DestroyingAnimation dat, bool isRewarded, bool isInfluenceOnBonus)
	{
		if (obj == null)
			return;

		if (obj.name == "del")
			return;

		List<GameObject> targets = new List<GameObject> ();
		bool isInferno = false, isElectric = false;
		obj.name = "del";

		if (isInfluenceOnBonus) {
			currentTurnStreak++;
			for (int i = 0; i < slots.Count; i++) {
				if ((slots [i].GetComponent<RectTransform> ().anchoredPosition - obj.transform.parent.gameObject.GetComponent<RectTransform> ().anchoredPosition).magnitude <= CellWidth * 1.3f && slots [i].transform.childCount > 0 && slots [i].transform.GetChild (0).gameObject.GetComponent<Image> ().color == currentColor && slots[i].transform.GetChild(0).name != "del") {
					
					//Debug.Log ("Slot: " + slots[i].name + "Colors: " + currentTurnStreak.ToString() + " Bonus: " + currentBonus.ToString());
					DestroyWithAnimation (slots [i].transform.GetChild (0).gameObject, DestroyingAnimation.Grinding, true, true);
				}
			}
		}
		if (obj.transform.childCount > 0){
			
			string name = obj.transform.GetChild (0).gameObject.name;

			if (name.Contains ("bomb")) {
				int bomblevel = 0;
				if (name == "bomb1") {
					for (int i = 0; i < slots.Count; i++) {
						if ((slots [i].GetComponent<RectTransform> ().anchoredPosition - obj.transform.parent.gameObject.GetComponent<RectTransform> ().anchoredPosition).magnitude <= CellWidth * 1.7f
						    && slots [i].transform.childCount > 0 && slots [i] != obj.transform.parent) {
							targets.Add (slots [i]);
						}
					}
					bomblevel = 1;
				} else {
					for (int i = 0; i < slots.Count; i++) {
						if ((slots [i].GetComponent<RectTransform> ().anchoredPosition - obj.transform.parent.gameObject.GetComponent<RectTransform> ().anchoredPosition).magnitude <= CellWidth * 2.8f
						    && slots [i].transform.childCount > 0 && slots [i] != obj.transform.parent) {
							targets.Add (slots [i]);
						}
					}
					bomblevel = 2;
				}
				if (GameController.Effects == 1) {
					GameObject ps = Instantiate (Dump.explosionParticle);
					ps.name = "particle";
					TrashCan.Add (ps);
					ps.GetComponent<Transform> ().position = obj.transform.position;
					var main = ps.GetComponent<ParticleSystem> ().main;
					main.startColor = obj.GetComponent<Image> ().color;
					if (bomblevel == 2)
						ps.GetComponent<Transform> ().localScale = new Vector3 (2f, 2f, 2f);
					ps.GetComponent<ParticleSystem> ().Play ();
					GameController.TryPlaySound (SoundPool.Bomb);
				}

			} else if (name.Contains ("electric")) {
				int electriclevel = 0;
				if (name == "electric1") {
					for (int i = 0; i < slots.Count; i++) {
						if (Mathf.Abs (slots [i].GetComponent<RectTransform> ().anchoredPosition.y - obj.transform.parent.gameObject.GetComponent<RectTransform> ().anchoredPosition.y) <= CellWidth * 0.3f
						    && slots [i].transform.childCount > 0 && slots [i] != obj.transform.parent && (slots [i].GetComponent<RectTransform> ().anchoredPosition - obj.transform.parent.gameObject.GetComponent<RectTransform> ().anchoredPosition).magnitude <= CellWidth * 3.1f) {
							targets.Add (slots [i]);
						}
						if (Mathf.Abs (slots [i].GetComponent<RectTransform> ().anchoredPosition.x - obj.transform.parent.gameObject.GetComponent<RectTransform> ().anchoredPosition.x) <= CellWidth * 0.3f
						    && slots [i].transform.childCount > 0 && slots [i] != obj.transform.parent && (slots [i].GetComponent<RectTransform> ().anchoredPosition - obj.transform.parent.gameObject.GetComponent<RectTransform> ().anchoredPosition).magnitude <= CellWidth * 3.1f) {
							targets.Add (slots [i]);
						}
					}
					electriclevel = 1;
				} else {
					for (int i = 0; i < slots.Count; i++) {
						if (Mathf.Abs (slots [i].GetComponent<RectTransform> ().anchoredPosition.y - obj.transform.parent.gameObject.GetComponent<RectTransform> ().anchoredPosition.y) <= CellWidth * 0.3f
						    && slots [i].transform.childCount > 0 && slots [i] != obj.transform.parent) {
							targets.Add (slots [i]);
						}
						if (Mathf.Abs (slots [i].GetComponent<RectTransform> ().anchoredPosition.x - obj.transform.parent.gameObject.GetComponent<RectTransform> ().anchoredPosition.x) <= CellWidth * 0.3f
						    && slots [i].transform.childCount > 0 && slots [i] != obj.transform.parent) {
							targets.Add (slots [i]);
						}
					}
					electriclevel = 2;
				}
				if (GameController.Effects == 1) {
					GameObject ps = null; 
					if (electriclevel == 1)
						ps = Instantiate (Dump.electricParticleSmall);
					else
						ps = Instantiate (Dump.electricParticleBig);
					ps.name = "particle";
					TrashCan.Add (ps);
					ps.GetComponent<Transform> ().position = obj.transform.position;
					var main = ps.GetComponent<ParticleSystem> ().main;
					Color col = obj.GetComponent<Image> ().color;
					main.startColor = col;
					ParticleSystem.MinMaxGradient mmg = new ParticleSystem.MinMaxGradient (new Color (col.r, col.g, col.b, 0.4f));

					for (int i = 0; i < ps.transform.childCount; i++) {
						var trails = ps.transform.GetChild (i).gameObject.GetComponent<ParticleSystem> ().trails;
						trails.colorOverTrail = mmg;
						trails.colorOverLifetime = mmg;
					}
					ps.GetComponent<ParticleSystem> ().Play ();
					GameController.TryPlaySound (SoundPool.Laser);
				}
				isElectric = true;
			} else if (name.Contains ("skull")) {
				if (name == "skull1") {
					for (int i = 0; i < slots.Count; i++) {
						if (slots [i].transform.childCount > 0 && slots [i].transform.GetChild (0).GetComponent<Image> ().color == obj.GetComponent<Image> ().color)
							targets.Add (slots [i]);
					}
				} else
					targets = slots;
				if (targets.Count > 0)
					GameController.TryPlaySound (SoundPool.Skull);
				isInferno = true;
			} else if (name == "coin") {
				if(multTurns>0 && GameController.GetSkillLevel(Skills.Double) == 4)
					GameController.Coins += 2;
				else
					GameController.Coins++;
				CoinBoard.GetComponent<Text> ().text = GameController.Coins.ToString ();
				GameController.SaveCoins ();
				GameController.TryPlaySound (SoundPool.Coin);
			}
			else if (name == "doublemult") {
				multTurns = 10;
				RecountCurrentMult ();
			}
			Destroy (obj.transform.GetChild (0).gameObject);
		}
		if (dat == DestroyingAnimation.None) {
			Destroy (obj);
			for (int i = 0; i < targets.Count; i++) {
				if(isInferno)
					DestroyWithAnimation (targets [i].transform.GetChild (0).gameObject, DestroyingAnimation.Inferno, true, false);
				else
					DestroyWithAnimation (targets [i].transform.GetChild (0).gameObject, DestroyingAnimation.Grinding, true, false);
			}
			return;
		}




		if (dat != DestroyingAnimation.Decreasing && GameController.Effects == 1) {
			GameObject part = Instantiate (Dump.devastateParticle);
			part.name = "particle";
			TrashCan.Add (part);
			part.GetComponent<Transform> ().position = obj.transform.position;
			var mainPart = part.GetComponent<ParticleSystem> ().main;
			if (dat == DestroyingAnimation.Inferno)
				mainPart.startColor = Color.black;
			else
				mainPart.startColor = obj.GetComponent<Image> ().color;
			part.GetComponent<ParticleSystem> ().Play ();
		}
		if(dat == DestroyingAnimation.Inferno)
			obj.GetComponent<Animator> ().Play("Inferno");
		else
			obj.GetComponent<Animator> ().Play("Decreasing");

		for (int i = 0; i < targets.Count; i++) {
			if(targets[i].transform.childCount>0)
			{
				if (isInferno)
					DestroyWithAnimation (targets [i].transform.GetChild (0).gameObject, DestroyingAnimation.Inferno, true, false);
				else if (isElectric)
					StartCoroutine(ElectricDesroyingDelay (targets [i].transform.GetChild (0).gameObject, DestroyingAnimation.Grinding, true, false));
				else
					DestroyWithAnimation (targets [i].transform.GetChild (0).gameObject, DestroyingAnimation.Grinding, true, false);
			}
		}
		if (isRewarded) {
			int points = (int)(GetPointsByColor (obj.GetComponent<Image> ().color)*currentMult);
			GameController.Score += points;
			ScoreBoard.GetComponent<Text> ().text = GameController.Score.ToString ();
			if (GameController.Score > GameController.BestScore) {
				BestScoreBoard.GetComponent<Text> ().text = GameController.Score.ToString ();
			}


			GameObject score = Instantiate (Dump.hitScore);
			score.transform.SetParent (MainLayer);
			score.transform.position = obj.transform.position;
			score.transform.GetChild (0).gameObject.GetComponent<Text> ().text = "+" + points;
			var anim = score.AddComponent<Animator> ();
			anim.runtimeAnimatorController = Dump.scoreController;
			anim.Play ("Score");
			TrashCan.Add (score);
		}
		CurrentColors[(int)Brick.GetBrickColor (obj)]--;
		StartCoroutine (TimerDestroyer (obj));
	}

	/// <summary>
	/// Timers the destroyer.
	/// </summary>
	/// <returns>The destroyer.</returns>
	/// <param name="obj">Object.</param>
	IEnumerator TimerDestroyer(GameObject obj)
	{
		yield return new WaitForSeconds (0.2f);
		Destroy (obj);
	}

	/// <summary>
	/// Electrics the desroying delay.
	/// </summary>
	/// <returns>The desroying delay.</returns>
	/// <param name="obj">Object for destroying.</param>
	/// <param name="da">Destroying animation (see enum).</param>
	/// <param name="isRewarded">If set to <c>true</c> is rewarded.</param>
	/// <param name="isInfluenceOnBonus">If set to <c>true</c> is influence on bonus.</param>
	IEnumerator ElectricDesroyingDelay(GameObject obj, DestroyingAnimation da, bool isRewarded, bool isInfluenceOnBonus)
	{
		yield return new WaitForSeconds (0.25f);
		DestroyWithAnimation (obj, da, isRewarded, isInfluenceOnBonus);
	}

	/// <summary>
	/// Gets the color of the points by.
	/// </summary>
	/// <returns>The points by color.</returns>
	/// <param name="color">Color.</param>
	int GetPointsByColor(Color color)
	{
		if (color == Dump.RedColor)
			return (int)ColorReward.Red;
		if (color == Dump.OrangeColor)
			return (int)ColorReward.Orange;
		if (color == Dump.YellowColor)
			return (int)ColorReward.Yellow;
		if (color == Dump.GreenColor)
			return (int)ColorReward.Green;
		if (color == Dump.BlueColor)
			return (int)ColorReward.Blue;
		if (color == Dump.DarkBlueColor)
			return (int)ColorReward.DarkBlue;
		if (color == Dump.PurpleColor)
			return (int)ColorReward.Purple;
		return 0;
	}

	/// <summary>
	/// Counts the turn.
	/// </summary>
	/// <returns>The turn.</returns>
	/// <param name="freeCount">Free count.</param>
	int CountTurn(int freeCount){ //подавляющий алгоритм ;
		range [6] = 0;
		if(GameController.Turn < 200){
			range [0] = 30; //1
			range [1] = 35; //2
			range [2] = 20; //3
			range [3] = 15; //4
			range [4] = 0;	//5
			range [5] = 0;	//6
		}
		else if(GameController.Turn >= 200 && GameController.Turn < 400){
			range [0] = 25; //1
			range [1] = 30; //2
			range [2] = 25; //3
			range [3] = 20; //4
			range [4] = 0;	//5
			range [5] = 0;	//6
		}
		else if(GameController.Turn >= 400){
			range [0] = 20; //1
			range [1] = 25; //2
			range [2] = 25;  //3
			range [3] = 20; //4
			range [4] = 10;	//5
			range [5] = 0;	//6
		}

		int sum = 0;
		for (int i = 0; i < MAXNUM; i++)
			sum += range [i];
		int rand = Random.Range (1, sum + 1);
		sum = 0;
		for (int i = 0; i < MAXNUM; i++) {
			if (rand > sum && rand <= sum + range [i])
				return i + 1;
			sum += range [i];
		}
		return 0;
	}

	/// <summary>
	/// Counts the bonus.
	/// </summary>
	/// <returns>The bonus.</returns>
	BrickType CountBonus()
	{
		if (currentTurnStreak >= 7) {
			if (GameController.GetSkillLevel (Skills.Skull) == 4) {
				if(Random.Range(0,2)%2 == 0)
					return BrickType.SuperSkull;
			}
			return BrickType.Skull;
		}
		if (currentTurnStreak >= 6) {
			if (GameController.GetSkillLevel (Skills.Electric) == 4) {
				if(Random.Range(0,2)%2 == 0)
					return BrickType.SuperElectric;
			}
			return BrickType.Electric;
		}
		if (currentTurnStreak >= 5) {
			if (GameController.GetSkillLevel (Skills.Bomb) == 4) {
				if (Random.Range (0, 2) % 2 == 0)
					return BrickType.SuperBomb;
			}
			return BrickType.Bomb;
		}
		return BrickType.Standart;
	}	

	/// <summary>
	/// Loads the game.
	/// </summary>
	public void LoadGame(){

		string currentGame = GameController.LoadGameData();
		string[] results = currentGame.Split ('_');
		if (results.GetLength (0) != 4) {
			Debug.Log ("Split failed");
			return;
		}
		GameController.IsSecondChanceUsed = int.Parse (results [0]);
		GameController.Score = int.Parse (results [1]);
		GameController.Turn = int.Parse (results [2]);
				
		if (results [3] != null) {
			int spawnedCount = 0;

			for (int i = 0; i < slots.Count*2; i+=2) {
				
				if (int.Parse (results [3] [i].ToString()) > 0) {
					Brick b = new Brick ((BrickType)int.Parse(results [3] [i + 1].ToString()),(BrickColor)int.Parse(results [3] [i].ToString()));
					b.brick.transform.SetParent (slots [i / 2].transform);
					b.animator.Play ("Spawn");
					spawnedCount++;
				}
			}
			int count = slots.Count * 2;
			for (int i = 0; i < 6; i+=2) {
				if (int.Parse (results [3] [count+i].ToString()) > 0) {
					Brick b = new Brick ((BrickType)int.Parse(results [3] [count + i + 1].ToString()), (BrickColor)int.Parse(results [3] [count+i].ToString()));
					b.brick.AddComponent<CanvasGroup> ();
					b.brick.transform.SetParent (ItemGrid.GetChild(i/2));
					b.scale = Vector3.one;
					b.animator.Play ("Spawn");
					b.brick.AddComponent<DragHandler> ();
				}
			}
			if (spawnedCount == slots.Count) {
				GameController.EndGame ();
			}

		} else {
			FillItemGrid ();
		}
	}

	/// <summary>
	/// Saves the game.
	/// </summary>
	public void SaveGame()
	{
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat ("{0}_{1}_{2}_", GameController.IsSecondChanceUsed, GameController.Score, GameController.Turn);
		for (int i = 0; i < slots.Count; i++) {
			if (slots [i].transform.childCount > 0) {
				GameObject brick = slots [i].transform.GetChild(0).gameObject;
				sb.AppendFormat ("{0}{1}", (int)Brick.GetBrickColor (brick), (int)Brick.GetBrickType (brick));
			} else
				sb.Append("00");
		}
		int count = 0;
		for (int i = 0; i < ItemGrid.childCount; i++) {
			if (ItemGrid.GetChild (i).childCount > 0) {
				GameObject brick = ItemGrid.GetChild (i).GetChild (0).gameObject;
				sb.AppendFormat ("{0}{1}", (int)Brick.GetBrickColor (brick), (int)Brick.GetBrickType (brick));
			} else
				count++;
		}
		for (int i = 0; i < count; i++) {
			sb.Append ("00");
		}
		GameController.SaveGameData (sb.ToString ());
	}

}

/// <summary>
/// I has turn ended.
/// </summary>
namespace UnityEngine.EventSystems
{
	/// <summary>
	/// I has turn ended.
	/// </summary>
	public interface IHasTurnEnded : IEventSystemHandler
	{
		void HasTurnEnded(GameObject go);
	}

}