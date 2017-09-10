using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSystem : MonoBehaviour {
	public GameObject planePrefab;
	public UnityStandardAssets.Utility.WaypointCircuit path;
	private GameObject plane;
	// Use this for initialization
	void Start () {
		plane = Instantiate (planePrefab, transform);
		plane.GetComponent<UnityStandardAssets.Utility.WaypointProgressTracker>().circuit = path;
		int childcount = 0;
		int increment = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (plane.transform.position.y > 5) {
			GameObject.FindGameObjectWithTag ("Plane_Collider").SetActive (false);
			StartCoroutine (respawn ());
		}
	}

	IEnumerator respawn()
	{
		yield return new WaitForSeconds (20);
		DestroyImmediate (plane);
		Start ();
	}
}
