using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

public enum DestroyingAnimation
{
	None = 0,
	Decreasing = 1,
	Grinding = 2,
	Inferno = 3
};

public class LevelEditor : MonoBehaviour, IHasTurnEnded{
	private static List<GameObject> slots;

	GameObject ScoreBoard;
	BrickType currentBonus;
	int currentScore = 0;
	Color orange;
	RectTransform mainGrid, itemGrid;
	public static RectTransform MainLayer{ get; set;}
	float CellWidth, CellHeight;
	List<GameObject> TrashCan;

	// Use this for initialization
	void Start () {
		GameController.Effects = true;
		Dump.Init ();
		ScoreBoard = GameObject.Find ("ScoreBoard");
		slots = new List<GameObject>();
		ScoreBoard.GetComponent<Text> ().text = currentScore.ToString ();
		mainGrid = GameObject.Find ("MainGrid").GetComponent<RectTransform> ();
		itemGrid = GameObject.Find ("ItemGrid").GetComponent<RectTransform> ();
		MainLayer = GameObject.Find ("MainLayer").GetComponent<RectTransform> ();

		TrashCan = new List<GameObject>();

		CellWidth = MainLayer.rect.height * 0.08f;
		CellHeight = CellWidth;
		mainGrid.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CellWidth * 6.3f);
		mainGrid.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CellHeight * 8.3f);
		mainGrid.anchoredPosition = new Vector2 (MainLayer.rect.width/2.0f, MainLayer.rect.height/2.0f);

		itemGrid.gameObject.GetComponent<GridLayoutGroup> ().cellSize = new Vector2 (CellWidth, CellHeight);
		itemGrid.gameObject.GetComponent<GridLayoutGroup> ().spacing = new Vector2 (CellWidth, 10f);


		mainGrid.gameObject.GetComponent<GridLayoutGroup> ().cellSize = new Vector2 (CellWidth, CellHeight);
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
				t.SetParent (mainGrid);	
				slot.AddComponent<Slot> ();

				t.localScale = Vector3.one;

				slots.Add(slot);
			}	
		}

		StartGame ();

	}

	public void FillItemGrid()
	{
		int r = Random.Range (1, 4);
		for (int i = 0; i < 3; i++) {
			Brick b;
			if(r-1 == i) b = new Brick(currentBonus);
			else b = new Brick(BrickType.Standart);
			b.brick.AddComponent<CanvasGroup> ();
			b.brick.transform.SetParent (itemGrid);
			b.scale = Vector3.one;
			b.animator.Play ("Spawn");
			b.brick.AddComponent<DragHandler> ();
		}
	}



	void StartGame()
	{
		foreach (GameObject sl in slots) {
			if(sl.transform.childCount > 0)
				Destroy(sl.transform.GetChild(0).gameObject);
		}
		foreach (Transform item in itemGrid) {
			Destroy (item.gameObject);
		}
		FillItemGrid ();

	}

	void Update () {
		
	}
		
	#region IHasTurnEnded implementation
	public void HasTurnEnded (GameObject currentItem)
	{
		for (int i = 0; i < itemGrid.childCount; i++) {
			DestroyWithAnimation (itemGrid.GetChild(i).gameObject, DestroyingAnimation.Decreasing, false);
		}
			
		for (int i = 0; i < TrashCan.Count; i++) {
			Destroy (TrashCan[i]);
		}
		TrashCan.Clear ();

		List<GameObject> targets = new List<GameObject> ();
		GameObject currentSlot = currentItem.transform.parent.gameObject;
		for (int i = 0; i < slots.Count; i++) {
			if ((slots [i].GetComponent<RectTransform>().anchoredPosition - currentSlot.GetComponent<RectTransform>().anchoredPosition).magnitude <= CellWidth * 1.3f && slots[i].transform.childCount>0) {
				targets.Add (slots [i]);
			}
		}
		if (targets.Count > 1) {
			currentBonus = CountBonus (targets);
			for (int i = 0; i < targets.Count; i++) {
				if (targets [i].transform.childCount > 0) {
					DestroyWithAnimation (targets [i].transform.GetChild (0).transform.gameObject, DestroyingAnimation.Grinding, true);
				}
			}
		}
		StartCoroutine (NewGeneration (0.5f));
	}
	#endregion

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
			Brick b = new Brick (BrickType.Standart);
			int index = Random.Range (0, freeSlots.Count - 1);
			b.brick.transform.SetParent (freeSlots [index]);
			b.animator.Play ("Spawn");
			freeSlots.RemoveAt (index);
			r--;		
		}

		if (freeSlots.Count-r == 0) {
			StartGame ();
		} 
		FillItemGrid ();
	}


	void DestroyWithAnimation(GameObject obj, DestroyingAnimation dat, bool isRewarded)
	{
		if (obj == null)
			return;
		if (obj.name == "del")
			return;
		List<GameObject> targets = new List<GameObject> ();
		bool isInferno = false;
		if (obj.transform.childCount > 0){
			
			string name = obj.transform.GetChild (0).gameObject.name;

			if (name == "bomb") {
				for (int i = 0; i < slots.Count; i++) {
					if ((slots [i].GetComponent<RectTransform> ().anchoredPosition - obj.transform.parent.gameObject.GetComponent<RectTransform> ().anchoredPosition).magnitude <= CellWidth * 1.7f
					    && slots [i].transform.childCount > 0 && slots [i] != obj.transform.parent) {
						targets.Add (slots [i]);
					}
				}
				if (GameController.Effects) {
					GameObject ps = Instantiate (Dump.explosionParticle);
					ps.name = "particle";
					TrashCan.Add (ps);
					ps.GetComponent<Transform> ().position = obj.transform.position;
					var main = ps.GetComponent<ParticleSystem> ().main;
					main.startColor = obj.GetComponent<Image> ().color;
					ps.GetComponent<ParticleSystem> ().Play ();
				}

			} else if (name == "electric") {
				for (int i = 0; i < slots.Count; i++) {
					if (Mathf.Abs(slots [i].GetComponent<RectTransform> ().anchoredPosition.y - obj.transform.parent.gameObject.GetComponent<RectTransform> ().anchoredPosition.y) <= CellWidth * 0.3f
					    && slots [i].transform.childCount > 0 && slots [i] != obj.transform.parent) {
						targets.Add (slots [i]);
					}
					if (Mathf.Abs(slots [i].GetComponent<RectTransform> ().anchoredPosition.x - obj.transform.parent.gameObject.GetComponent<RectTransform> ().anchoredPosition.x) <= CellWidth * 0.3f
					    && slots [i].transform.childCount > 0 && slots [i] != obj.transform.parent) {
						targets.Add (slots [i]);
					}
				}
				GameObject ps = Instantiate (Dump.devastateParticle);
				ps.name = "particle";
				TrashCan.Add (ps);
				ps.GetComponent<Transform> ().position = obj.transform.position;
				var main = ps.GetComponent<ParticleSystem> ().main;
				main.startColor = obj.GetComponent<Image> ().color;
				ps.GetComponent<ParticleSystem> ().Play ();

			} else if (name == "skull") {
				for (int i = 0; i < slots.Count; i++) {
					if (slots [i].transform.childCount > 0 && slots [i].transform.GetChild (0).gameObject.GetComponent<Image> ().color == obj.GetComponent<Image> ().color && slots [i] != obj.transform.parent)
						targets.Add (slots [i]);
				}
				isInferno = true;
			}
			Destroy (obj.transform.GetChild (0).gameObject);
		}
		if (dat == DestroyingAnimation.None) {
			obj.name = "del";
			Destroy (obj);
			for (int i = 0; i < targets.Count; i++) {
				if(isInferno)
					DestroyWithAnimation (targets [i].transform.GetChild (0).gameObject, DestroyingAnimation.Inferno, true);
				else
					DestroyWithAnimation (targets [i].transform.GetChild (0).gameObject, DestroyingAnimation.Grinding, true);
			}
			return;
		}

		obj.name = "del";


		if (dat != DestroyingAnimation.Decreasing && GameController.Effects) {
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
				if(isInferno)
					DestroyWithAnimation (targets [i].transform.GetChild (0).gameObject, DestroyingAnimation.Inferno, true);
				else
					DestroyWithAnimation (targets [i].transform.GetChild (0).gameObject, DestroyingAnimation.Grinding, true);
			}
		}
		if (isRewarded) {
			int points = GetPointsByColor (obj.GetComponent<Image> ().color);
			currentScore += points;
			ScoreBoard.GetComponent<Text> ().text = currentScore.ToString ();

			GameObject score = Instantiate (Dump.hitScore);
			score.transform.SetParent (MainLayer);
			score.transform.position = obj.transform.position;
			score.transform.GetChild (0).gameObject.GetComponent<Text> ().text = "+" + points;
			var anim = score.AddComponent<Animator> ();
			anim.runtimeAnimatorController = Dump.scoreController;
			anim.Play ("Score");
			TrashCan.Add (score);
		}

		StartCoroutine (TimerDestroyer (obj));
	}

	IEnumerator TimerDestroyer(GameObject obj)
	{
		yield return new WaitForSeconds (0.2f);
		Destroy (obj);
	}

	int GetPointsByColor(Color color)
	{
		if (color == Color.red)
			return (int)ColorReward.Red;
		if (color == Dump.OrangeColor)
			return (int)ColorReward.Orange;
		if (color == Color.yellow)
			return (int)ColorReward.Yellow;
		if (color == Color.green)
			return (int)ColorReward.Green;
		if (color == Color.cyan)
			return (int)ColorReward.Blue;
		if (color == Color.blue)
			return (int)ColorReward.DarkBlue;
		if (color == Color.magenta)
			return (int)ColorReward.Purple;
		return 0;
	}

	int CountTurn(int freeCount){
		return Random.Range (1, 6);
	}

	BrickType CountBonus(List<GameObject> targets)
	{
		int[] colors = new int[7];
		Color currentColor;
		for (int i = 0; i < targets.Count; i++) {
			currentColor = targets [i].transform.GetChild (0).GetComponent<Image> ().color;
			if(currentColor == Color.red)
				colors [0]++;
			else if(currentColor == Dump.OrangeColor)
				colors [1]++;
			else if(currentColor == Color.yellow)
				colors [2]++;
			else if(currentColor == Color.green)
				colors [3]++;
			else if(currentColor == Color.cyan)
				colors [4]++;
			else if(currentColor == Color.blue)
				colors [5]++;
			else if(currentColor == Color.magenta)
				colors [6]++;
		}
		int max = 0;
		for (int i = 0; i < 7; i++) {
			if (colors [i] > max)
				max = colors [i];
		}
		if (max == 5)
			return BrickType.Skull;
		if (max == 4)
			return BrickType.Electric;
		if (targets.Count == 5)
			return BrickType.Bomb;
		return BrickType.Standart;
	}	
}
	
namespace UnityEngine.EventSystems
{
	public interface IHasTurnEnded : IEventSystemHandler
	{
		void HasTurnEnded(GameObject go);
	}

}