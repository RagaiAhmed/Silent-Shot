using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour {

	public Text label;
	public int hp=100;
	// Use this for initialization
	void Start () 
	{
		set_health ();
	}

	public void decrease (float damage)
	{
		hp -=(int) damage;
		if (hp<=0)
			die ();
		else
			set_health ();
	}
	void set_health()
	{
		label.text = "Hp: "+hp;
	}

	void die ()
	{
		
	}
}
