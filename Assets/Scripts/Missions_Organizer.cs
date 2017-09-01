using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;


[System.Serializable]
public class Mission  
{
	public GameObject Victim ;
	public GameObject Target;
	public Transform Police_cars;
	public int mission_time_in_seconds;
}

public class Missions_Organizer : MonoBehaviour 
{
	GameObject Player;
	public Mission[] missions;
	Mission rndm_mission;
	bool target_died;
	Text timer;
	bool player_died;
	Transform pointer;
	public Transform WareHouse;
	public AudioSource victory;
	void Start()
	{
		Player = GameObject.FindGameObjectWithTag ("Player_Main");

		timer =	GameObject.FindGameObjectWithTag ("Timer").transform.GetChild (0).GetComponent<Text> ();
		timer.gameObject.SetActive (true);
		if ((missions.Length == 0))
			return;
		rndm_mission = missions[Random.Range (0, missions.Length)];
		rndm_mission.Target.SetActive (true);

		pointer = GameObject.FindGameObjectWithTag ("Arrow").transform.GetChild(0);
		pointer.gameObject.SetActive (true);
		GameObject.FindGameObjectWithTag ("Info").GetComponent<Text> ().text = "Kill your target!";
	}

	void Update()
	{
		if (pointer == null)
		{
			pointer = GameObject.FindGameObjectWithTag ("Arrow").transform.GetChild(0);
			pointer.gameObject.SetActive (true);
		}
		if (Player == null) 
		{
			Player = GameObject.FindGameObjectWithTag ("Player_Main");
			return;
		}

		if (Time.timeScale>0) 
		{
			pointer_towards(rndm_mission.Victim!=null?rndm_mission.Victim.transform.position:WareHouse.transform.position);
			timer.text = "Timer : " + Mathf.Floor (Time.timeSinceLevelLoad / 60) + ":" +Mathf.Floor( Time.timeSinceLevelLoad % 60) + "/" + Mathf.Floor (rndm_mission.mission_time_in_seconds / 60) + ":" +Mathf.Floor( rndm_mission.mission_time_in_seconds % 60);
			if (rndm_mission.Victim==null&&!target_died) 
			{
				rndm_mission.Police_cars.gameObject.SetActive (true);
				timer.gameObject.SetActive (false);
				target_died = true;
				foreach (Transform car in rndm_mission.Police_cars)
				{
					car.gameObject.GetComponent<UnityStandardAssets.Vehicles.Car.CarAIControl>().m_Target= Player.transform;

				}
				GameObject.FindGameObjectWithTag ("Info").GetComponent<Text> ().text = "Target killed! Escape the cops!";

			}
			if ((!player_died&&Time.timeSinceLevelLoad > rndm_mission.mission_time_in_seconds&&(rndm_mission.Victim!=null&&rndm_mission.Victim.GetComponent<enemyShooting> ().escaped))) 
			{
				player_died = true;
				Player.GetComponent<Main_Health> ().die ();

			}
		}
	}

	void OnTriggerEnter(Collider c)
	{
		if (!rndm_mission.Victim&&(c.gameObject.CompareTag ("Player_Main")||c.gameObject.CompareTag ("Player")||(c.gameObject.CompareTag ("Vehicle")&&c.GetComponentInParent<VehicleController>().isInsideTheCar))) 
		{
			victory.Play ();
			Transform mdlMsg = GameObject.FindGameObjectWithTag ("Game_Over").transform.GetChild (0);
			mdlMsg.GetComponent<Text> ().text = "Mission Passed !";
			mdlMsg.GetComponent<Text> ().color = Color.green;
			mdlMsg.gameObject.SetActive (true);
			float sc = (rndm_mission.mission_time_in_seconds - Time.timeSinceLevelLoad) * 2;
			if(sc < rndm_mission.mission_time_in_seconds / 2)
				sc = rndm_mission.mission_time_in_seconds / 2;
			Player.GetComponent<Main_Health> ().add_score (sc);
			StartCoroutine (Stop_level(5));
		}
	}
	IEnumerator Stop_level(float delay)
	{
		yield return new WaitForSeconds (delay);
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		SceneManager.LoadScene ("Main");
	}

	void pointer_towards(Vector3 object_location)
	{
		pointer.eulerAngles = new Vector3(-60,Quaternion.LookRotation (object_location - pointer.position).eulerAngles.y,0);
	}
}
