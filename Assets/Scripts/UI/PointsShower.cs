using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PointsShower : MonoBehaviour {

	Text t;
	void Start () 
	{
		t = GetComponent<Text> ();
	}
	
	void Update () 
	{
		t.text="Your Points : "+PlayerPrefs.GetInt("Points",0);
	}
}
