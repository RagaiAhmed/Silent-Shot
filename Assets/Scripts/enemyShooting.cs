using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class enemyShooting : MonoBehaviour {
	public Transform player;
	public float minDist = 15f;
	public float enemyDamage = 2f;
	public NavMeshAgent nav;
	void Start() 
	{
		nav = GetComponent<NavMeshAgent> ();
	}
	// Update is called once per frame
	void Update () {
		float dist = Vector3.Distance (gameObject.transform.position, player.transform.position);
		if (dist <= minDist) {
			takeDamageFromEnemy health = player.GetComponent<takeDamageFromEnemy> ();
			health.damage_me (enemyDamage);
			nav.enabled = false;
		} else {
			nav.enabled = true;
		}
	}
}
