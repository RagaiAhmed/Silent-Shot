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
	[Tooltip("All vehicles in the scene containing the 'MS Vehicle Controller' component must be associated with this list.")]
	public GameObject[] vehicles;
	[Space(10)][Tooltip("This variable is responsible for defining the vehicle in which the player will start. It represents an index of the 'vehicles' list, where the number placed here represents the index of the list. The selected index will be the starting vehicle.")]
	public int startingVehicle = 0;
	[Tooltip("The player must be associated with this variable. This variable should only be used if your scene also has a player other than a vehicle. This \"player\" will temporarily be disabled when you get in a vehicle, and will be activated again when you leave a vehicle.")]
	public GameObject player;
	[Tooltip("If this variable is true and if you have a player associated with the 'player' variable, the game will start at the player. Otherwise, the game will start in the starting vehicle, selected in the variable 'startingVehicle'.")]
	public bool startInPlayer = false;
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

	int currentVehicle = 0;
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
		CheckEqualKeyCodes ();
		Time.timeScale = 1;
		error = false;
		currentVehicle = startingVehicle;

		if (startingVehicle >= vehicles.Length) {
			error = true;
			Debug.LogError ("Vehicle selected to start does not exist in the 'vehicles' list");
		}
		for (int x = 0; x < vehicles.Length; x++) {
			if (vehicles [x]) {
				if (!vehicles [x].GetComponent<VehicleController> ()) {
					error = true;
					Debug.LogError ("The vehicle associated with the index " + x + " does not have the 'VehicleController' component. So it will be disabled.");
				}
			}else{
				error = true;
				Debug.LogError ("No vehicle was associated with the index " + x + " of the vehicle list.");
			}
		}
		if (error) {
			for (int x = 0; x < vehicles.Length; x++) {
				if (vehicles [x]) {
					VehicleController component = vehicles [x].GetComponent<VehicleController> ();
					if (component) {
						component.disableVehicle = true;
					}
					vehicles [x].SetActive (false);
				}
			}
			return;
		}
		//
		for (int x = 0; x < vehicles.Length; x++) {
			if (vehicles [x]) {
				vehicles [x].GetComponent<VehicleController> ().isInsideTheCar = false;
			}
		}
		if (player) {
			player.SetActive (false);
		}
		if (startInPlayer) {
			if (player) {
				player.SetActive (true);
			} else {
				startInPlayer = false;
				if (vehicles.Length > startingVehicle && vehicles [currentVehicle]) {
					vehicles [startingVehicle].GetComponent<VehicleController> ().isInsideTheCar = true;
				}
			}
		} else {
			if (vehicles.Length > startingVehicle && vehicles [currentVehicle]) {
				vehicles [startingVehicle].GetComponent<VehicleController> ().isInsideTheCar = true;
			}
		}
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

	void Update () {
		if (!error) {
			#region customizeInputsValues
			verticalInput = Input.GetAxis (_verticalInput);
			horizontalInput = Input.GetAxis (_horizontalInput);
			mouseXInput = Input.GetAxis (_mouseXInput);
			mouseYInput = Input.GetAxis (_mouseYInput);
			mouseScrollWheelInput = Input.GetAxis (_mouseScrollWheelInput);
			#endregion

			vehicleCode = vehicles [currentVehicle].GetComponent<VehicleController> ();

			if (InputBroker.GetKeyDown (controls.enterEndExit) && !blockedInteraction && player && controls.enable_enterEndExit_Input) {
				if (vehicles.Length <= 1) {
					if (vehicleCode.isInsideTheCar) {
						vehicleCode.ExitTheVehicle ();
						if (player) {
							player.SetActive (true);
							if (vehicleCode.doorPosition[0].transform.position != vehicles [currentVehicle].transform.position) {
								player.transform.position = vehicleCode.doorPosition[0].transform.position;
							} else {
								player.transform.position = vehicleCode.doorPosition[0].transform.position + Vector3.up * 3.0f;
							}
						}
						blockedInteraction = true;
						StartCoroutine ("WaitToInteract");
					} else {
						currentDistanceTemp = Vector3.Distance (player.transform.position, vehicleCode.doorPosition[0].transform.position);
						if (currentDistanceTemp < minDistance) {
							vehicleCode.EnterInVehicle ();
							if (player) {
								player.SetActive (false);
							}
							blockedInteraction = true;
							StartCoroutine ("WaitToInteract");
						}
					}
				} else {
					proximityObjectIndex = 0;
					proximityDoorIndex = 0;
					for (int x = 0; x < vehicles.Length; x++) {
						controllerTemp = vehicles [x].GetComponent<VehicleController> ();
						if (controllerTemp.doorPosition.Length > 0) {
							for (int y = 0; y < controllerTemp.doorPosition.Length; y++) {
								currentDistanceTemp = Vector3.Distance (player.transform.position, controllerTemp.doorPosition [y].transform.position);
								proximityDistanceTemp = Vector3.Distance (player.transform.position, vehicles [proximityObjectIndex].GetComponent<VehicleController> ().doorPosition [proximityDoorIndex].transform.position);
								if (currentDistanceTemp < proximityDistanceTemp) {
									proximityObjectIndex = x;
									proximityDoorIndex = y;
								}
							}
						}
					}
					
					if (vehicleCode.isInsideTheCar) {
						vehicleCode.ExitTheVehicle ();
						if (player) {
							player.SetActive (true);
							if (vehicleCode.doorPosition[0].transform.position != vehicles [currentVehicle].transform.position) {
								player.transform.position = vehicleCode.doorPosition[0].transform.position;
							} else {
								player.transform.position = vehicleCode.doorPosition[0].transform.position + Vector3.up * 3.0f;
							}
						}
						blockedInteraction = true;
						StartCoroutine ("WaitToInteract");
					} else {
						controllerTemp = vehicles [proximityObjectIndex].GetComponent<VehicleController> ();
						proximityDistanceTemp = Vector3.Distance (player.transform.position, controllerTemp.doorPosition[0].transform.position);
						for (int x = 0; x < controllerTemp.doorPosition.Length; x++) {
							currentDistanceTemp = Vector3.Distance (player.transform.position, controllerTemp.doorPosition [x].transform.position);
							if (currentDistanceTemp < proximityDistanceTemp) {
								proximityDistanceTemp = currentDistanceTemp;
							}
						}
						if (proximityDistanceTemp < minDistance) {
							currentVehicle = proximityObjectIndex;
							vehicles [currentVehicle].GetComponent<VehicleController> ().EnterInVehicle ();
							if (player) {
								player.SetActive (false);
							}
							blockedInteraction = true;
							StartCoroutine ("WaitToInteract");
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

			foreach (GameObject g in vehicles) {
				VehicleController v = g.GetComponent<VehicleController> ();
				if (v.isDestroyed) {
					v.doorPosition = new GameObject[0];
					v.Explode ();
					v.isDestroyed = false;
				}
				if (v.isInsideTheCar)
					player.transform.position = v.transform.position;
			}
		}
	}

	IEnumerator WaitToInteract(){
		yield return new WaitForSeconds (0.7f);
		blockedInteraction = false;
	}

	void EnableVehicle(int index){
		currentVehicle = Mathf.Clamp (currentVehicle, 0, vehicles.Length-1);
		if (index != currentVehicle) {
			if (vehicles [currentVehicle]) {
				for (int x = 0; x < vehicles.Length; x++) {
					vehicles [x].GetComponent<VehicleController> ().ExitTheVehicle ();
				}
				vehicles [currentVehicle].GetComponent<VehicleController> ().EnterInVehicle ();
			}
		}
	}

	void OnGUI(){
		if (!error) {
			if (vehicles.Length > 1 && !player) {
				if (GUI.Button (new Rect (5, 5, 50, 40), "<<")) {
					currentVehicle--;
					EnableVehicle (currentVehicle + 1);
				}
				if (GUI.Button (new Rect (60, 5, 50, 40), ">>")) {
					currentVehicle++;
					EnableVehicle (currentVehicle - 1);
				}
			}
			if (vehicles.Length > 0 && currentVehicle < vehicles.Length && GUIVisualizer && vehicleCode) {
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
