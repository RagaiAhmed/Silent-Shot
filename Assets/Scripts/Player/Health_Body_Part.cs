﻿using UnityEngine;


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
		float d = damage * DamageMultiplier;
		main.CmdDecrease (d);
		if (!did)
			return;
		Main_Health doer = did.GetComponent<Main_Health> ();
		if (doer)
			doer.add_score(d);

	}

	void OnCollisionEnter(Collision c)
	{
		float allVelocity = c.relativeVelocity.magnitude ;

		if ((c.collider.CompareTag("Vehicle")||c.collider.CompareTag("Player_Vehicle"))&&allVelocity>7.5f)
			CmdDecrease (allVelocity*5,null);
	}

}
	
