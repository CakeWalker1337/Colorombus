using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public static GameObject itemBeingDragged;
	public static Vector3 startPos;
	public static bool isAnyBlockPicked = false;
	public static Transform startParent;
	private bool isDraggedOnce = false;

	#region IBeginDragHandler implementation
	public void OnBeginDrag (PointerEventData eventData)
	{
		if (isDraggedOnce)
			return;
		if (isAnyBlockPicked)
			return;
		isAnyBlockPicked = true;
		itemBeingDragged = gameObject;
		startPos = transform.position;
		transform.SetParent (LevelEditor.MainLayer);
		startParent = transform.parent;
		GetComponent<CanvasGroup> ().blocksRaycasts = false;
	}
	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		if (isDraggedOnce)
			return;
		if (itemBeingDragged != gameObject)
			return;
		transform.position = Input.mousePosition;
		Vector2 pos;

		RectTransformUtility.ScreenPointToLocalPointInRectangle(LevelEditor.MainLayer, Input.mousePosition, LevelEditor.MainLayer.gameObject.GetComponent<Canvas>().worldCamera, out pos);
		transform.position = LevelEditor.MainLayer.transform.TransformPoint(pos);
	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
		if (isDraggedOnce)
			return;
		itemBeingDragged = null;
		GetComponent<CanvasGroup> ().blocksRaycasts = true;
		if (transform.parent == startParent) {
			for (int i = 0; i < LevelEditor.ItemGrid.childCount; i++) {
				if (LevelEditor.ItemGrid.GetChild (i).childCount == 0) {
					transform.SetParent(LevelEditor.ItemGrid.GetChild(i));
					isAnyBlockPicked = false;
					return;
				}
			}

		} else {
			isDraggedOnce = true;
			isAnyBlockPicked = false;
		}
	}

	#endregion

}
