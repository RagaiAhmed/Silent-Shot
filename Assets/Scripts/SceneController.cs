using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

[Serializable]
public class Controls {

	[Space(10)][Tooltip("If this variable is true, the control for this variable will be activated.")]
	public bool enable_enterEndExit_Input = true;
	[Tooltip("The key that must be pressed to enter or exit the vehicle.")]
	public KeyCode enterEndExit = KeyCode.T;

	[Space(10)][Tooltip("If this variable is true, the control for this variable will be activated.")]
	public bool enable_handBrakeInput_Input = true;
	[Tooltip("The key that must be pressed to activate or deactivate the vehicle hand brake.")]
	public KeyCode handBrakeInput = KeyCode.Space;
}

public class SceneController : MonoBehaviour {

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

	[Space(10)][Tooltip("Here you can configure the vehicle controls, choose the desired inputs and also, deactivate the unwanted ones.")]
	public Controls controls;
	//[Tooltip("All vehicles in the scene containing the 'MS Vehicle Controller' component must be associated with this list.")]
	 GameObject vehicle;
	//[Space(10)][Tooltip("This variable is responsible for defining the vehicle in which the player will start. It represents an index of the 'vehicles' list, where the number placed here represents the index of the list. The selected index will be the starting vehicle.")]
//	public int startingVehicle = 0;
	//[Tooltip("The player must be associated with this variable. This variable should only be used if your scene also has a player other than a vehicle. This \"player\" will temporarily be disabled when you get in a vehicle, and will be activated again when you leave a vehicle.")]
	 GameObject player;
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
	VehicleController vehicleCode;
	VehicleController controllerTemp;
	float currentDistanceTemp;
	float proximityDistanceTemp;

	void Awake () {
		vehicle = gameObject;
		vehicleCode = vehicle.GetComponent<VehicleController> ();

		CheckEqualKeyCodes ();
		Time.timeScale = 1;
		error = false;

		if (!vehicleCode)
		{
			error = true;
			Debug.LogError ("The vehicle associated does not have the 'VehicleController' component. So it will be disabled.");
		}
			
		//


				vehicleCode.ExitTheVehicle ();

	}

	void CheckEqualKeyCodes(){
		var type = typeof(Controls);
		var fields = type.GetFields();
		var values = (from field in fields
			where field.FieldType == typeof(KeyCode)
			select (KeyCode)field.GetValue(controls)).ToArray();

		foreach(var value in values)
			if(Array.FindAll(values, (a) => { return a == value; }).Length > 1)
				Debug.LogError ("There are similar commands in the 'controls' list. Use different keys for each command.");
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


			if (InputBroker.GetKeyDown (controls.enterEndExit) && !blockedInteraction && player && controls.enable_enterEndExit_Input)
			{
				if (vehicle) 
				{
					if (vehicleCode.isInsideTheCar) 
					{
						vehicleCode.ExitTheVehicle ();
						if (player) 
						{
							player.SetActive (true);
							if (vehicleCode.doorPosition.Length>0&&vehicleCode.doorPosition[0].transform.position != vehicle.transform.position) 
							{
								player.transform.position = vehicleCode.doorPosition[0].transform.position;
							} else
							{
								player.transform.position = vehicleCode.doorPosition[0].transform.position + Vector3.up * 3.0f;
							}
						}
						blockedInteraction = true;
						StartCoroutine ("WaitToInteract");
					} 
					else 
					{
						if(vehicleCode.doorPosition.Length>0)
						{
							currentDistanceTemp = Vector3.Distance (player.transform.position, vehicleCode.doorPosition[0].transform.position);
							if (currentDistanceTemp < minDistance)
							{
								vehicleCode.EnterInVehicle ();
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

			health = "Health: " + (int)vehicleCode.health;
			gear = "Gear: " + vehicleCode.currentGear;
			velocitykmh = "Velocity(km/h): " + (int)(vehicleCode.KMh * clampGear);
			velocityms = "Velocity(m/s): " + (int)((vehicleCode.KMh / 3.6f) * clampGear);
			velocitymph = "Velocity(mp/h): " + (int)(vehicleCode.KMh * 0.621371f * clampGear);
			handBrake = "HandBreak: " + vehicleCode.handBrakeTrue;

			if (vehicleCode.isDestroyed) {
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

	void EnableVehicle(int index){
		vehicleCode.EnterInVehicle ();

	}

	void OnGUI(){
		if (!error) {
			if (vehicle && GUIVisualizer && vehicleCode) {
				if (vehicleCode.isInsideTheCar) {
					GUI.Box (new Rect (5, 50, 180, 125), "");
					clampGear = Mathf.Clamp (vehicleCode.currentGear, -1, 1);
					if (clampGear == 0) {
						clampGear = 1;
					}

					GUI.skin.label.fontSize = 14;
					GUI.Label (new Rect (10, 50, 300, 35), gear);
					GUI.Label (new Rect (10, 70, 300, 35), velocitykmh);
					GUI.Label (new Rect (10, 90, 300, 35), velocityms);
					GUI.Label (new Rect (10, 110, 300, 35), velocitymph);
					GUI.Label (new Rect (10, 130, 300, 35), handBrake);
					GUI.Label (new Rect (10, 150, 300, 35), health);
				}
			}
		}
	}
}
