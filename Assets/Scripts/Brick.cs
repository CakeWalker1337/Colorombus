using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BrickType
{
	Error = 0,
	Standart = 1,
	Bomb = 2,
	Electric = 3,
	Skull = 4,
	SuperBomb = 5,
	SuperElectric = 6,
	SuperSkull = 7,
	DoubleMult = 8,
	Coin = 9
};

public enum BrickColor
{
	None = 0,
	Red = 1,
	Orange = 2,
	Yellow = 3,
	Green = 4,
	Blue = 5,
	DarkBlue = 6,
	Purple = 7,
	Random = 8,
	Error = 9
};

public class Brick {

	public GameObject brick{ get; set;}
	public Color color
	{
		get{
			return brick.GetComponent<Image> ().color;	
		}
		set
		{
			brick.GetComponent<Image> ().color = value;
		}
	}

	public Vector3 position
	{
		get{
			return brick.GetComponent<RectTransform> ().position;	
		}
		set
		{
			brick.GetComponent<RectTransform> ().position = value;
		}
	}

	public Vector3 scale
	{
		get{
			return brick.GetComponent<RectTransform> ().localScale;	
		}
		set
		{
			brick.GetComponent<RectTransform> ().localScale = value;
		}
	}

	public Animator animator
	{
		get{
			return brick.GetComponent<Animator> ();	
		}
	}

	public Brick(BrickType bt, BrickColor bc){
		brick = new GameObject();
		brick.layer = 5;
		Image im = brick.AddComponent<Image> ();
		im.sprite = Dump.rectSprite;
		im.color = PickBrickColor (bc);
		var anim = brick.AddComponent<Animator> ();
		anim.runtimeAnimatorController = Dump.spawnController;
		if (bt != BrickType.Standart) {
			var grid = brick.AddComponent<GridLayoutGroup> ();
			grid.childAlignment = TextAnchor.MiddleCenter;
			grid.cellSize = new Vector2 (brick.GetComponent<RectTransform> ().rect.width * 0.6f, brick.GetComponent<RectTransform> ().rect.height * 0.6f);

			GameObject skill = new GameObject ();
			skill.layer = 5;
			skill.transform.SetParent (brick.transform);
			Image skillImage = skill.AddComponent<Image> ();
			if (bt == BrickType.Bomb) {
				skill.name = "bomb1";
				skillImage.sprite = Dump.bombSprite;
			}
			else if (bt == BrickType.Electric) {
				skill.name = "electric1";
				skillImage.sprite = Dump.electricSprite;
			}
			else if(bt == BrickType.Skull){
				skill.name = "skull1";
				skillImage.sprite = Dump.skullSprite;
			}
			else if (bt == BrickType.SuperBomb) {
				skill.name = "bomb2";
				skillImage.sprite = Dump.superBombSprite;
			}
			else if (bt == BrickType.SuperElectric) {
				skill.name = "electric2";
				skillImage.sprite = Dump.superElectricSprite;
			}
			else if(bt == BrickType.SuperSkull){
				skill.name = "skull2";
				skillImage.sprite = Dump.superSkullSprite;
			}
			else if(bt == BrickType.DoubleMult){
				skill.name = "doublemult";
				skillImage.sprite = Dump.doubleMultSprite;
			}
			else if(bt == BrickType.Coin){
				skill.name = "coin";
				skillImage.sprite = Dump.coinSprite;
			}
		}

	}

	private Color PickBrickColor(BrickColor bc)
	{
		int res = -1;
		if (bc == BrickColor.Random)
			res = Random.Range (1, 8);
		else
			res = (int)bc;
		LevelEditor.CurrentColors [res]++;
		switch (res) {
		case 1:
			return Dump.RedColor;
		case 2:
			return Dump.OrangeColor;
		case 3:
			return Dump.YellowColor;
		case 4:
			return Dump.GreenColor;
		case 5:
			return Dump.BlueColor;
		case 6:
			return Dump.DarkBlueColor;
		case 7:
			return Dump.PurpleColor;
		default:
			return new Color();
		}
	}

	public static BrickColor GetBrickColor(GameObject brick){
		Color c = brick.GetComponent<Image> ().color;
		if (c == new Color ())
			return BrickColor.None;
		if (c == Dump.RedColor)
			return BrickColor.Red;
		if (c == Dump.OrangeColor)
			return BrickColor.Orange;
		if (c == Dump.YellowColor)
			return BrickColor.Yellow;
		if (c == Dump.GreenColor)
			return BrickColor.Green;
		if (c == Dump.BlueColor)
			return BrickColor.Blue;
		if (c == Dump.DarkBlueColor)
			return BrickColor.DarkBlue;
		if (c == Dump.PurpleColor)
			return BrickColor.Purple;
		return BrickColor.Error;
	}

	public static BrickType GetBrickType(GameObject brick){
		if (brick.transform.childCount == 0)
			return BrickType.Standart;
		GameObject child = brick.transform.GetChild(0).gameObject;
		if (child.name == "bomb1")
			return BrickType.Bomb;
		if (child.name == "electric1")
			return BrickType.Electric;
		if (child.name == "skull1")
			return BrickType.Skull;
		if (child.name == "bomb2")
			return BrickType.SuperBomb;
		if (child.name == "electric2")
			return BrickType.SuperElectric;
		if (child.name == "skull2")
			return BrickType.SuperSkull;
		if (child.name == "doublemult")
			return BrickType.DoubleMult;
		if (child.name == "coin")
			return BrickType.Coin;
		return BrickType.Error;
	}

}
