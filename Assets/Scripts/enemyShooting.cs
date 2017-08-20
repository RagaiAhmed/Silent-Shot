using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class enemyShooting : MonoBehaviour {
	public Transform [] Nodes;
	public float spot_range;
	public GameObject torso;
	NavMeshAgent nav;
	public Shooting gun;
	bool chasing,in_range;
	Vector3 current_node;
	Vector3 player_pos;
	Animator anim;
	bool didnt_know;
	int index;

	void Start() 
	{
		anim = GetComponent<Animator> ();
		current_node=transform.position;
		nav = GetComponent<NavMeshAgent> ();
	}

	void LateUpdate ()
	{
		check_if_seen_player ();
	
		if (chasing) 
		{
			nav.isStopped = true;

			shoot ();
			if (!check_if_seen_player ()) 
			{
				chasing = false;
				current_node = player_pos;
			}
		}
		else
		{
			anim.SetBool ("Shooting", false);
			nav.isStopped = false;
			wander ();
			nav.SetDestination (current_node);

		}

	}

	void shoot()
	{
		anim.SetBool ("Shooting", true);
		torso.transform.eulerAngles = Quaternion.LookRotation (player_pos - torso.transform.position).eulerAngles + new Vector3 (0, 30); 
		if (!gun.reloading && gun.total_ammo > 0) 
		{
			didnt_know = true;
			gun.shoot (player_pos);
		}
		else if (gun.reloading&&didnt_know) 
		{
			didnt_know = false;
			anim.SetTrigger ("Reloading");
		}
	}

	void wander()
	{
		if (isStopped ()) 
		{
			index = Random.Range (0, Nodes.Length);
			current_node = Nodes [index].position;
		}
		else 
		{
			current_node=Nodes [index].position;
		}
	}

	bool check_if_seen_player ()
	{
		RaycastHit seen;
		if (Physics.SphereCast (transform.position, 5, transform.forward, out seen, spot_range)) {
			if (seen.collider.CompareTag ("Player")) {
				player_pos = seen.transform.position;
				chasing = true;
				return true;
			}
		} else if (isStopped ())
			chasing = false;
		return false;
	}

	bool isStopped()
	{
		return (Vector3.Distance(transform.position,current_node)<1||!nav.hasPath);
	}
}
