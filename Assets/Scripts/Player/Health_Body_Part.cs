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
		Main_Health doer = did.GetComponent<Main_Health> ();
		if (doer)
			doer.add_score(d);


	}

}
