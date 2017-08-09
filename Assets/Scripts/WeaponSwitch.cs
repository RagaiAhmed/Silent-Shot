using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitch : MonoBehaviour {

	public Transform WeaponHolder;
	public int DefaultWeapon;

	int current = 0;

	KeyCode[] numberkeys = 
	{
		KeyCode.Alpha1,
		KeyCode.Alpha2,
		KeyCode.Alpha3,
		KeyCode.Alpha4,
		KeyCode.Alpha5,
		KeyCode.Alpha6,
		KeyCode.Alpha7,
		KeyCode.Alpha8,
		KeyCode.Alpha9
	};

	void Start()
	{
		switchto (DefaultWeapon);
	}

	void Update ()
	{
		if (Time.timeScale > 0)
		{
			for (int i = 0; i < 9; i++) 
			{
				if (Input.GetKey (numberkeys [i]))
					switchto (i);
			}
			float state = Input.GetAxis ("Mouse ScrollWheel");
			if ( state > 0)
				switchto ((current + 1) % WeaponHolder.childCount);
			else if (state < 0) 
				switchto((current-1) % WeaponHolder.childCount);
		}
		
	}

	void switchto(int index)
	{
		if (current != index && index < WeaponHolder.childCount && index >=0 ) 
		{
			WeaponHolder.GetChild (current).gameObject.SetActive (false);
			WeaponHolder.GetChild (index)  .gameObject.SetActive (true) ;
			current = index;
		}
	}

}
