using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ui_logic : MonoBehaviour {

	public GameObject menu;
	public Button exit;

	void Start ()
	{
		exit.onClick.AddListener (Application.Quit);
	}

	void Update ()
	{
		if (Input.GetButtonDown ("Cancel"))
		{
			menu.SetActive (!menu.activeSelf);
			Cursor.visible = !Cursor.visible;
			if (menu.activeSelf)
			{
				Time.timeScale = 0;
				Cursor.lockState = CursorLockMode.None;
			}
			else 
			{
				Time.timeScale = 1;
				Cursor.lockState = CursorLockMode.Locked;

			}
		}
	}
}
