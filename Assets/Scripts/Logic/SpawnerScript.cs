using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour {


	public GameObject[] chars;
	public GameObject[] primary;
	public GameObject[] Secondary;
	public GameObject knife;

	void Start()
	{
		GameObject player = Instantiate(chars[PlayerPrefs.GetInt("Character")],transform.position,transform.rotation);
		Transform weaponHolder = player.transform;
		foreach (int i in player.GetComponent<WeaponSwitch>().WeaponHolder_path) 
		{
			weaponHolder = weaponHolder.GetChild (i);
		}
		Instantiate (primary [PlayerPrefs.GetInt ("Primary")], weaponHolder);
		Instantiate (Secondary [PlayerPrefs.GetInt ("Secondary")], weaponHolder);
		Instantiate (knife, weaponHolder);

	}
}
