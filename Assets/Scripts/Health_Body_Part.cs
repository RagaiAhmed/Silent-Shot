using UnityEngine;

public class Health_Body_Part : MonoBehaviour {


	public Main_Health main;
	public bool head_shot;
	public float DamageMultiplier;
	public void decrease (float damage)
	{
		if (head_shot)
			main.headShot ();
		else
			main.decrease (damage * DamageMultiplier);
	}
}
