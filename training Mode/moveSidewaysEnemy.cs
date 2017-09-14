using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class moveSidewaysEnemy : MonoBehaviour {
	public float speed = 0.25f;

	void Update() {
		transform.Translate (speed, 0f, 0f);
	}
	public void OnCollisionEnter(Collision col) {
		
		if (col.transform.name == "Cube (4)" || col.transform.name == "Cube (3)") {
			speed = speed * -1f;
		}
	}
}