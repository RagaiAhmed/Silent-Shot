using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class pausingScript : MonoBehaviour 
{
	public float i = 1f;
	public float x = -1f;
	public float y;
	public float butActVar = 1;
	public float butActVar2 = -1;
	public GameObject restartButton;
	public GameObject quitButton;
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.P)) 
		{
			Time.timeScale = i + x;
			x = x * -1f;
			y = Time.timeScale;
			if (butActVar == 1) {
				restartButton.SetActive (true);
				quitButton.SetActive (true);
			} else {
				restartButton.SetActive (false);
				quitButton.SetActive (false);
			}
			butActVar = butActVar + butActVar2;
			butActVar2 = butActVar2 * -1f;
		}
	}
}
