using UnityEngine;

public class Destructable : MonoBehaviour {
	public GameObject destroyed;

	public	void destroy()
	{
		Instantiate (destroyed, transform.position, transform.rotation); // put the destroyed
		Destroy (gameObject); // delete current
	}
}
