using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Multi_Child_sync : NetworkBehaviour 
{
	[ClientRpc]
	public void RpcPutIn (GameObject t,string strng)
	{
		
		Transform holder = t.transform.Find (strng);
		transform.parent =holder;
	}
}
