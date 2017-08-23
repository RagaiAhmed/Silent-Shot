using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class spawningEnemies : MonoBehaviour
{
	public float spawnTime = 5.5f;
	public GameObject enemy;
	Transform [] Nodes;
	void Start () 
	{
		Nodes = new Transform[transform.childCount];
		for (int i = 0; i < transform.childCount; i++)
			Nodes [i] = transform.GetChild (i);
		InvokeRepeating ("spawn", spawnTime, spawnTime);
	}
	void spawn ()
	{


		Transform spawnPoint = Nodes [Random.Range (0, transform.childCount)];
		GameObject spawned = Instantiate (enemy, spawnPoint.position, spawnPoint.rotation);
		spawned.GetComponent<enemyShooting> ().Nodes = Nodes;
	}
}