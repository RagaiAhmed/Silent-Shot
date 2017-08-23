using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseSpawn : MonoBehaviour {

	public float spawnTime = 5.5f;
	public GameObject police;
	public GameObject car;
	public float range=10;
	void Start () 
	{
		InvokeRepeating ("spawn", spawnTime, spawnTime);
	}
	void spawn ()
	{
		Vector3 spawnPoint;
		GameObject spawned;

		spawnPoint = transform.position + new Vector3 (Random.Range (-range, range), 0, Random.Range (-range, range));
		if (!(Physics.OverlapSphere (spawnPoint, 0.2f).Length > 0)) 
		{
			spawned = Instantiate (police, spawnPoint, Quaternion.LookRotation (transform.position - spawnPoint));
			spawned.GetComponent<enemyShooting>().Nodes=new Transform[1] {transform};
			Destroy (spawned, 10);
		}
			spawnPoint = transform.position + new Vector3 (Random.Range (-range*4, range*4), 0, Random.Range (-range*4, range*4));

		if (!(Physics.OverlapSphere (spawnPoint, 0.2f).Length > 0))
		{
			spawned = Instantiate (car, spawnPoint, Quaternion.LookRotation (transform.position - spawnPoint));
			spawned.GetComponent<UnityStandardAssets.Vehicles.Car.CarAIControl> ().SetTarget(transform);
			Destroy (spawned, 10);

		}

	}
}
