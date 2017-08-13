using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WeaponSwitch : MonoBehaviour {
	public Text reload_label;
	public Text Name_label;


	public Transform WeaponHolder;
	private Animator player;
	public float switch_time;
	public AudioClip switching;
	private AudioSource aud;

	public float scroll_cool_down;

	float last;

	bool free = true;
	Shooting state;
	int current = 0;

	KeyCode[] numberkeys = 
	{
		KeyCode.Alpha1,
		KeyCode.Alpha2,
		KeyCode.Alpha3,
		KeyCode.Alpha4,
		KeyCode.Alpha5,
		KeyCode.Alpha6,
		KeyCode.Alpha7,
		KeyCode.Alpha8,
		KeyCode.Alpha9
	};

	void Start()
	{
		aud = GetComponent<AudioSource> ();
		player = GetComponent<Animator> ();

	}

	void Update ()
	{
		if (Time.timeScale > 0)
		{
			for (int i = 0; i < 9; i++) 
			{
				if (Input.GetKeyDown (numberkeys [i]))
					switchto (i);
			}
			float scroll_state = Input.GetAxis ("Mouse ScrollWheel");
			if (scroll_state > 0 && cool_down ()) 
			{	last = Time.time;
				switchto ((current + 1) % WeaponHolder.childCount);
			}
			else if (scroll_state < 0 && cool_down()) 
			{
				last = Time.time;
				switchto((current-1) % WeaponHolder.childCount);
			}

			if (Input.GetButtonDown ("SwitchWeapon")) 
			{
				switchto (current);
			}
			
		}
		
	}

	void switchto(int index)
	{
		if (index < 0)
			index += WeaponHolder.childCount;
		if (index < WeaponHolder.childCount && index >= 0) 
		{
			player.SetLayerWeight (1, 1);

			GameObject gun;

			player.SetTrigger ("Switch");
			aud.PlayOneShot (switching);

			gun = WeaponHolder.GetChild (current).gameObject;
			state = gun.GetComponent<Shooting> ();
			if (state!=null)
				state.current_state = gun.transform.GetChild (2);
			StartCoroutine	(change_state (gun, false));
			player.SetBool (gun.tag, false);

			if (index != current || free)
			{
				current = index;
				gun = WeaponHolder.GetChild (index).gameObject;
				state = gun.GetComponent<Shooting> ();
				if (state != null)
					state.current_state = gun.transform.GetChild (0);
				StartCoroutine	(change_state (gun, true));
				player.SetBool (gun.tag, true);
				free = false;
			}
			else
			{
				free = true;
				reload_label.text = "";
				Name_label.text = "";

			}
		}
	}

	IEnumerator change_state (GameObject obj,bool state)
	{
		yield return new WaitForSeconds(switch_time/2);
		obj.SetActive(state);

	}


	bool cool_down()
	{
		return Time.time - last >= scroll_cool_down;
	}
}
