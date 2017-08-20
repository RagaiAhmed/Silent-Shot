using UnityEngine.UI;
using UnityEngine;

public class Main_Health : MonoBehaviour {

	public AudioClip[] Hurt;
	AudioSource aud;
	public Image label;
	public float Health;
	private float hp;
	public float regeneration;
	// Use this for initialization
	void Start () 
	{
		label = GameObject.FindGameObjectWithTag ("Health").transform.GetChild(0).GetComponent<Image> ();
		aud = GetComponent<AudioSource> ();
		hp = Health;
		set_health ();
		label.gameObject.SetActive (true);

	}

	public void decrease (float damage)
	{
		aud.Stop ();
		aud.clip = Hurt [Random.Range (0, Hurt.Length)];
		aud.Play ();
		hp -= damage;
		if (hp <= 0) 
			{
			// TODO call animator for death
			}
		else
			set_health ();
	}
	void set_health()
	{
		label.CrossFadeAlpha (1-hp/Health, 0.2f, true);
	}
	public void headShot ()
	{
		// TODO call animator for headshot animation
	}
	void Update()
	{
		if (Time.timeScale>0 && hp < Health)
		{
			hp += Time.deltaTime * regeneration;
			set_health ();
		}
	}

	void OnCollsionEnter(Collision c)
	{
		if (c.transform.tag == "Vehicle") {
			float allVelocity = Mathf.Sqrt (Mathf.Pow (c.relativeVelocity.x, 2) + Mathf.Pow (c.relativeVelocity.y, 2) + Mathf.Pow (c.relativeVelocity.z, 2));
			if (allVelocity > 1) {
				decrease (allVelocity * 3);
				print ("ouch");}
		}
	}
}
