using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour {


	public GameObject[] chars;
	public GameObject[] primary;
	public GameObject[] Secondary;
	public GameObject knife;
	public GameObject Car;
	public GameObject Police;


	void Start()
	{
		GameObject player = Instantiate(chars[PlayerPrefs.GetInt("Character")],transform.position,transform.rotation);
		Transform weaponHolder = player.transform;
		foreach (int i in player.GetComponent<WeaponSwitch>().WeaponHolder_path) 
		{
			weaponHolder = weaponHolder.GetChild (i);
		}
		int m = PlayerPrefs.GetInt ("Primary",-1);
		if (m!=-1)
			Instantiate (primary [m], weaponHolder);
		m = PlayerPrefs.GetInt ("Secondary",-1);
		if (m!=-1)
			Instantiate (Secondary [m], weaponHolder);
		Instantiate (knife, weaponHolder);

	}
}
