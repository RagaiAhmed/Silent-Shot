using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Multi_Sound : NetworkBehaviour {

	AudioSource audiosrc;
	void Start()
	{
		audiosrc = GetComponent<AudioSource> ();
	}

	[Command]
	public void Cmdplay () 
	{
		Rpcplay ();
	}

	[ClientRpc]
	void Rpcplay()
	{
		//audiosrc.PlayOneShot (aud);
	}
}
