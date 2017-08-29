using System.Collections;
using UnityEngine.UI;
using UnityEngine;


public class Shooting : MonoBehaviour {

	// The following settings should be modified for every weapon individualy
	public float fire_rate; // fire rate in bullets/second
	private float cool_down_between_shots; // to be calculated from fire rate
	public float gun_reach; // the farthest the gun can reach
	public Object hit_effect; // the particle system effect upon bullet hitting a non-player object
	public Object blood_effect; // the particle system effect when a bullet hit a player
	public int burst=1;

	public bool is_scope;
	private GameObject scope;
	public float gun_damage; // damage for each gun per bullet
	public ParticleSystem muzzle_flash; // the particle system effect for gun fire flash
	float last_shot; // indicating the time of last shot
	private bool released=true; // if trigger released
	public float recoil; // recoil value

	public int layer;
	private Head_Movement player; // to apply recoil
	private Animator anim;
	Transform pl;

	public int clip_ammo_size; // size of ammo per reload
	public int total_ammo; // total ammount of ammo given
	public int default_ammo;

	public int current_ammo=0; // current ammo in the gun
	public bool reloading=false;

	public GameObject bullet_hole;
	public Transform gun_cam;

	public AudioClip GunFire;
	public AudioClip Reload;
	public AudioClip NoAmmo;

	public float side_shift;
	public bool in_hand=true;
	private float recoil_ammount;
	public Transform current_state;
	AudioSource Audio;
	public Vector3[] standard;
	public bool non_player;

	void Awake()
	{
		if (default_ammo == 0)
			default_ammo = total_ammo;
	}

	void Start()
	{
		if (!in_hand)
			return;
		StartCoroutine(reload()); // reloads at the start of the game
	}

	void OnEnable()
	{	

		cool_down_between_shots = 1 / fire_rate;// calculates the cool down

		if (!in_hand)
			return;
		pl = transform;
		for (int i = 0; i < layer; i++) 
		{
			pl = pl.parent;
		}
		Audio = GetComponent<AudioSource> ();
		reloading = false;
		if (non_player)
			return;
		transform.localPosition = standard [0];
		transform.localEulerAngles = standard [1];
		anim = pl.GetComponent<Animator> ();
		pl.GetComponent<WeaponSwitch> ().Name_label.text = GetComponent<Linkpic>().Name;
		player = pl.GetComponent<Head_Movement> ();
		current_state = transform.GetChild (0);
		set_ammo ();
		player.side_shift = side_shift;
		gun_cam = GameObject.FindGameObjectWithTag ("gun_cam").transform;
		scope = GameObject.FindGameObjectWithTag ("Scope").transform.GetChild(0).gameObject;
	}


	void LateUpdate () 
	{
		if (Time.timeScale > 0&&in_hand&&!non_player)
		{
			if (!reloading) // if not reloading
			{  
				shoot (Vector3.zero); // take shooting input
				if (Input.GetAxisRaw ("Reload") == 1) // if want to reload
					StartCoroutine (reload ());	 // reload
				aim (); // take aiming input
			}

			gun_cam.position = Vector3.Lerp (current_state.position, gun_cam.position, 0.2f);
			gun_cam.rotation = Quaternion.Slerp (gun_cam.rotation, current_state.rotation, 0.2f);

		}
 	}


	public void shoot(Vector3 n)
	{
		/* SHOOT A BULLET AT WILL , SOLIDER*/
		if ((Input.GetAxisRaw ("Fire1") == 1||non_player) && current_ammo>0) // if fire button hit
		{ 

			
			if ((Time.time - last_shot >= cool_down_between_shots || released)) 
			{ // if not in cooldown or gun was released before and there are ammo
				muzzle_flash.Play (); // shows the muzzle flash
				Audio.PlayOneShot (GunFire);
				last_shot = Time.time; // saves the time to know whether the next bullet will be in cooldown or not
				RaycastHit info;  // a variable representing information from a hit on a ray cast
				// if a ray from center of the screen hit a collider
				Ray r;
				if (!non_player)
					r = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0));
				else
					r = new Ray (transform.position,n - transform.position);
				r.origin += r.direction * 0.2f;
				if (burst>1)
				{
					for (int i = 0; i < burst; i++)
					{
						if (!non_player)
							r = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0));
						else
							r = new Ray (transform.position,n - transform.position);

						r.direction += new Vector3 (Random.Range (-0.1f, 0.1f), Random.Range (-0.1f, 0.1f));
						r.origin += Camera.main.transform.forward*0.5f;
						if (Physics.Raycast (r, out info, gun_reach))
						{
							if (info.collider.transform.IsChildOf (pl))
								return;
							hit (info, gun_damage, gun_reach - info.distance, r.direction);
						}

					}
				}
				else
				{
					if (Physics.Raycast (r, out info, gun_reach))
					{
						if (info.collider.transform.IsChildOf (pl))
							return;
						hit (info, gun_damage, gun_reach - info.distance, r.direction);
					}
				}

					
				// calculating recoil
				if (!non_player) 
				{
					float to_recoil = Random.Range (recoil / 2, recoil);
					player.apply_y (Random.Range (-to_recoil / 4, to_recoil / 4));
					recoil_ammount -= player.apply_x (-to_recoil);
				}


				current_ammo -= 1;
				if (current_ammo == 0) 
				{
					StartCoroutine (reload ());	 // reload
				}
				if (!non_player)
					set_ammo ();
			} 
			else
			{

				if (recoil_ammount>0&&!non_player)
				{
					player.apply_x(recoil/2);
					recoil_ammount -= recoil;
				}
			}
			released = false; // trigger not released

		} 
		else 
		{
			if (recoil_ammount>0&&!non_player)
			{
				player.apply_x(recoil/2);
				recoil_ammount -= recoil;
			}
		
			released = true;

			if (current_ammo == 0 &&( Input.GetAxisRaw ("Fire1") == 1 || non_player)) 
			{
				StartCoroutine (reload ());	 // reload

				if (!Audio.isPlaying)
				{
					Audio.clip = NoAmmo;
					Audio.Play();
				}
			}
		}
	}


	IEnumerator reload ()
	{
		
		if (total_ammo == 0 || current_ammo == clip_ammo_size) 
		{
			if (!Audio.isPlaying)
			{
				Audio.clip = NoAmmo;
				Audio.Play();
			}
		} 
		else
		{

			if (!non_player) 
			{	
				anim.SetBool ("Aim",false);
				current_state = transform.GetChild (0);
				if (is_scope) 
				{
					gameObject.layer = LayerMask.NameToLayer ("Weapon");
					scope.SetActive (false);
					Camera.main.fieldOfView = 50;
				}
				anim.SetTrigger ("Reload");

			}
			Transform temp = current_state;
			current_state = transform.GetChild (2);
			Audio.PlayOneShot(Reload);
			reloading = true;
			yield return new WaitForSeconds (Reload.length); //waits for the animation
			reloading = false;
			current_state = temp;
			int new_ammo = clip_ammo_size-current_ammo; 
			total_ammo -=  new_ammo ;
			current_ammo += new_ammo;
			if (total_ammo < 0) 
			{
				current_ammo += total_ammo;
				total_ammo = 0;
			}
			if (!non_player)
			{			set_ammo ();
				if (Input.GetButton ("Fire2"))
				{
					anim.SetBool ("Aim",true);
					current_state = transform.GetChild (1);
					if (is_scope) 
					{
						gameObject.layer = LayerMask.NameToLayer ("Invisible");
						scope.SetActive (true);
						Camera.main.fieldOfView = 25;
					}
				}
			}


		}

	}


	void aim()
	{
		if (Input.GetButtonDown ("Fire2"))
		{
			anim.SetBool ("Aim",true);
			current_state = transform.GetChild (1);
			if (is_scope) 
			{
				gameObject.layer = LayerMask.NameToLayer ("Invisible");
				scope.SetActive (true);
				Camera.main.fieldOfView = 25;
			}
		}
		else if(Input.GetButtonUp("Fire2"))
		{
			anim.SetBool ("Aim",false);
			current_state = transform.GetChild (0);
			if (is_scope) 
			{
				gameObject.layer = LayerMask.NameToLayer ("Weapon");
				scope.SetActive (false);
				Camera.main.fieldOfView = 50;
			}


		}
	}


	public void set_ammo()
	{
		player.gameObject.GetComponent<WeaponSwitch> ().reload_label.text = current_ammo + "/" + total_ammo;
	}

	void hit(RaycastHit info,float damage,float left_distance,Vector3 direction)
	{
		Rigidbody r = info.transform.gameObject.GetComponent<Rigidbody> ();
		if (r) 
		{
			r.AddForce (direction * damage * 10);
		}
		if (info.transform.CompareTag("Player")||info.transform.CompareTag("Player_Main")||info.transform.CompareTag("Enemy")) 
		{
			Health_Body_Part hbp =	info.collider.gameObject.GetComponent<Health_Body_Part> (); // decrease player health
			if (hbp)
				hbp.CmdDecrease (gun_damage,pl.gameObject);
			Destroy (Instantiate (blood_effect, info.point, Quaternion.LookRotation (info.normal)), 0.125f); // make blood effect and deletes it after some time
		} 
		else 
		{
			Destroy (Instantiate (hit_effect, info.point, Quaternion.LookRotation (info.normal)), 0.125f); // makes hit effect and destroys it after some time
			if (info.transform.CompareTag ("Shootable")) 
			{
				Destructable d = info.transform.gameObject.GetComponent<Destructable> ();
				if (d)
				{
					d.destroy ();
				}
				RaycastHit another_info;
				if (Physics.Raycast (info.point+direction*0.01f, direction,out another_info,left_distance)&&left_distance>0.01f) 
				{
					hit (another_info, damage / 2, left_distance - another_info.distance,direction);
				}
			}
			else if (!info.transform.CompareTag("Non-Shootable") && !info.transform.CompareTag("Grass")&& !info.transform.CompareTag("Pistol")&& !info.transform.CompareTag("Gun"))
			{
				Instantiate (bullet_hole, info.point + info.normal * 0.001f, Quaternion.LookRotation (info.normal)).transform.parent= info.transform;
			}
		}
	}
		


	void OnTriggerStay(Collider c)
	{
		if (c.CompareTag ("Player"))
		{
			WeaponSwitch ws = c.GetComponentInParent<WeaponSwitch> ();
			if (ws)
				ws.pick (gameObject);
		}
	}
	void OnTriggerExit(Collider c)
	{
		if (c.CompareTag ("Player"))
		{
			WeaponSwitch ws = c.GetComponentInParent<WeaponSwitch> ();
			if (ws)
				ws.end_pick ();
		}
	}
	public void drop()
	{
		transform.parent = null;
		gameObject.layer=LayerMask.NameToLayer("Default");
		foreach (Transform child in transform)
		{
			if (child)
				child.gameObject.layer = gameObject.layer;
		}

		in_hand = false;

		gameObject.AddComponent<BoxCollider> ();

		SphereCollider sp = gameObject.AddComponent<SphereCollider> ();
		sp.isTrigger = true;
		sp.radius = 0.5f/transform.localScale.x;

		gameObject.AddComponent<Rigidbody> ().useGravity = true;
		gameObject.SetActive (true);

	}
		
}
