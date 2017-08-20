using UnityEngine;

public class Health_Body_Part : MonoBehaviour {

	public int layer;
	private Main_Health main;
	public float DamageMultiplier;

	void Start()
	{
		Transform t = transform;
		for (int i = 0; i < layer; i++) 
		{
			t = t.parent;
		}
		main = t.GetComponent<Main_Health> ();
	}
	public void decrease (float damage,GameObject did)
	{
		float d = damage * DamageMultiplier;
		main.decrease (d);
		if (!did)
			return;
		Main_Health doer = did.GetComponent<Main_Health> ();
		if (doer)
			doer.add_score(d);


	}
	void OnCollsionEnter(Collision c)
	{
		if (c.transform.tag == "Vehicle") {
			float allVelocity = Mathf.Sqrt (Mathf.Pow (c.relativeVelocity.x, 2) + Mathf.Pow (c.relativeVelocity.y, 2) + Mathf.Pow (c.relativeVelocity.z, 2));
			if (allVelocity > 1)
				decrease (allVelocity * 3,null);
		}
	}

}
