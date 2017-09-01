using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Info : MonoBehaviour {
	string previous = "";

	void Update()
	{
		StartCoroutine (Begin ());
	}

	IEnumerator Begin ()
	{
		Text text = gameObject.GetComponent<Text> ();
		if (text.text != previous) {
			previous = text.text;
			yield return new WaitForSeconds(5);
			text.text = "";
		}
		else
			previous = text.text;
	}
}
