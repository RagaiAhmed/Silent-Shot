using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;


public class SceneControllerForHelicopters : MonoBehaviour {

	public bool Normal_car=false;
	#region defineInputs
	[Tooltip("Vertical input recognized by the system")]
	public string _verticalInput = "Vertical";

	[Tooltip("Horizontal input recognized by the system")]
	public string _horizontalInput = "Horizontal";

	[Tooltip("Horizontal input for camera movements")]
	public string _mouseXInput = "Mouse X";

	[Tooltip("Vertical input for camera movements")]
	public string _mouseYInput = "Mouse Y";

	[Tooltip("Scroll input, to zoom in and out of the cameras.")]
	public string _mouseScrollWheelInput = "Mouse ScrollWheel";
	#endregion

	//[Tooltip("All vehicles in the scene containing the 'MS Vehicle Controller' component must be associated with this list.")]
	GameObject vehicle;
	//[Space(10)][Tooltip("This variable is responsible for defining the vehicle in which the player will start. It represents an index of the 'vehicles' list, where the number placed here represents the index of the list. The selected index will be the starting vehicle.")]
	//	public int startingVehicle = 0;
	//[Tooltip("The player must be associated with this variable. This variable should only be used if your scene also has a player other than a vehicle. This \"player\" will temporarily be disabled when you get in a vehicle, and will be activated again when you leave a vehicle.")]
	public GameObject player;
	//[Tooltip("If this variable is true and if you have a player associated with the 'player' variable, the game will start at the player. Otherwise, the game will start in the starting vehicle, selected in the variable 'startingVehicle'.")]
	//public bool startInPlayer = false;
	[Tooltip("This is the minimum distance the player needs to be in relation to the door of a vehicle, to interact with it.")]
	public float minDistance = 3;
	[Space(10)][Tooltip("If this variable is true, useful data will appear on the screen, such as the car's current gear, speed, brakes, among other things.")]
	public bool GUIVisualizer = true;

	#region customizeInputs
	[HideInInspector]
	public float verticalInput = 0;
	[HideInInspector]
	public float horizontalInput = 0;
	[HideInInspector]
	public float mouseXInput = 0;
	[HideInInspector]
	public float mouseYInput = 0;
	[HideInInspector]
	public float mouseScrollWheelInput = 0;
	#endregion

	int clampGear;
	int proximityObjectIndex;
	int proximityDoorIndex;
	bool blockedInteraction = false;
	bool error;
	string health;
	string gear;
	string velocitykmh;
	string velocityms;
	string velocitymph;
	string handBrake;
	HelicopterController vehicleCode;
	HelicopterController controllerTemp;
	float currentDistanceTemp;
	float proximityDistanceTemp;

	void Awake () {
		vehicle = gameObject;
		vehicleCode = vehicle.GetComponent<HelicopterController> ();

		Time.timeScale = 1;
		error = false;

		if (!vehicleCode&&!Normal_car)
		{
			error = true;
			Debug.LogError ("The vehicle associated does not have the 'VehicleController' component. So it will be disabled.");
		}

		//


		vehicleCode.ExitTheVehicle ();

	}

	int searchForIndex(object x, object[] a)
	{
		for (int y = 0; y < a.Length; y++) {
			if (a [y] == x)
				return y;
		}
		return -1;
	}

	void Update ()
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player_Main");
		float minimum=-1;
		float dist;
		foreach (GameObject pl in players) 
		{
			dist = Vector3.Distance (pl.transform.position, vehicle.transform.position);
			if (minimum == -1)
				minimum = dist;
			if (dist  <= minimum)
			{
				player = pl;
			}
		}
		if (!error) {
			#region customizeInputsValues
			verticalInput = Input.GetAxis (_verticalInput);
			horizontalInput = Input.GetAxis (_horizontalInput);
			mouseXInput = Input.GetAxis (_mouseXInput);
			mouseYInput = Input.GetAxis (_mouseYInput);
			mouseScrollWheelInput = Input.GetAxis (_mouseScrollWheelInput);
			#endregion


			if (InputBroker.GetButtonDown("EnterExit") && !blockedInteraction && player)
			{
				if (Normal_car)
				{
					Destroy (GetComponent < UnityStandardAssets.Vehicles.Car.CarAudio>());
					Destroy (GetComponent<UnityStandardAssets.Utility.WaypointProgressTracker>());
					Destroy (GetComponent<UnityStandardAssets.Vehicles.Car.CarAIControl>());
					Destroy (GetComponent< UnityStandardAssets.Vehicles.Car.CarController>());
					Destroy (GetComponent<PoliceAI> ());

					vehicleCode.enabled = true;
					Normal_car = false;
				}
				if (vehicle) 
				{
					if (vehicleCode.isInsideTheCar) 
					{
						vehicleCode.ExitTheVehicle ();
						if (player) 
						{
							if (vehicleCode.doorPosition.Length>0&&vehicleCode.doorPosition[0].transform.position != vehicle.transform.position) 
							{
								player.transform.position = vehicleCode.doorPosition[0].transform.position;
							} else
							{
								player.transform.position = vehicleCode.doorPosition[0].transform.position + Vector3.up * 3.0f;
							}
							player.SetActive (true);
						}
						blockedInteraction = true;
						StartCoroutine ("WaitToInteract");
					} 
					else if(player.activeSelf)
					{
						if(vehicleCode.doorPosition.Length>0)
						{
							currentDistanceTemp = Vector3.Distance (player.transform.position, vehicleCode.doorPosition[0].transform.position);
							if (currentDistanceTemp < minDistance)
							{
								vehicleCode.EnterInVehicle ();
								vehicleCode._cameras.cameras[0]._camera.GetComponent<SmoothMouseLook>().sensitivityX = player.GetComponent<Head_Movement> ().Sensitivity;
								vehicleCode._cameras.cameras[0]._camera.GetComponent<SmoothMouseLook>().sensitivityY = player.GetComponent<Head_Movement> ().Sensitivity;
								if (player) {
									player.SetActive (false);
								}
								blockedInteraction = true;
								StartCoroutine ("WaitToInteract");
							}
						}

					}
				}
			}
			if (vehicleCode.isDestroyed) 
			{
				if(vehicleCode.isInsideTheCar)
				{
					vehicleCode.ExitTheVehicle ();
					if (player) 
					{
						if (vehicleCode.doorPosition.Length>0&&vehicleCode.doorPosition[0].transform.position != vehicle.transform.position) 
						{
							player.transform.position = vehicleCode.doorPosition[0].transform.position;
						} else
						{
							player.transform.position = vehicleCode.doorPosition[0].transform.position + Vector3.up * 3.0f;
						}
						player.SetActive (true);
					}
					player.GetComponent<Main_Health> ().RpcDecrease(50F);
					blockedInteraction = true;
					StartCoroutine ("WaitToInteract");
				}
				vehicleCode.doorPosition = new GameObject[0];
				vehicleCode.Explode ();
				vehicleCode.isDestroyed = false;
			}
			if (vehicleCode.isDestroyed) 
			{
				if(vehicleCode.isInsideTheCar)
				{
					vehicleCode.ExitTheVehicle ();
					if (player) 
					{
						if (vehicleCode.doorPosition.Length>0&&vehicleCode.doorPosition[0].transform.position != vehicle.transform.position) 
						{
							player.transform.position = vehicleCode.doorPosition[0].transform.position;
						} else
						{
							player.transform.position = vehicleCode.doorPosition[0].transform.position + Vector3.up * 3.0f;
						}
						player.SetActive (true);
					}
					player.GetComponent<Main_Health> ().RpcDecrease(50F);
					blockedInteraction = true;
					StartCoroutine ("WaitToInteract");
				}
				vehicleCode.doorPosition = new GameObject[0];
				vehicleCode.Explode ();
				vehicleCode.isDestroyed = false;
			}
			if (vehicleCode.isInsideTheCar)
				player.transform.position = vehicleCode.transform.position;
		}
	}

	IEnumerator WaitToInteract(){
		yield return new WaitForSeconds (0.7f);
		blockedInteraction = false;
	}

	void EnableVehicle()
	{
		if (Normal_car)
		{
			Destroy (GetComponent<UnityStandardAssets.Vehicles.Car.CarAIControl> ());
			Destroy (GetComponent<UnityStandardAssets.Vehicles.Car.CarAudio> ());
			Destroy (GetComponent<UnityStandardAssets.Vehicles.Car.CarController> ());


			HelicopterController vc = GetComponent<HelicopterController> ();

			if (vc) 
			{
				vc.enabled = true;
			}

		}
		vehicleCode.EnterInVehicle ();

	}
}
