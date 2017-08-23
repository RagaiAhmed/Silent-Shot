using UnityEngine;
using UnityEngine.Networking;


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

	public void CmdDecrease (float damage,GameObject did)
	{
		if (main.died)
			return;
		float d = damage * DamageMultiplier;
		main.RpcDecrease (d);
		if (!did)
			return;
		Main_Health doer = did.GetComponent<Main_Health> ();
		if (doer)
			doer.add_score(d);


	}
	void OnCollisionEnter(Collision c)
	{
		float allVelocity = c.relativeVelocity.magnitude ;
		if (allVelocity>1)
			CmdDecrease (allVelocity * 3,null);
	}

}
	
