using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Multi_Spawner : NetworkBehaviour {

	public NetworkManager nt;
	public int player;
	public int primary;
	public int secondary;
	public bool ok;
	void LateUpdate () 
	{
		if (!ok)
			return;
		ok = false;
		PlayerChar m =	new PlayerChar ();
		m.player_num = player ;
		m.primary = primary;
		m.secondary = secondary;
		ClientScene.AddPlayer (nt.client.connection, 0,m);

	}
}
