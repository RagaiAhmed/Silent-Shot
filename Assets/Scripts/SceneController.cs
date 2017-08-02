using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

[Serializable]
public class Controls {
	
	[Tooltip("If this variable is true, the control for this variable will be activated.")]
	public bool enable_reloadScene_Input = true;
	[Tooltip("The key that must be pressed to reload the current scene.")]
	public KeyCode reloadScene = KeyCode.R;

	[Space(10)][Tooltip("If this variable is true, the control for this variable will be activated.")]
	public bool enable_startTheVehicle_Input = true;
	[Tooltip("The key that must be pressed to turn the vehicle engine on or off.")]
	public KeyCode startTheVehicle = KeyCode.F;

	[Space(10)][Tooltip("If this variable is true, the control for this variable will be activated.")]
	public bool enable_enterEndExit_Input = true;
	[Tooltip("The key that must be pressed to enter or exit the vehicle.")]
	public KeyCode enterEndExit = KeyCode.T;

	[Space(10)][Tooltip("If this variable is true, the control for this variable will be activated.")]
	public bool enable_handBrakeInput_Input = true;
	[Tooltip("The key that must be pressed to activate or deactivate the vehicle hand brake.")]
	public KeyCode handBrakeInput = KeyCode.Space;

	[Space(10)][Tooltip("If this variable is true, the control for this variable will be activated.")]
	public bool enable_switchingCameras_Input = true;
	[Tooltip("The key that must be pressed to toggle between the cameras of the vehicle.")]
	public KeyCode switchingCameras = KeyCode.C;
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
	[Tooltip("All vehicles in the scene containing the 'Vehicle Controller' component must be associated with this list.")]
	public GameObject[] vehicles;
	[Space(10)][Tooltip("This variable is responsible for defining the vehicle in which the player will start. It represents an index of the 'vehicles' list, where the number placed here represents the index of the list. The selected index will be the starting vehicle.")]
	public int startingVehicle = 0;
	[Tooltip("The player must be associated with this variable. This variable should only be used if your scene also has a player other than a vehicle. This \"player\" will temporarily be disabled when you get in a vehicle, and will be activated again when you leave a vehicle.")]
	public GameObject player;
	[Tooltip("If this variable is true and if you have a player associated with the 'player' variable, the game will start at the player. Otherwise, the game will start in the starting vehicle, selected in the variable 'startingVehicle'.")]
	public bool startInPlayer = false;
	[Tooltip("This is the minimum distance the player needs to be in relation to the door of a vehicle, to interact with it.")]
	public float minDistance = 3;

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
	string sceneName;
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
		sceneName = SceneManager.GetActiveScene ().name;
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

			if (Input.GetKeyDown (controls.reloadScene) && controls.enable_reloadScene_Input) {
				SceneManager.LoadScene (sceneName);
			}

			if (Input.GetKeyDown (controls.enterEndExit) && !blockedInteraction && player && controls.enable_enterEndExit_Input) {
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

						for (int y = 0; y < controllerTemp.doorPosition.Length; y++) {
							currentDistanceTemp = Vector3.Distance (player.transform.position, controllerTemp.doorPosition[y].transform.position);
							proximityDistanceTemp = Vector3.Distance (player.transform.position, vehicles [proximityObjectIndex].GetComponent<VehicleController> ().doorPosition[proximityDoorIndex].transform.position);
							if (currentDistanceTemp < proximityDistanceTemp) {
								proximityObjectIndex = x;
								proximityDoorIndex = y;
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

			gear = "Gear: " + vehicleCode.currentGear;
			velocitykmh = "Velocity(km/h): " + (int)(vehicleCode.KMh * clampGear);
			velocityms = "Velocity(m/s): " + (int)((vehicleCode.KMh / 3.6f) * clampGear);
			velocitymph = "Velocity(mp/h): " + (int)(vehicleCode.KMh * 0.621371f * clampGear);
			handBrake = "HandBreak: " + vehicleCode.handBrakeTrue;
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
}
