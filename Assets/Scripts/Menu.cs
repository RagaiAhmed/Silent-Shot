using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	public Button play,quit;

	void Start () 
	{
		quit.onClick.AddListener (Application.Quit);
		play.onClick.AddListener (load);
	}
	void load()
	{
		SceneManager.LoadScene ("Test");
	}
}
