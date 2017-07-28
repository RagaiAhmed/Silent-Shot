using System.Collections;
using System.Collections.Generic;
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
	private bool released; // if trigger released
	public float recoil; // recoil value
	private Head_Movement head; // to apply recoil
	public int clip_ammo_size;
	public int total_ammo;
	private int current_ammo;
	private bool reloading=false;

	void Start()
	{
		cool_down_between_shots = 1 / fire_rate; // calculates the cool down
		head = transform.parent.gameObject.GetComponent<Head_Movement> (); // gets head moving script
		StartCoroutine(reload());	
	}
	void Update () 
	{
		if (!reloading) 
		{
			shoot ();
			if (Input.GetAxisRaw ("Reload") == 1) 
				StartCoroutine(reload());	
		}
		// TODO zoom
	}


	void shoot()
	{
		/* SHOOT A BULLET AT WILL , SOLIDER*/
		if (Input.GetAxisRaw ("Fire1") == 1) // if fire button hit
		{
			if (current_ammo == 0) 
			{
				// TODO sound effect of no ammo **Stage 2
				return;
			}
			
			if (Time.time - last_shot >= cool_down_between_shots || released) // if not in cooldown or gun was released before
			{
				muzzle_flash.Play (); // shows the muzzle flash
				// TODO shooting sound effect **Stage 2
				last_shot = Time.time; // saves the time to know whether the next bullet will be in cooldown or not
				RaycastHit info;  // a variable representing information from a hit on a ray cast
				if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out info, gun_reach)) 
				{
					if (info.collider.gameObject.tag == "Player") // if hit a player
					{
						info.collider.gameObject.GetComponent<Health>().decrease(gun_damage); // decrease player health
						Destroy(Instantiate (blood_effect, info.point, Quaternion.LookRotation (info.normal)),0.125f); // make blood effect and deletes it after some time
					}
					else
						Destroy(Instantiate (hit_effect, info.point, Quaternion.LookRotation (info.normal)),0.125f); // makes hit effect and destroys it after some time
					//TODO with effect script of bullet hit sound **Stage 2
				}
				// calculating recoil
				Vector3 recoil_rot = new Vector3 (Random.Range (-recoil / 50, recoil / 100), Random.Range (-recoil / 200, recoil / 200), 0)
					+ head.new_rotation;
				head.new_rotation =Vector3.Lerp(head.new_rotation ,recoil_rot,0.2f); // applying recoil while keeping it smooth
			}

			current_ammo -= 1;
			released=false;

		} 
		else {released = true;}
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
			current_ammo = Mathf.Min(total_ammo,clip_ammo_size); 
			total_ammo -= current_ammo;
		}
	}
}
