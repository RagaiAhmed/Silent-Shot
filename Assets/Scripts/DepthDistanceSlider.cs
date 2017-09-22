using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DepthDistanceSlider : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		GetComponent<Slider> ().onValueChanged.AddListener (adjust_sens);
		GetComponent<Slider> ().value = PlayerPrefs.GetInt ("Depth", 360);
	}

	void adjust_sens(float done)
	{
		PlayerPrefs.SetFloat("Depth", done);
		GameObject[] mainCameras = GameObject.FindGameObjectsWithTag ("MainCamera");
		foreach (GameObject g in mainCameras) {
			Camera cam = g.GetComponent<Camera> ();
			if (cam)
				cam.farClipPlane = done;
		}
	}
}
