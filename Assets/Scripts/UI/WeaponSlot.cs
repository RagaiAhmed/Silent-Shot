using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;
public class WeaponSlot : MonoBehaviour,IPointerClickHandler {

	
	ShowingWeapons main;
	int parent_number;
	int number;
	public bool available;
	public int price;

	void OnEnable()
	{
		main = transform.parent.parent.parent.GetComponent<ShowingWeapons> ();
		parent_number = transform.parent.parent.GetSiblingIndex();
		number = transform.parent.GetSiblingIndex();
		if (!available)
		{
			if (PlayerPrefs.GetInt (transform.parent.parent.name + transform.parent.name, 0) != 0) 
			{
				available = true;
				transform.parent.GetComponent<Image> ().color = Color.gray;
			}
			else
			{
				transform.GetChild (0).GetComponent<Text> ().text = price+" $";
			}
		}
		if (available)
		{
			Destroy (transform.GetChild (0).gameObject);
		}
	}

	public void OnPointerClick (PointerEventData event_data)
	{
		if (available)
			main.Select (parent_number, number);
		else
		{
			int points = PlayerPrefs.GetInt ("Points", 0);
			if (points >= price)
			{
				PlayerPrefs.SetInt ("Points",points-price);
				available = true;
				PlayerPrefs.SetInt (transform.parent.parent.name + transform.parent.name, 1);
				transform.parent.GetComponent<Image> ().color = Color.gray;
				Destroy (transform.GetChild (0).gameObject);

			}
		}
	}

}
