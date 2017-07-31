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
	private Head_Movement head; // to apply recoil
	public int clip_ammo_size; // size of ammo per reload
	public int total_ammo; // total ammount of ammo given
	private int current_ammo=0; // current ammo in the gun
	private bool reloading=false;
	private float anti_recoil=0f; // to calculate the back of recoil
	public GameObject aim_cursor; //  aim cursor when aiming
	public Text reload_label;

	void Start()
	{
		cool_down_between_shots = 1 / fire_rate;// calculates the cool down
		head = transform.parent.parent.gameObject.GetComponent<Head_Movement> (); // gets head moving script
		StartCoroutine(reload()); // reloads at the start of the game
	}

	void OnEnable()
	{
		set_ammo ();
	}

	void FixedUpdate () 
	{
		if (!reloading)  // if not reloading
		{
			shoot (); // take shooting input
			if (Input.GetAxisRaw ("Reload") == 1) // if want to reload
				StartCoroutine(reload());	 // reload
			aim(); // take aiming input
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
				// TODO shooting sound effect **Stage 2
				last_shot = Time.time; // saves the time to know whether the next bullet will be in cooldown or not
				RaycastHit info;  // a variable representing information from a hit on a ray cast
				// if a ray from center of the screen hit a collider
				if (Physics.Raycast (Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0)), out info, gun_reach)) 
				{ 
					hit(info,gun_damage,gun_reach-info.distance,(info.point - transform.parent.position).normalized);
				}
				// calculating recoil
				anti_recoil += Random.Range (0, recoil / 50); // random recoil value in recoil range
				apply_rotation (-anti_recoil,0.2f); // apply recoil
				anti_recoil *= 0.2f; // the recoil saved as the only applied percent of recoil

				current_ammo -= 1;
				set_ammo ();
				released = false; // trigger not released
			}
			else 
			{
				if (current_ammo == 0) 
				{
					/*TODO sound effect of no ammo*/
				}
				// restores gun in place
				if (anti_recoil > 0) 
				{
					apply_rotation (anti_recoil,0.2f);
					anti_recoil *= 0.8f;
				}
			}
		} 
		else 
		{
			released = true;
			if (anti_recoil > 0)// restores gun in place
			{
				apply_rotation (anti_recoil,0.2f);
				anti_recoil *= 0.8f;
			}
		}
	}

	void apply_rotation(float amount,float with) // apply a rotation around x axis with a specific percentage
	{
		Vector3 rot = new Vector3 (amount, 0, 0) + head.new_rotation;
		head.new_rotation = Vector3.Lerp (head.new_rotation, rot, with);
	}

	IEnumerator reload ()
	{
		if (total_ammo == 0 || current_ammo == clip_ammo_size) 
		{
			//TODO sound_effect of no ammo or full clip **Stage 2
		} 
		else
		{
			// TODO initiate animation **Stage 2
			// TODO Sound effect **Stage 2
			float time_to_wait = 1;
			reloading = true;
			yield return new WaitForSeconds (time_to_wait); //waits for the animation
			reloading = false;
			int new_ammo = Mathf.Min(total_ammo,clip_ammo_size)-current_ammo; 
			total_ammo -=  new_ammo ;
			current_ammo += new_ammo;
			set_ammo ();
		}

	}

	void aim()
	{
		if (Input.GetButtonDown ("Fire2")) 
		{
			aim_cursor.SetActive (true);
			// TODO animation of aiming **Stage 2
		}
		else if (Input.GetButtonUp ("Fire2")) 
		{
			aim_cursor.SetActive (false);
			// TODO animation of leaving aiming **Stage 2
		}
	}

	void set_ammo()
	{
		reload_label.text = current_ammo + "/" + total_ammo;
	}

	void hit(RaycastHit info,float damage,float left_distance,Vector3 direction)
	{
		if (info.transform.CompareTag("Player")) { // if hit a player
			
			info.collider.gameObject.GetComponent<Health> ().decrease (gun_damage); // decrease player health
			Destroy (Instantiate (blood_effect, info.point, Quaternion.LookRotation (info.normal)), 0.125f); // make blood effect and deletes it after some time
		} 
		else 
		{
			Destroy (Instantiate (hit_effect, info.point, Quaternion.LookRotation (info.normal)), 0.125f); // makes hit effect and destroys it after some time
			//TODO with effect script of bullet hit sound **Stage 2
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

			}
			RaycastHit another_info;
			print (info.transform.gameObject.name);
			if (Physics.Raycast (info.point+direction*0.01f, direction,out another_info,left_distance)&&left_distance>0.01f) 
			{
				hit (another_info, damage / 2, left_distance - another_info.distance,direction);
			}
		}
	}

}
