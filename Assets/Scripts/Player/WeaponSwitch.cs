using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WeaponSwitch : MonoBehaviour {
	public GameObject Inventory;

	public Text reload_label;
	public Text Name_label;
	public Text message;


	public int[] WeaponHolder_path;

	private Transform WeaponHolder;
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
		Inventory = GameObject.FindGameObjectWithTag ("Inventory").transform.GetChild (0).gameObject;

		reload_label=GameObject.FindGameObjectWithTag("Ammo").GetComponent<Text>();
		Name_label=GameObject.FindGameObjectWithTag("WPName").GetComponent<Text>();
		message=GameObject.FindGameObjectWithTag("Message").GetComponent<Text>();

		WeaponHolder = transform;
		foreach (int i in WeaponHolder_path)
		{
			WeaponHolder = WeaponHolder.GetChild (i);
		}
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
				show_hide ();
			}
			if (Input.GetButtonDown ("Inventory")) 
			{
				show_inventory ();
			}
			if (Input.GetButtonDown("Drop")) 
			{
				drop (current);
			}

		}
		
	}

	public void switchto(int index)
	{
		if (index < 0)
			index += WeaponHolder.childCount;
		if (index < WeaponHolder.childCount && index >= 0) 
		{
			player.SetLayerWeight (1, 1);

			GameObject gun;

			player.SetTrigger ("Switch");
			aud.PlayOneShot (switching);
			if (current < WeaponHolder.childCount && current >= 0) 
			{
				gun = WeaponHolder.GetChild (current).gameObject;
				state = gun.GetComponent<Shooting> ();
				if (state != null)
					state.current_state = gun.transform.GetChild (2);
				StartCoroutine	(change_state (gun, false));
				player.SetBool (gun.tag, false);
			}
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
				make_free ();

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

	void show_hide()
	{
		switchto (current);
	}

	public void drop(int i)
	{
		
		
		GameObject gun = WeaponHolder.GetChild (i).gameObject;
		if (!gun.GetComponent<Shooting> ())
			return;
		player.SetBool (gun.tag, false);
		player.SetTrigger ("Switch");
		gun.GetComponent<Shooting> ().drop ();
		current -= 1;
		if (current < 0)
			current = 0;

		make_free ();


	}
		


	public void pick(GameObject go)
	{
		message.text = "Press V to grab : " + go.name;
		if (Input.GetButtonDown ("Take")) 
		{
			go.SetActive (false);
			message.text = "";
			go.layer = LayerMask.NameToLayer("Weapon");
			foreach (Transform child in go.transform)
			{
				if (child)
					child.gameObject.layer = go.layer;
			}
			go.transform.parent = WeaponHolder;
			Vector3[] standard = go.GetComponent<Shooting> ().standard;
			go.transform.localPosition = standard [0];
			go.transform.localEulerAngles = standard [1];
			go.GetComponent<Shooting> ().in_hand = true;
			Destroy (go.GetComponent<SphereCollider> ());
			Destroy (go.GetComponent<BoxCollider> ());
			Destroy (go.GetComponent<Rigidbody> ());
			switchto (-1);

		}

	}
	public void end_pick ()
	{
		message.text = "";
	}

	public void make_free()
	{
		free = true;
		reload_label.text = "";
		Name_label.text = "";
		player.GetComponent<Head_Movement> ().side_shift = 0;
		player.SetLayerWeight (1, 0.5f);
	}

	void show_inventory ()
	{
		Inventory.GetComponent<Inventory> ().show (WeaponHolder);
	}

	public void switch_in(int frm, int to)
	{
		if (current == frm)
			current = to;
		else if (current == to) 
		{
			current += (frm - to) / (Mathf.Abs (frm - to));
		}
		WeaponHolder.GetChild (frm).SetSiblingIndex (to);
		show_inventory ();
		
	}
}
