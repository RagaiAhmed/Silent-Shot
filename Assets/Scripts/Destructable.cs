using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour {
	public GameObject destroyed;

	public	void destroy(Vector3 force)
	{
		if (destroyed != null) { // there is a destroyed object to replace
			Instantiate (destroyed, transform.position, transform.rotation); // but the destroyed
			Destroy (gameObject); // delete current
		} else {
			GetComponent<Rigidbody> ().AddForce (force); // add force on destroyable objects like a bullet hit it
		}
	}
}
