using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class enemyShooting : MonoBehaviour {
	public Transform [] Nodes;
	public float spot_range;
	public GameObject torso;
	NavMeshAgent nav;
//	shoot gun;
	bool chasing,in_range;
	Vector3 current_node;

	void Start() 
	{
//		gun = torso.GetComponent<shoot> ();
		current_node=transform.position;
		nav = GetComponent<NavMeshAgent> ();
	}

	void Update ()
	{
		check_if_seen_player ();
		if (chasing) 
		{
			shoot ();
			if (isStopped()||!nav.hasPath)
				chasing = false;
		}
		else
		{
			wander ();
		}

	}

	void shoot()
	{
		print (888);
		//torso.transform.rotation = Quaternion.LookRotation (current_node,torso.transform.position);
		//gun.shoot ();
	}

	void wander()
	{
		if (isStopped()) 
		{
			current_node = Nodes [Random.Range (0, Nodes.Length)].position;
			nav.SetDestination (current_node);
		}
	}

	void check_if_seen_player ()
	{
		RaycastHit seen;
		if (Physics.SphereCast (transform.position, 10, torso.transform.rotation.eulerAngles, out seen, spot_range)) 
		{
			if (seen.collider.CompareTag ("Player"))
			{
				current_node = seen.transform.position;
				chasing = true;
			}
		}
	}
	bool isStopped()
	{
		return ((current_node-transform.position).magnitude < 1);
	}
}
