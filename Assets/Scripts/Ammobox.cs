using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Ammobox : MonoBehaviour {

	void OnTriggerEnter(Collider c)
	{
		WeaponSwitch w = GameObject.FindGameObjectWithTag ("Player_Main").GetComponent<WeaponSwitch>();
		if (w) {
			w.refill ();
			GameObject.FindGameObjectWithTag ("Info").GetComponent<Text> ().text = "Ammo Replenished";
		}
	}

}
