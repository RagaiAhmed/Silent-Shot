using UnityEngine.UI;
using UnityEngine;

public class Main_Health : MonoBehaviour {

	public AudioClip[] Hurt;
	AudioSource aud;
	public Image label;
	public Slider healthSlider;
	public float Health;
	private float hp;
	public float regeneration;
	// Use this for initialization
	void Start () 
	{
		label = GameObject.FindGameObjectWithTag ("Health").transform.GetChild(0).GetComponent<Image> ();
		healthSlider= label.transform.parent.GetChild(1).GetComponent<Slider> ();
		aud = GetComponent<AudioSource> ();
		hp = Health;
		set_health ();

		label.gameObject.SetActive (true);
		healthSlider.gameObject.SetActive (true);


	}

	public void decrease (float damage)
	{
		aud.PlayOneShot( Hurt [Random.Range (0, Hurt.Length)]);
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
		healthSlider.value = hp / Health;
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
}
