using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	public GameObject welcome;
	public GameObject character_select;
	public Button play;
	public Button quit;
	public Button close_welcome,close_char;
	public Button character_selector;
	public AudioClip soundclick;
	public Button select_weapon;
	public GameObject weapon_selector;
	public Button Close_weapon_selector;
	public Button choose_map_button;
	AudioSource aud;
	public GameObject map_chooser;
	public Button map_chooser_close;
	void Start () 
	{
		aud = GetComponent<AudioSource> ();
		quit.onClick.AddListener (Application.Quit);
		play.onClick.AddListener (load);
		close_welcome.onClick.AddListener (close);
		close_char.onClick.AddListener (close_character);
		character_selector.onClick.AddListener (Char_select);
		select_weapon.onClick.AddListener (open_weapon_selector);
		Close_weapon_selector.onClick.AddListener (close_weapon_selector);
		choose_map_button.onClick.AddListener (open_map_chooser);
		map_chooser_close.onClick.AddListener (close_map_chooser);

	}
	void load()
	{
		aud.PlayOneShot (soundclick);
		float character = PlayerPrefs.GetInt ("Character",-1);

		if (character == -1) 
		{
			welcome.SetActive (true);
		}
		else
		{
			character_select.SetActive (true);
		}

	}

	void close ()
	{
		aud.PlayOneShot (soundclick);
		welcome.SetActive (false);
	}

	void Char_select()
	{
		aud.PlayOneShot (soundclick);
		character_select.SetActive (true);
		welcome.SetActive (false);
	}

	void close_character()
	{
		aud.PlayOneShot (soundclick);

		character_select.SetActive (false);
	}
	void open_weapon_selector()
	{
		aud.PlayOneShot (soundclick);
		if (PlayerPrefs.GetInt ("Character", -1) != -1) 
		{
			character_select.SetActive (false);
			weapon_selector.SetActive (true);
		}
	}
	void close_weapon_selector()
	{		
		aud.PlayOneShot (soundclick);
		weapon_selector.SetActive (false);
		
	}

	void open_map_chooser()
	{
		aud.PlayOneShot (soundclick);
		if (PlayerPrefs.GetInt ("Primary", -1) != -1&&PlayerPrefs.GetInt ("Secondary", -1) != -1)
		{
			weapon_selector.SetActive (false);
			map_chooser.SetActive (true);
		}

	}
	void close_map_chooser()
	{
		aud.PlayOneShot (soundclick);
		map_chooser.SetActive (false);
	}
}
