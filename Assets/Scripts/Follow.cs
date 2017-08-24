using UnityEngine;

public class Follow : MonoBehaviour {

	public Transform to_follow;

	Vector3 diff;
	void Start () 
	{
		diff = to_follow.position - transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position = to_follow.position - diff;
		transform.rotation = Quaternion.Euler(to_follow.eulerAngles.x,to_follow.eulerAngles.y,0);

	}
}
