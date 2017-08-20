using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class CharSlot : MonoBehaviour , IPointerClickHandler  {

	CharacterSelector main;
	int number;

	void Start()
	{
		main = transform.parent.GetComponent<CharacterSelector> ();
		number = transform.GetSiblingIndex();
	}

	public void OnPointerClick (PointerEventData event_data)
	{
		main.Select (number);
	}
		
}
