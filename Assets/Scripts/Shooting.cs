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

	public float gun_damage; // damage for each gun per bullet
	public ParticleSystem muzzle_flash; // the particle system effect for gun fire flash
	float last_shot; // indicating the time of last shot
	private bool released=true; // if trigger released
	public float recoil; // recoil value

	public Head_Movement player; // to apply recoil

	public int clip_ammo_size; // size of ammo per reload
	public int total_ammo; // total ammount of ammo given

	private int current_ammo=0; // current ammo in the gun
	private bool reloading=false;
	private Vector3 anti_recoil; // to calculate the back of recoil

	public Text reload_label;
	public GameObject bullet_hole;
	public Transform gun_cam;

	public AudioClip GunFire;
	public AudioClip Reload;
	public AudioClip NoAmmo;

	private Transform current_state;

	AudioSource Audio;
	void Start()
	{
		Audio = GetComponent<AudioSource> ();
		cool_down_between_shots = 1 / fire_rate;// calculates the cool down
		StartCoroutine(reload()); // reloads at the start of the game
		current_state=gun_cam.transform.GetChild(1);
	}

	void OnEnable()
	{
		set_ammo ();
	}


	void Update()
	{
		if (Time.timeScale > 0) 
		{
			transform.position = Vector3.Lerp (current_state.position,transform.position,0.1f);
			transform.rotation = Quaternion.Lerp (transform.rotation, current_state.rotation, 0.2f);
		}			
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
		if (Input.GetAxisRaw ("Fire1") == 1) // if fire button hit
		{ 
			if ((Time.time - last_shot >= cool_down_between_shots || released) && current_ammo > 0) // if not in cooldown or gun was released before and there are ammo
			{ 
				muzzle_flash.Play (); // shows the muzzle flash
				Audio.PlayOneShot(GunFire);
				last_shot = Time.time; // saves the time to know whether the next bullet will be in cooldown or not
				RaycastHit info;  // a variable representing information from a hit on a ray cast
				// if a ray from center of the screen hit a collider
				Ray r =Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0));
				if (Physics.Raycast (r, out info, gun_reach)) 
				{
					hit(info,gun_damage,gun_reach-info.distance,r.direction);
				}
				// calculating recoil
				anti_recoil +=new Vector3(Random.Range (0, recoil / 50),Random.Range(-recoil/100, recoil / 100),0); // random recoil value in recoil range
				anti_recoil = -apply_rotation (-anti_recoil,0.2f); // the recoil saved as the only applied percent of recoil

				current_ammo -= 1;
				set_ammo ();
				released = false; // trigger not released
			}
			else 
			{
				if (current_ammo == 0) 
				{
					if (!Audio.isPlaying)
					{
						Audio.clip = NoAmmo;
						Audio.Play();
					}

				
					// restores gun in place

					restore_gun ();

				}
				if (fire_rate == 0) 
				{
					restore_gun ();
				}
			}
		} 
		else 
		{
			released = true;
			restore_gun ();
		}
	}

	void restore_gun()
	{
		if (anti_recoil.magnitude > 0)// restores gun in place
		{
			anti_recoil -= apply_rotation (anti_recoil,0.1f);
		}
	}

	Vector3 apply_rotation(Vector3 amount,float with) // apply a rotation around with a specific percentage
	{
		Vector3 rot = amount*with + player.new_rotation;
		player.new_rotation = rot;
		return amount*with;
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
			current_state=gun_cam.transform.GetChild(3);
			Audio.PlayOneShot(Reload);
			float time_to_wait = 1;
			reloading = true;
			yield return new WaitForSeconds (time_to_wait); //waits for the animation
			reloading = false;
			int new_ammo = Mathf.Min(total_ammo+current_ammo,clip_ammo_size-current_ammo); 
			total_ammo -=  new_ammo ;
			current_ammo += new_ammo;
			set_ammo ();
			current_state=gun_cam.transform.GetChild(1);

		}

	}

	void aim()
	{
		if (Input.GetButtonDown("Fire2"))
			current_state=gun_cam.transform.GetChild(0);
		else if(Input.GetButtonUp("Fire2"))
			current_state=gun_cam.transform.GetChild(1);
	}

	void set_ammo()
	{
		reload_label.text = current_ammo + "/" + total_ammo;
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

}
