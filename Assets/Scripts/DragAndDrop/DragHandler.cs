using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Drag handler.
/// </summary>
public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public static GameObject itemBeingDragged;
	public static Vector3 startPos;
	public static bool isAnyBlockPicked = false;
	public static Transform startParent;
	private bool isDraggedOnce = false;

	#region IBeginDragHandler implementation
	/// <summary>
	/// Raises the begin drag event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnBeginDrag (PointerEventData eventData){
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

	/// <summary>
	/// Raises the drag event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnDrag (PointerEventData eventData){
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

	/// <summary>
	/// Raises the end drag event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnEndDrag (PointerEventData eventData){
		if (isDraggedOnce)
			return;
		itemBeingDragged = null;
		GetComponent<CanvasGroup> ().blocksRaycasts = true;
		if (transform.parent == startParent) {
			for (var i = 0; i < LevelEditor.ItemGrid.childCount; i++) {
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
