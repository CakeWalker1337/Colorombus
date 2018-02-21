using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Background move.
/// </summary>
public class BackgroundMove : MonoBehaviour {

	private Vector3 pos;
	private int currentTarget;
	// Use this for initialization
	void Start () {
		GameObject point1 = GameObject.Find ("ImagePoint2");
		pos = point1.transform.position;
		currentTarget = 2;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (Vector3.Normalize(pos - transform.position)*Time.deltaTime * 100);
		if (Mathf.Abs(transform.position.x - pos.x) < 1 && Mathf.Abs(transform.position.y - pos.y) < 1) {
			if (currentTarget == 2) {
				GameObject point = GameObject.Find ("ImagePoint3");
				pos = point.transform.position;
				currentTarget = 3;
			} 
			else if (currentTarget == 3){
				GameObject point = GameObject.Find ("ImagePoint4");
				pos = point.transform.position;
				currentTarget = 4;
			}
			else if (currentTarget == 4){
				GameObject point = GameObject.Find ("ImagePoint1");
				pos = point.transform.position;
				currentTarget = 1;
			}
			else if (currentTarget == 1){
				GameObject point = GameObject.Find ("ImagePoint2");
				pos = point.transform.position;
				currentTarget = 2;
			}
		}
	}
}
