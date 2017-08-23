using UnityEngine;

public class Day_Night_Cycle : MonoBehaviour {
	public float Day_Duration;
	void Start () 
	{
		// Changes time of whole day rotation to radians per second
		GetComponent<Rigidbody> ().angularVelocity = new Vector3 (0, 0, (2 * Mathf.PI) / (Day_Duration * 60));
	}
	

}
