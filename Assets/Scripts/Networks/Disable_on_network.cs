using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Disable_on_network : NetworkBehaviour
{

	void Start () 
	{
		Debug.Log (isLocalPlayer); 
	}

	void Update () 
	{
		
	}

}
