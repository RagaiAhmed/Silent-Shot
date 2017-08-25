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
	bool tracing_player;
	public bool escaper;
	public Transform escape_node;
	bool escaping;
	public bool escaped;

	void Start() 
	{
		anim = GetComponent<Animator> ();
		current_node=transform.position;
		nav = GetComponent<NavMeshAgent> ();
		if (!nav.isOnNavMesh) 
		{
			Destroy (gameObject);
		}
	}

	void LateUpdate ()
	{
		anim.SetBool ("Walking",nav.velocity.magnitude<0.5f);
		check_if_seen_player (transform.forward);
		check_if_seen_player (transform.forward+transform.right);
		check_if_seen_player (transform.forward-transform.right);




		if (escaping) 
		{
			if (Vector3.Distance (transform.position, current_node) < 5)
			{
				escaped = true;
			}

		}
		else if (chasing) 
		{
			if (escaper) 
			{
				nav.speed = 4;
				current_node = escape_node.position;
				nav.SetDestination (current_node);
				anim.SetTrigger ("Run");
				anim.speed = 4;
				escaping = true;
			}
			else
			{
				nav.isStopped = true;

				shoot ();
				if (!(check_if_seen_player (transform.forward)||check_if_seen_player (transform.right)||check_if_seen_player (-transform.right))) 
				{
					chasing = false;
					current_node = player_pos;
					tracing_player = true;
				}
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
			if (Nodes.Length > 0) 
			{
				index = Random.Range (0, Nodes.Length);
				if (Nodes [index]!=null) 
				{
					current_node = Nodes [index].position;
					tracing_player = false;
				}
			}

		}
		else 
		{
			if (!tracing_player) 
			{
				if (Nodes[index]!=null)
					current_node=Nodes [index].position;
				
			}
		}
	}

	bool check_if_seen_player (Vector3 direction)
	{
		RaycastHit seen;
		if (Physics.SphereCast (transform.position, 5,direction, out seen, spot_range)) 
		{
			if (seen.collider.CompareTag ("Player")) 
			{
				player_pos = seen.transform.position;
				chasing = true;
				return true;
			}
		}
		else if (isStopped ())
			chasing = false;
		return false;
	}

	bool isStopped()
	{
		return (Vector3.Distance(transform.position,current_node)<5||!nav.hasPath);
	}
}
