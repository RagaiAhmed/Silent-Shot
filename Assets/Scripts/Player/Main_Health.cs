using UnityEngine.UI;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.SceneManagement;
//using UnityEngine.Networking;


public class Main_Health : MonoBehaviour {

	public AudioClip[] Hurt;
	AudioSource aud;
	public Image label;
	public Slider healthSlider;

	bool hurtin;
	public float Health;
	private float hp;
	public float regeneration;
	public bool is_player=true;
	public Transform root;
	public bool died=false;
	public float factor;
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

	public void RpcDecrease (float damage)
	{
		if (died)
			return;
		if (regeneration>=factor)
		{
			regeneration -= factor;
			StartCoroutine (restor_gen_after());
		}
		if (!hurtin) 
		{	
			AudioClip	ac = Hurt [Random.Range (0, Hurt.Length)];
			aud.PlayOneShot(ac);
			hurtin = true;
			StartCoroutine (non_hurtin (ac.length));
		}
		hp -= damage;
		if (hp <= 0) 
			{
			die ();
			}
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


	public void die()
	{
		died = true;
		Destroy (GetComponent<Animation> ());
		Destroy (GetComponent<NavMeshAgent> ());
		Destroy(GetComponent<enemyShooting> ());
		Destroy (GetComponent<Animator> ());
		Destroy (GetComponent<WeaponSwitch> ());
		Destroy (GetComponent<Head_Movement> ());
		Destroy (GetComponent<Character_Control> ());
		Destroy (GetComponent<PedestrianObject> ());



		add_to_children (root);
		Destroy (gameObject, 10);
		if (is_player)
		{
			GetComponent<WeaponSwitch> ().switchto (-1);
			GameObject.FindGameObjectWithTag ("Game_Over").transform.GetChild (0).gameObject.SetActive (true);
			StartCoroutine (return_main());

		}
	}
	void Update()
	{
		if (Time.timeScale>0 && hp < Health&&!died)
		{
			hp += Time.deltaTime * regeneration;
			set_health ();
		}
	}
	bool add_to_children(Transform parent)
	{
		Shooting sht = parent.GetComponent<Shooting> ();
		if (sht)
		{
			sht.drop ();
			return false;
		}
		Rigidbody rb = parent.gameObject.GetComponent<Rigidbody> ();
		if (!rb)
			rb=parent.gameObject.AddComponent<Rigidbody> ();
		foreach (Transform child in parent) 
		{
			if(add_to_children (child))
				child.gameObject.AddComponent<CharacterJoint> ().connectedBody = rb;

		}
		return true;
	}
	public void add_score(float d)
	{
		if (is_player)
		{
			PlayerPrefs.SetInt("Points",Mathf.CeilToInt(PlayerPrefs.GetInt("Points",0)+d));
		}
			
	}

	IEnumerator restor_gen_after()
	{
		yield return new WaitForSeconds (5f);
		regeneration += factor;
	}
	IEnumerator return_main()
	{
		yield return new WaitForSeconds (8);
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		SceneManager.LoadScene ("Main");
	}
	IEnumerator non_hurtin(float time)
	{
		yield return new WaitForSeconds (time);
		hurtin = false;
	}

	void OnCollisionEnter(Collision c)
	{
		float allVelocity = c.relativeVelocity.magnitude ;
	
		if (allVelocity>20)
			RpcDecrease (allVelocity );
	}
}
