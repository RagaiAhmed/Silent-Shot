using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

	public Transform Bag;
	public Transform weapon_holder;

	public void show (Transform WeaponHolder)
	{
		weapon_holder = WeaponHolder;
		Time.timeScale = 0;
		gameObject.SetActive (true);
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		for (int i = 0; i < WeaponHolder.transform.childCount; i++) 
		{
			Transform slot = Bag.transform.GetChild (i);
			Transform Weapon = WeaponHolder.GetChild (i);
			Text name = slot.GetChild (1).GetComponent<Text>();
			Text ammo = name.transform.GetChild (0).GetComponent<Text>();
			Button b = slot.GetChild (2).GetComponent<Button> ();
			name.text = Weapon.name;
			slot.GetChild (3).gameObject.SetActive (true);
			slot.GetChild(3).GetComponent<Image> ().sprite=Weapon.GetComponent<Linkpic>().Pic;
			Shooting sh = Weapon.GetComponent<Shooting> ();
			if (sh)
			{
				b.gameObject.SetActive (true);
				ammo.text = sh.current_ammo + "/" + sh.total_ammo;
				b.onClick.RemoveAllListeners ();
				b.onClick.AddListener(delegate {button_click(int.Parse(slot.transform.GetChild(0).GetComponent<Text>().text));});
			}
			else
			{
				b.gameObject.SetActive (false);
				ammo.text="";
			}
		}
		for (int i = WeaponHolder.transform.childCount; i < 5; i++)
		{
			Transform slot = Bag.transform.GetChild (i);
			Text name = slot.GetChild (1).GetComponent<Text>();
			Text ammo = name.transform.GetChild (0).GetComponent<Text>();
			name.text = "Empty Slot" ;
			slot.GetChild (3).gameObject.SetActive(false);
			ammo.text = "";
			slot.GetChild (2).gameObject.SetActive (false);

		}
	}
	void Update()
	{
		if(Input.GetButtonDown ("Inventory")) 
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			Time.timeScale = 1;
			gameObject.SetActive (false);
		}
	}

	void button_click(int i)
	{
		i -= 1;
		weapon_holder.GetComponentInParent<WeaponSwitch> ().drop (i);
		show (weapon_holder);
	}
}
