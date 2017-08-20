using UnityEngine.UI;
using UnityEngine;
using UnityEngine.AI;


public class Main_Health : MonoBehaviour {

	public AudioClip[] Hurt;
	AudioSource aud;
	public Image label;
	public Slider healthSlider;
	public float Health;
	private float hp;
	public float regeneration;
	public bool is_player=true;
	public Transform root;
	// Use this for initialization
	void Start () 
	{
		aud = GetComponent<AudioSource> ();

		if (is_player) 
		{
			label = GameObject.FindGameObjectWithTag ("Health").transform.GetChild(0).GetComponent<Image> ();
			healthSlider= label.transform.parent.GetChild(1).GetComponent<Slider> ();
			hp = Health;
			set_health ();

			label.gameObject.SetActive (true);
			healthSlider.gameObject.SetActive (true);
		}



	}

	public void decrease (float damage)
	{
		aud.PlayOneShot( Hurt [Random.Range (0, Hurt.Length)]);
		hp -= damage;
		if (hp <= 0) 
			{
			die ();
			}
		else
			set_health ();
	}
	void set_health()
	{
		if (is_player)
		{
			label.CrossFadeAlpha (1-hp/Health, 0.2f, true);
			healthSlider.value = hp / Health;
		}

	}


	void die()
	{
		if (!is_player) 
		{
			GetComponent<NavMeshAgent> ().enabled=false;
			Destroy(GetComponent<enemyShooting> ());
			Destroy (GetComponent<Animator> ());
			add_to_children (root);
			Destroy (gameObject, 10);
		}
	}
	void Update()
	{
		if (Time.timeScale>0 && hp < Health)
		{
			hp += Time.deltaTime * regeneration;
			set_health ();
		}
	}
	void add_to_children(Transform parent)
	{
		parent.gameObject.AddComponent<Rigidbody> ();
		foreach (Transform child in parent) 
		{
			add_to_children (child);	
		}
	}
	public void add_score(float d)
	{
		if (is_player)
			PlayerPrefs.SetInt("Points",Mathf.CeilToInt(PlayerPrefs.GetInt("Points",0)+d));

	}
}
