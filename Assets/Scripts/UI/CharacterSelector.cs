using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterSelector : MonoBehaviour {

	void Start()
	{
		int i = PlayerPrefs.GetInt ("Character", -1);
		if (i != -1) 
		{			
			transform.GetChild (i).GetComponent<Image> ().color = Color.cyan;
		}
	}

	public void Select(int n)
	{
		int i = PlayerPrefs.GetInt ("Character", -1);
		if (i!=-1)
			transform.GetChild (i).GetComponent<Image> ().color = Color.white;
		transform.GetChild (n).GetComponent<Image> ().color = Color.cyan;
		PlayerPrefs.SetInt ("Character", n);

	}
}
