using UnityEngine;

public class Health_Body_Part : MonoBehaviour {

	public int layer;
	private Main_Health main;
	public bool head_shot;
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
	public void decrease (float damage)
	{
		if (head_shot)
			main.headShot ();
		else
			main.decrease (damage * DamageMultiplier);
	}
}
