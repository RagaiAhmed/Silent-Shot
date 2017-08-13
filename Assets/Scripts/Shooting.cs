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

	public float reload_time;

	public float gun_damage; // damage for each gun per bullet
	public ParticleSystem muzzle_flash; // the particle system effect for gun fire flash
	float last_shot; // indicating the time of last shot
	private bool released=true; // if trigger released
	public float recoil; // recoil value

	public Head_Movement player; // to apply recoil
	public Animator anim;

	public int clip_ammo_size; // size of ammo per reload
	public int total_ammo; // total ammount of ammo given

	private int current_ammo=0; // current ammo in the gun
	private bool reloading=false;

	public GameObject bullet_hole;
	public Transform gun_cam;

	public AudioClip GunFire;
	public AudioClip Reload;
	public AudioClip NoAmmo;

	public float side_shift;

	private float recoil_ammount;
	public Transform current_state;
	AudioSource Audio;
	void Start()
	{
		current_state = transform.GetChild (0);
		Audio = GetComponent<AudioSource> ();
		cool_down_between_shots = 1 / fire_rate;// calculates the cool down
		StartCoroutine(reload()); // reloads at the start of the game
	}

	void OnEnable()
	{
		player.gameObject.GetComponent<WeaponSwitch> ().Name_label.text = transform.name;
		set_ammo ();
		player.side_shift = side_shift;
		reloading = false;
	}


	void LateUpdate () 
	{
		if (Time.timeScale > 0)
		{
			if (!reloading) // if not reloading
			{  
				shoot (); // take shooting input
				if (Input.GetAxisRaw ("Reload") == 1) // if want to reload
					StartCoroutine (reload ());	 // reload
				aim (); // take aiming input
			}
		}
 	}


	void shoot()
	{
		/* SHOOT A BULLET AT WILL , SOLIDER*/
		if (Input.GetAxisRaw ("Fire1") == 1 && current_ammo>0) // if fire button hit
		{ 
			
			if ((Time.time - last_shot >= cool_down_between_shots || released)) 
			{ // if not in cooldown or gun was released before and there are ammo
				muzzle_flash.Play (); // shows the muzzle flash
				Audio.PlayOneShot (GunFire);
				last_shot = Time.time; // saves the time to know whether the next bullet will be in cooldown or not
				RaycastHit info;  // a variable representing information from a hit on a ray cast
				// if a ray from center of the screen hit a collider
				Ray r = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0));
				if (Physics.Raycast (r, out info, gun_reach)) {
					hit (info, gun_damage, gun_reach - info.distance, r.direction);
				}
				// calculating recoil

				float to_recoil = Random.Range (recoil / 2, recoil);
				player.apply_y (Random.Range (-to_recoil / 4, to_recoil / 4));
				recoil_ammount -= player.apply_x (-to_recoil);

				current_ammo -= 1;
				set_ammo ();
				released = false; // trigger not released
			} 
			else
			{
				if (recoil_ammount>0)
				{
					player.apply_x(recoil/2);
					recoil_ammount -= recoil;
				}
			}
		} 
		else 
		{
			if (recoil_ammount>0)
			{
				player.apply_x(recoil/2);
				recoil_ammount -= recoil;
			}
		
			released = true;

			if (current_ammo == 0 && Input.GetAxisRaw ("Fire1") == 1) 
			{
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
			anim.SetTrigger ("Reload");
			Transform temp = current_state;
			current_state = transform.GetChild (2);
			Audio.PlayOneShot(Reload);
			reloading = true;
			yield return new WaitForSeconds (reload_time); //waits for the animation
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
			set_ammo ();

		}

	}


	void aim()
	{
		if (Input.GetButtonDown ("Fire2"))
		{
			anim.SetBool ("Aim",true);
			current_state = transform.GetChild (1);
		}
		else if(Input.GetButtonUp("Fire2"))
		{
			anim.SetBool ("Aim",false);
			current_state = transform.GetChild (0);


		}
	}


	void set_ammo()
	{
		player.gameObject.GetComponent<WeaponSwitch> ().reload_label.text = current_ammo + "/" + total_ammo;
	}

	void hit(RaycastHit info,float damage,float left_distance,Vector3 direction)
	{
		if (info.transform.CompareTag("Player")) { // if hit a player
			
			info.collider.gameObject.GetComponent<Health_Body_Part> ().decrease (gun_damage); // decrease player health
			Destroy (Instantiate (blood_effect, info.point, Quaternion.LookRotation (info.normal)), 0.125f); // make blood effect and deletes it after some time
		} 
		else 
		{
			Destroy (Instantiate (hit_effect, info.point, Quaternion.LookRotation (info.normal)), 0.125f); // makes hit effect and destroys it after some time
			if (info.transform.CompareTag ("Shootable")) 
			{
				Destructable d = info.transform.gameObject.GetComponent<Destructable> ();
				if (d != null)
				{
					d.destroy ();
				}
				else 
				{
					Rigidbody r = info.transform.gameObject.GetComponent<Rigidbody> ();
					r.AddForce(direction * damage);
				}
				RaycastHit another_info;
				if (Physics.Raycast (info.point+direction*0.01f, direction,out another_info,left_distance)&&left_distance>0.01f) 
				{
					hit (another_info, damage / 2, left_distance - another_info.distance,direction);
				}
			}
			else if (info.transform.CompareTag ("Wall"))
			{
				Instantiate (bullet_hole,info.point+info.normal*0.001f,Quaternion.LookRotation(info.normal),info.transform);
			}
		}
	}

	void Update ()
	{
		gun_cam.position = Vector3.Lerp (current_state.position,gun_cam.position, 0.2f);
		gun_cam.rotation = Quaternion.Slerp(gun_cam.rotation ,current_state.rotation,0.2f);
	}

}
