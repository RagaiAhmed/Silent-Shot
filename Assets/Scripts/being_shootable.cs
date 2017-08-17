using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class being_shootable : MonoBehaviour {
	public float health = 100f;

	public void damage_object (float damageamount)
	{
		health -= damageamount;
		if (health <= 0) {
			gameObject.SetActive (false);
		}
	}
}