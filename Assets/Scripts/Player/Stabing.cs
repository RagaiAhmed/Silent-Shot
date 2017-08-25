using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stabing : MonoBehaviour {

	public float damage;
	public float reach;
	public GameObject blood_effect;
	public int layer;
	private Animator player;
	public Transform gun_cam;
	public Transform current_state;
	public WeaponSwitch PL;
	public float stab_time;


	public float side_shift;


	public Vector3[] standard;

	void Start()
	{
		current_state = transform.GetChild (0);
	}
	void OnEnable()
	{
		transform.localPosition = standard [0];
		transform.localEulerAngles = standard [1];

		Transform t = transform;
		for (int i = 0; i < layer; i++)
		{
			t = t.parent;
		}
		player = t.GetComponent<Animator>();
		PL = t.GetComponent<WeaponSwitch> ();
		gun_cam = GameObject.FindGameObjectWithTag ("gun_cam").transform;
		PL.Name_label.text = GetComponent<Linkpic>().Name;
		PL.reload_label.text="";
		PL.GetComponent<Head_Movement> ().side_shift = side_shift;
	}

	void Update () 
	{
		if (Time.timeScale > 0)
		{
			gun_cam.position = Vector3.Lerp(gun_cam.position ,current_state.position,0.2f);
			gun_cam.rotation = Quaternion.Slerp(gun_cam.rotation ,current_state.rotation,0.2f);

			if (Input.GetButtonDown("Fire1"))
			{					
				StartCoroutine( stab());

			}

		}
		
	}


	IEnumerator stab ()
	{
		player.SetTrigger ("Stab");	
		current_state = transform.GetChild (1);
		RaycastHit hit;
		Ray r = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0));
		if (Physics.Raycast (r, out hit, reach)) 
		{
			if (hit.transform.CompareTag("Player")||hit.transform.CompareTag("Enemy"))
			{ // if hit a player

				hit.collider.gameObject.GetComponent<Health_Body_Part> ().CmdDecrease (damage,player.gameObject); // decrease player health
				Destroy (Instantiate (blood_effect, hit.point, Quaternion.LookRotation (hit.normal)), 0.125f); // make blood effect and deletes it after some time
			} 
			else 
			{
				if (hit.transform.CompareTag ("Shootable")) 
				{
					Destructable d = hit.transform.gameObject.GetComponent<Destructable> ();
					if (d != null)
					{
						d.destroy ();
					}
				}
			}
		}
		yield return new WaitForSeconds (stab_time);
		current_state = transform.GetChild (0);

	}
}
