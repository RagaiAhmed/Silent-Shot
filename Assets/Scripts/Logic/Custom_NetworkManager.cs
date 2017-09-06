using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerChar : MessageBase
{
	public int player_num;
	public int primary;
	public int secondary;
}

public class Custom_NetworkManager : NetworkManager
{
	public GameObject[] Players;
	public GameObject[] Primary;
	public GameObject[] Secondary;
	public GameObject knife;

	public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
	{
		PlayerChar choosed_character = extraMessageReader.ReadMessage<PlayerChar> ();
		playerPrefab = Players [choosed_character.player_num];

		if (playerPrefab == null)
		{
			if (LogFilter.logError) { Debug.LogError("The PlayerPrefab is empty on the NetworkManager. Please setup a PlayerPrefab object."); }
			return;
		}

		if (playerPrefab.GetComponent<NetworkIdentity>() == null)
		{
			if (LogFilter.logError) { Debug.LogError("The PlayerPrefab does not have a NetworkIdentity. Please add a NetworkIdentity to the player prefab."); }
			return;
		}

		if (playerControllerId < conn.playerControllers.Count  && conn.playerControllers[playerControllerId].IsValid && conn.playerControllers[playerControllerId].gameObject != null)
		{
			if (LogFilter.logError) { Debug.LogError("There is already a player at that playerControllerId for this connections."); }
			return;
		}
			
		Transform startPos = GetStartPosition();
		if (startPos == null)
		{
			startPos=transform;
		}

		GameObject player = (GameObject)Instantiate(playerPrefab, startPos.position, startPos.rotation);

		GameObject pr=null, sc=null, kn=null;
		Transform weaponHolder = player.transform;
		foreach (int i in player.GetComponent<WeaponSwitch>().WeaponHolder_path) 
		{
			weaponHolder = weaponHolder.GetChild (i);
		}
		if (choosed_character.primary>0)
			pr=Instantiate (Primary [choosed_character.primary], weaponHolder);
		if (choosed_character.secondary>0)
			sc=Instantiate (Secondary [choosed_character.secondary], weaponHolder);
		kn=Instantiate (knife, weaponHolder);

	
		NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
		if (pr) 
		{
			pr = pr.transform.GetChild (0).gameObject;
			NetworkServer.SpawnWithClientAuthority(pr,player);
		}
		if (sc) 
		{
			sc = sc.transform.GetChild (0).gameObject;

			NetworkServer.SpawnWithClientAuthority(sc,player);

		}
		kn = kn.transform.GetChild (0).gameObject;

		NetworkServer.SpawnWithClientAuthority(kn,player);

		pr.GetComponent<Multi_Child_sync>().RpcPutIn(player,"Root/Hips/Spine/Spine1/RightShoulder/RightArm/RightForeArm/GunHolder");
		sc.GetComponent<Multi_Child_sync>().RpcPutIn(player,"Root/Hips/Spine/Spine1/RightShoulder/RightArm/RightForeArm/GunHolder");
		kn.GetComponent<Multi_Child_sync>().RpcPutIn(player,"Root/Hips/Spine/Spine1/RightShoulder/RightArm/RightForeArm/GunHolder");

	}
		
}


