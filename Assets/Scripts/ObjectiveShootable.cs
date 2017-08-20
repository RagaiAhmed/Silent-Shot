using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ObjectiveShootable : MonoBehaviour {
	public float health = 200f;
	public Transform start;
	public Transform end;
	NavMeshAgent nav;
	void Start(){
		nav = GetComponent<NavMeshAgent> ();
	}
	public void damage_object (float damageamount)
	{
		health -= damageamount;
		if (health != 200f) {
			nav.SetDestination(start.position);
		} else if(health == 200f) {
			nav.SetDestination(end.position);
		}

	}
}