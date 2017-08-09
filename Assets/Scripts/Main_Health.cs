using UnityEngine.UI;
using UnityEngine;

public class Main_Health : MonoBehaviour {

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
		if (hp <= 0) 
			{
			// TODO call animator for death
			}
		else
			set_health ();
	}
	void set_health()
	{
		label.text = "Hp: "+hp;
	}
	public void headShot ()
	{
		// TODO call animator for headshot animation
	}
}
