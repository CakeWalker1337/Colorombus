using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BrickType
{
	Standart = 0,
	Bomb = 1,
	Electric = 2,
	Skull = 3
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

	public Brick(BrickType bt){
		brick = new GameObject();
		brick.layer = 5;
		Image im = brick.AddComponent<Image> ();
		im.sprite = Dump.rectSprite;
		im.color = PickRandomColor ();
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
				skill.name = "bomb";
				skillImage.sprite = Dump.bombSprite;
			}
			else if (bt == BrickType.Electric) {
				skill.name = "electric";
				skillImage.sprite = Dump.electricSprite;
			}
			else{
				skill.name = "skull";
				skillImage.sprite = Dump.skullSprite;
			}
		}

	}

	private Color PickRandomColor()
	{
		int res = Random.Range(0, 7);
		switch (res) {
		case 0:
			return Color.red;
		case 1:
			return Dump.OrangeColor;
		case 2:
			return Color.yellow;
		case 3:
			return Color.green;
		case 4:
			return Color.cyan;
		case 5:
			return Color.blue;
		case 6:
			return Color.magenta;
		default:
			return new Color();
		}
	}
}
