using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Sounds
{
	public string name;
	public AudioClip[] clips;
}

public class Multi_Sound : NetworkBehaviour {

	AudioSource audiosrc;
	void Start()
	{
		audiosrc = GetComponent<AudioSource> ();

	}

	[Command]
	public void Cmdplay(string Type,string property,bool check_before,int index) 
	{
		Rpcplay(Type,property,check_before,index);
	}

	[ClientRpc]
	void Rpcplay(string t,string property,bool check_before,int index) 
	{
		if (check_before && audiosrc.isPlaying)
			return;
		Object obj = GetComponent (t);

		System.Type type = obj.GetType();

		if (index>=0)
			audiosrc.PlayOneShot(((AudioClip[])type.GetField (property).GetValue(obj))[index]);
		else
			audiosrc.PlayOneShot((AudioClip)type.GetField (property).GetValue(obj));

	}
}
