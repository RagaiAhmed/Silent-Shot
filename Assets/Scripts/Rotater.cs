using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour {

	void FixedUpdate () {
		transform.Rotate (new Vector3 (0, 0, 50));
	}
}
