using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class takeDamageFromEnemy : MonoBehaviour 
{
	public float health = 200f;
	public Slider healthslider;
	// Update is called once per frame
	void Update()
	{
		healthslider.value = health/2f;
	}
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.name == "box enemy" || col.gameObject.name == "box enemy(Clone)") 
		{
			damage_me (1);
		}
	}
	public void damage_me(float damage){
		health -= damage;
		if (health <= 0f) {
			gameObject.SetActive (false);
		}
	}
}
