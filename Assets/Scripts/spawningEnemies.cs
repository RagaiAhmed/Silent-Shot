using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class spawningEnemies : MonoBehaviour {
	public Transform[] spawnPoints;
	public float spawnTime = 5.5f;
	public GameObject enemy;
	// Use this for initialization
	void Start () {
		InvokeRepeating ("spawn", spawnTime, spawnTime);
		StartCoroutine ("delayforTime", spawnTime);
	}
	void spawn () {
		int spawnPointIndex = Random.Range(0, spawnPoints.Length);
		Instantiate (enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
	}
	IEnumerator delayforTime(float delay){
		yield return new WaitForSeconds (delay);
	}
}