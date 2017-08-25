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
		GameObject player = GameObject.FindGameObjectWithTag ("Player_Main");
		if (player)
			player.GetComponent<Head_Movement> ().Sensitivity = done * (range);
	}

	void OnEnable()
	{
		GameObject player = GameObject.FindGameObjectWithTag ("Player_Main");
		if (player) 
		{
			GetComponent<Slider> ().value = player.GetComponent<Head_Movement> ().Sensitivity /range;
				
		}
	}
}
