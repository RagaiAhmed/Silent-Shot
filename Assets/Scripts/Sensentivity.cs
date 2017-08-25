using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sensentivity : MonoBehaviour {

	public float range;
	// Use this for initialization
	void Start ()
	{
		GetComponent<Slider> ().onValueChanged.AddListener (adjust_sens);
	}

	void adjust_sens(float done)
	{
		GameObject.FindGameObjectWithTag ("Player_Main").GetComponent<Head_Movement> ().Sensitivity = done * (range-1)+1;
	}

}
