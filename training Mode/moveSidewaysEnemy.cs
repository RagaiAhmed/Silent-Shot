using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class moveSidewaysEnemy : MonoBehaviour {
	public float speed = 0.25f;
	public GameObject ObjToLeft;
	public GameObject ObjToRight;
	void Update() {
		transform.Translate (speed, 0f, 0f);
		float rightDist = Vector3.Distance (ObjToRight.transform.position, gameObject.transform.position);
		float leftDist = Vector3.Distance (ObjToLeft.transform.position, gameObject.transform.position);
		if (leftDist <= 2f || rightDist <= 2f) {
			speed = speed * -1f;
		}
	}
	public void OnCollisionEnter(Collision col) {
		
		if (col.transform.name == ObjToRight.name || col.transform.name == ObjToLeft.name) {
			speed = speed * -1f;
		}
	}
}