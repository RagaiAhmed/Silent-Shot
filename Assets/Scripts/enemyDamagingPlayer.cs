using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyDamagingPlayer : MonoBehaviour 
{
	public float playerHealth = 200f;
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.name == "player") 
		{
			playerHealth -= 1f;
			if (playerHealth <= 3f) 
			{
				Destroy (col.gameObject);
				Time.timeScale = 0.0f;
			}
		}
	}
}
