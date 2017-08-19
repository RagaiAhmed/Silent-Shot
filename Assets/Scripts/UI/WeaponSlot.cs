using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class WeaponSlot : MonoBehaviour,IPointerClickHandler {

	
	ShowingWeapons main;
	int parent_number;
	int number;


	void Start()
	{
		main = transform.parent.parent.parent.GetComponent<ShowingWeapons> ();
		parent_number = transform.parent.parent.GetSiblingIndex();
		number = transform.parent.GetSiblingIndex();
	}

	public void OnPointerClick (PointerEventData event_data)
	{
		main.Select (parent_number,number);
	}

}
