using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapSlot : MonoBehaviour , IPointerClickHandler {


	
	public void OnPointerClick(PointerEventData event_data)
	{
		transform.parent.parent.parent.GetComponent<Map_Shower> ().Select (transform.parent.GetSiblingIndex ());
	}
}
