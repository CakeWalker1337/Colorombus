using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Slot : MonoBehaviour, IDropHandler{
	public GameObject item
	{
		get
		{
			if (transform.childCount > 0) {
				return transform.GetChild (0).gameObject;
			}
			return null;
		}
	}


	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
		if (DragHandler.itemBeingDragged == null)
			return;
		if (!item) {
			DragHandler.itemBeingDragged.transform.SetParent (transform);
			ExecuteEvents.ExecuteHierarchy<IHasTurnEnded>(gameObject, null, (x,y) => x.HasTurnEnded(DragHandler.itemBeingDragged));
		}
	}
	#endregion
}
