using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShowingWeapons : MonoBehaviour 
{
	void Start()
	{
		int i = PlayerPrefs.GetInt ("Primary", -1);
		if (i != -1) 
		{			
			transform.GetChild (0).GetChild (i).GetComponent<Image> ().color = Color.cyan;
		}
		i = PlayerPrefs.GetInt ("Secondary", -1);
		if (i != -1) 
		{			
			transform.GetChild (1).GetChild (i).GetComponent<Image> ().color = Color.cyan;
		}
	}

	public void Select (int index,int num)
	{
		int old;
		if (index == 0)
		{
			old = PlayerPrefs.GetInt ("Primary",-1);
			if (old != -1)
				transform.GetChild (0).GetChild (old).GetComponent<Image> ().color = Color.grey;
			PlayerPrefs.SetInt ("Primary", num);
			transform.GetChild (0).GetChild (num).GetComponent<Image> ().color = Color.cyan;
		}
		else
		{
			old = PlayerPrefs.GetInt ("Secondary",-1);
			if (old != -1)
				transform.GetChild (1).GetChild (old).GetComponent<Image> ().color = Color.grey;
			PlayerPrefs.SetInt ("Secondary", num);
			transform.GetChild (1).GetChild (num).GetComponent<Image> ().color = Color.cyan;
		}


	}
}
