﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class locker 
{
	public static bool isPlaying=true;
}

public class ui_logic : MonoBehaviour {

	public GameObject menu;
	public Button exit;
	public Button resume;
	public Button main_menu;
	public Button new_game;

	void Start ()
	{
		exit.onClick.AddListener (Application.Quit);
		resume.onClick.AddListener (show_hide);
		main_menu.onClick.AddListener (load_main);
		new_game.onClick.AddListener (reload);

	}

	void Update ()
	{
		if (Input.GetButtonDown ("Cancel"))
		{
			show_hide ();
		}
	}

	void show_hide()
	{
		menu.SetActive (!menu.activeSelf);
		Cursor.visible = !Cursor.visible;
		if (menu.activeSelf)
		{
			locker.isPlaying = false;
			Cursor.lockState = CursorLockMode.None;
		}
		else 
		{
			locker.isPlaying = true;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
	void load_main ()
	{
		SceneManager.LoadScene ("Main");
	}
	void reload()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene().name);

	}
}
