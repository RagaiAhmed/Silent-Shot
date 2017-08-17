using System.Collections;
using UnityEngine;

public class switchingweapons1 : MonoBehaviour {
	public int selected = 0;
	// Use this for initialization
	void Start () 
	{
		switchweapons ();
	}
	// Update is called once per frame
	void Update () 
	{
		int previous = selected - 1;
		if (Input.GetKeyDown (KeyCode.Alpha1)) 
		{
			selected = 0;
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) 
		{
			selected = 1;
		}
		if (previous != selected) 
		{
			switchweapons ();
		}
	}
	void switchweapons()
	{
		int i = 0;
		foreach(Transform weapon in transform)
		{
			if (i == selected) {
				weapon.gameObject.SetActive (true);
			} 

			else 
			{
				weapon.gameObject.SetActive (false);
			}
			i++;
		} 
	}
}
