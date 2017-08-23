using Random = UnityEngine.Random;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class WheelClass {
	[Tooltip("In this variable you must associate the mesh of the wheel of this class.")]
	public Transform wheelMesh;
	[Tooltip("In this variable you must associate the collider of the wheel of this class.")]
	public WheelCollider wheelCollider;
	[Tooltip("If this variable is true, this wheel will receive engine torque.")]
	public bool wheelDrive = true;
	[Tooltip("If this variable is true, the wheel associated with this index will receive rotation defined by the flywheel.")]
	public bool wheelTurn = false;
	[Range(-2.0f,2.0f)][Tooltip("In this variable you can set the horizontal offset of the sliding mark of this wheel.")]
	public float skidMarkShift = 0.0f;
	[HideInInspector]
	public Vector3 wheelWorldPosition;
}

[Serializable]
public class VehicleWheelsClass {
	[Tooltip("The front right wheel collider must be associated with this variable")]
	public WheelClass rightFrontWheel;
	[Tooltip("The front left wheel collider must be associated with this variable")]
	public WheelClass leftFrontWheel;
	[Tooltip("The rear right wheel collider should be associated with this variable")]
	public WheelClass rightRearWheel;
	[Tooltip("The rear left wheel collider should be associated with this variable")]
	public WheelClass leftRearWheel;
}
	
[Serializable]
public class VehicleAdjustmentClass {
	[Tooltip("In this variable an empty object affiliated to the vehicle should be associated with the center position of the vehicle, perhaps displaced slightly downward, with the intention of representing the center of mass of the vehicle.")]
	public Transform centerOfMass;
	[Tooltip("In this variable you must associate the object that represents the steering wheel of the vehicle. The pivot of the object must already be correctly rotated to avoid problems.")]
	public GameObject volant;
	[HideInInspector]
	public AnimationCurve angle_x_Velocity = new AnimationCurve(new Keyframe(0, 1),new Keyframe(100, 0.6f),new Keyframe(500, 0.3f));
	[Tooltip("If this variable is true, the vehicle will start with the engine running. But this only applies if the player starts inside this vehicle.")]
	public bool startOn = true;
	[Range(0.01f,1.0f)][Tooltip("This variable defines the ABS brake force of the vehicle.")]
	public float brakeForce = 0.125f;
	[Range(500,2000000)][Tooltip("In this variable you must define the mass that the vehicle will have. Common vehicles usually have a mass around 1500")]
	public int vehicleMass = 2000;
	[Space(10)][Tooltip("In this class you can adjust some forces that the vehicle receives, such as gravity simulation.")]
	public AerodynamicAdjustmentClass _aerodynamics;
	[Tooltip("In this class, you can set the amount of help the vehicle will receive to rotate. This helps prevent the vehicle from slipping.")]
	public StabilizeSlippageClass stabilizeSlippage;
	[Tooltip("In this class, you can set the amount of help the vehicle will receive to rotate. This rotation follows the angular velocity of the vehicle.")]
	public StabilizeTheAngularRotationClass improveRotation;
	[Tooltip("In this variable there are some variables that allow to improve the control of the vehicle.")]
	public StabilizeTurnsClass improveControl;
}
[Serializable]
public class StabilizeSlippageClass {
	[Tooltip("If this variable is true, the script will help the vehicle to rotate more realistically. This will help the vehicle, causing it to slip less in tight corners.")]
	public bool stabilize = false;
	[Range(0.1f,1.0f)][Tooltip("In this variable you can adjust how perceptible this script help will be.")]
	public float adjustment = 0.6f;
}
[Serializable]
public class StabilizeTheAngularRotationClass {
	[Tooltip("If this variable is true, the script will help the vehicle rotate more realistically.")]
	public bool stabilize = true;
	[Range(0.1f,10.0f)][Tooltip("In this variable you can adjust how perceptible this script help will be.")]
	public float adjustment = 2.0f;
}
[Serializable]
public class StabilizeTurnsClass {
	[Tooltip("If this variable is true, the script will force the wheels of the vehicle automatically, making it difficult for the vehicle to tip over. But this also makes the suspension of the vehicle more rigid.")]
	public bool stabilize = true;
	[Space(7)][Range(0.0f,1.2f)][Tooltip("How much the code will stabilize the vehicle's skidding.")]
	public float tireSlipsFactor = 0.85f;
	[Range(0.1f,2.0f)][Tooltip("This variable defines how much lateral force the vehicle will receive when the steering wheel is rotated. This helps the vehicle to rotate more realistically.")]
	public float helpToTurn = 0.35f;
	[Range(0.1f,1.0f)][Tooltip("This variable defines how fast the vehicle will straighten automatically. This occurs naturally in a vehicle when it exits a curve.")]
	public float helpToStraightenOut = 0.1f;
	[Range(0.1f,4.0f)][Tooltip("This variable defines how much downforce the vehicle will receive. This helps to simulate a more realistic gravity, but should be set up carefully so as not to make some surreal situations.")]
	public float downForce = 3.0f;
	[Range(0.1f,3.0f)][Tooltip("This variable defines how much the vehicle will slip into gullies or very steep slopes.")]
	public float slideOnSlopes = 3.0f;
}

[Serializable]
public class VehicleCamerasClass {
	[Tooltip("If this variable is checked, the script will automatically place the 'IgnoreRaycast' layer on the player when needed.")]
	public bool setLayers = true;
	[Tooltip("Here you must associate all the cameras that you want to control by this script, associating each one with an index and selecting your preferences.")]
	public CameraTypeClass[] cameras;
}
[Serializable]
public class CameraTypeClass {
	[Tooltip("A camera must be associated with this variable. The camera that is associated here, will receive the settings of this index.")]
	public Camera _camera;
	public enum TipoRotac{ETS_StyleCamera}
	[Tooltip("Here you must select the type of rotation and movement that camera will possess.")]
	public TipoRotac rotationType = TipoRotac.ETS_StyleCamera;
}
	
[Serializable]
public class TorqueAdjustmentClass {
	[Range(0.5f,2000.0f)][Tooltip("This variable defines the torque that the motor of the vehicle will have.")]
	public float engineTorque = 3;
	[Range(2,12)][Tooltip("This variable defines the number of gears that the vehicle will have.")]
	public int numberOfGears = 6;
	[Range(0.5f,2.0f)][Tooltip("This variable defines the speed range of each gear. The higher the range, the faster the vehicle goes, however, the torque is relatively lower.")]
	public float speedOfGear = 1.5f;
	[Range(0.5f,2.0f)][Tooltip("In this variable, you can manually adjust the torque that each gear has. But it is not advisable to change these values.")]
	public float[] manualAdjustmentOfTorques = new float[12] { 1.1f, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

	[HideInInspector] public AnimationCurve[] gears = new AnimationCurve[12]{
		new AnimationCurve(new Keyframe(0, 1.5f),new Keyframe(10, 2.0f),new Keyframe(30, 0)),
		new AnimationCurve(new Keyframe(0, 0.2f),new Keyframe(30, 1),new Keyframe(45, 0)),
		new AnimationCurve(new Keyframe(0, 0.2f),new Keyframe(45, 1),new Keyframe(60, 0)),
		new AnimationCurve(new Keyframe(15, 0.0f),new Keyframe(60, 1),new Keyframe(75, 0)),
		new AnimationCurve(new Keyframe(30, 0.0f),new Keyframe(75, 1),new Keyframe(90, 0)),
		new AnimationCurve(new Keyframe(45, 0.0f),new Keyframe(90, 1),new Keyframe(105, 0)),
		new AnimationCurve(new Keyframe(60, 0.0f),new Keyframe(105, 1),new Keyframe(120, 0)),
		new AnimationCurve(new Keyframe(75, 0.0f),new Keyframe(120, 1),new Keyframe(135, 0)),
		new AnimationCurve(new Keyframe(90, 0.0f),new Keyframe(135, 1),new Keyframe(150, 0)),
		new AnimationCurve(new Keyframe(105, 0.0f),new Keyframe(150, 1),new Keyframe(165, 0)),
		new AnimationCurve(new Keyframe(120, 0.0f),new Keyframe(165, 1),new Keyframe(180, 0)),
		new AnimationCurve(new Keyframe(135, 0.0f),new Keyframe(180, 1),new Keyframe(195, 0)),
	};
	[HideInInspector] public int[] minVelocityGears = new int[12] {0,15,30,45,60,75,90,105,120,135,150,165};
	[HideInInspector] public int[] idealVelocityGears = new int[12] {10,30,45,60,75,90,105,120,135,150,165,180};
	[HideInInspector] public int[] maxVelocityGears = new int[12] {30,45,60,75,90,105,120,135,150,165,180,195};
}

[Serializable]
public class VehicleSkidMarksClass {
	[Range(0.1f,6.0f)][Tooltip("This variable defines the width of the vehicle's skid trace.")]
	public float brandWidth = 0.3f;
	[Range(1.0f,10.0f)][Tooltip("This variable sets the sensitivity of the vehicle to start generating traces of skidding. The more sensitive, the easier to generate the traces.")]
	public float sensibility = 2.0f;
}

[Serializable]
public class AerodynamicAdjustmentClass {
	[Tooltip("If this variable is true, the script will simulate a force down on the vehicle, leaving jumps more realistic.")]
	public bool extraGravity = true;
	[Range(0.0f,10.0f)][Tooltip("This variable defines how much force will be added to the vehicle suspension to avoid rotations. This makes the vehicle more rigid and harder to knock over.")]
	public float feelingHeavy = 1.0f;
	[Range(0.00f,1.0f)][Tooltip("This variable defines how much force will be simulated on the vehicle while it is in walls. Too high values make the vehicle stick to walls instead of slipping.")]
	public float horizontalDownForce = 0.3f;
	[Range(0,1.0f)][Tooltip("This variable defines how much force will be simulated in the vehicle while on flat terrain. Values too high cause the suspension to reach the spring limit.")]
	public float verticalDownForce = 0.8f;
}

[Serializable]
public class VehicleSoundsClass {
	[Range(1.5f,6.0f)][Tooltip("This variable defines the speed of the engine sound.")]
	public float speedOfEngineSound = 3.5f;
	[Space(5)][Tooltip("The audio referring to the sound of the engine must be associated here.")]
	public AudioClip engineSound;
	[Tooltip("The default sound that will be emitted when the vehicle slides or skates.")]
	public AudioClip skiddingSound;
	[Tooltip("The sound related to a collision in the wheel must be associated with this variable.")]
	public AudioClip wheelImpactSound;
	[Space(5)][Tooltip("Collision sounds should be associated with this list.")]
	public AudioClip[] collisionSounds;

}
[Serializable]
public class GroundSoundsClass {
	[Tooltip("The tag related to this index. The vehicle will emit the sound associated with this index when it finds land defined with this tag.")]
	public string groundTag;
	[Tooltip("The sound that the wheels will emit when they find a terrain defined by the tag of this index.")]
	public AudioClip groundSound;
	[Range(0.01f,1.0f)][Tooltip("The sound volume associated with the variable 'groundSound'")]
	public float volume = 0.15f;
}

[RequireComponent(typeof(Rigidbody))]
public class VehicleController : MonoBehaviour {

	[Space(7)][Tooltip("In this variable, empty objects must be associated with positions close to the vehicle doors.")]
	public GameObject[] doorPosition;

	[Space(7)][Tooltip("In this class must be configured the cameras that the vehicle has.")]
	public VehicleCamerasClass _cameras;

	[Tooltip("In this class you can configure the vehicle torque, number of gears and their respective torques.")]
	public TorqueAdjustmentClass _vehicleTorque;

	[Tooltip("In this class you can adjust various settings that allow changing the way the vehicle is controlled, as well as the initial state of some variables, such as the engine and the brake.")]
	public VehicleAdjustmentClass _vehicleSettings;

	[Tooltip("In this class, you can adjust all wheels of the vehicle separately, each with its own settings.")]
	public VehicleWheelsClass _wheels;

	[Tooltip("In this class, you can adjust all vehicle sounds, and the preferences of each.")]
	public VehicleSoundsClass _sounds;

	[Tooltip("In this class, you can adjust all preferences in relation to vehicle skid marks, such as color, width, among other options.")]
	public VehicleSkidMarksClass _skidMarks;
	[Tooltip("In this variable, the 'SkidMarks' shader must be associated. Otherwise, the vehicle will not generate skid marks.")]
	public Shader skidMarksShader;

	[Tooltip("The ParticleSystem to be used for explosion events.")]
	public ParticleSystem _explosionParticle;

	[Tooltip("The sound effect to be used for explosion events.")]
	public AudioClip _explosionAudio;

	[Tooltip("The lights of the car.")]
	public GameObject _carLight;

	float verticalInput = 0;
	float horizontalInput = 0;
	float mouseXInput = 0;
	float mouseYInput = 0;

	bool changinGears;
	bool changinGearsAuto;
	bool theEngineIsRunning;
	bool enableEngineSound;
	bool youCanCall;
	bool brakingAuto;
	bool colliding;

	int groundedWheels;
	float sumRPM;
	float mediumRPM;
	float angle1Ref;
	float angle2Volant;
	float volantStartRotation;
	float leftDifferential;
	float rightDifferential;
	float timeAutoGear;
	float previousRotation;
	float reverseForce;
	float engineInput;
	float angleRefVolant;
	float lastKnownTorque = 0;
	float pitchAUD = 1;
	float speedLerpSound = 1;
	float engineSoundFactor;

	float maxAngleVolant;

	float torqueM;
	float adjustTorque;

	Vector3 axisFromRotate;
	Vector3 torqueForceAirRotation;

	Vector2 tireSlipTireSlips;
	Vector2 tireForceTireSlips;
	Vector2 localRigForceTireSlips;
	Vector2 localVelocityWheelTireSlips;
	Vector2 localSurfaceForceDTireSlips;
	Vector2 rawTireForceTireSlips;
	Vector2 tempLocalVelocityVector2;
	Vector3 tempWheelVelocityVector3;
	Vector3 velocityLocalWheelTemp;
	Vector2 surfaceLocalForce;
	Vector3 surfaceLocalForceTemp;
	Vector3 wheelSpeedLocalSurface;
	Vector3 downForceUPTemp;
	float normalTemp;
	float forceFactorTempLocalSurface;
	float downForceTireSlips;
	float estimatedSprungMass;
	float angularWheelVelocityTireSlips;
	float wheelMaxBrakeSlip;
	float minSlipYTireSlips;
	float maxFyTireSlips;

	bool forwardSKid;
	float velxCurrentRPM;
	float nextPitchAUD;
	float sensibilitySkid1;
	float sensibilitySKid2;
	float maxSKidSensibility;
	float skidForwardFD;
	float skidForwardFE;
	float skidForwardTD;
	float skidForwardTE;
	float skidLatFD;
	float skidLatFE;
	float skidLatTD;
	float skidLatTE;

	float rpmFD;
	float rpmFE;
	float rpmTD;
	float rpmTE;

	float lastRightForwardPositionY;
	float lastLeftForwardPositionY;
	float lastRightRearPositionY;
	float lastLeftRearPositionY;
	float sensImpactFR;
	float sensImpactFL;
	float sensImpactRR;
	float sensImpactRL;
	float additionalCurrentGravity;
	float currentBrakeValue;
	float forceEngineBrake;

	float currentDownForceVehicle;
	float startDownForceVehicle;

	Rigidbody ms_Rigidbody;

	AudioSource engineSoundAUD;
	AudioSource beatsOnWheelSoundAUD;
	AudioSource skiddingSoundAUD;
	AudioSource beatsSoundAUD;

	Vector2 tireSL;
	Vector2 tireFO;

	Vector3 lateralForcePointTemp;
	Vector3 forwardForceTemp;
	Vector3 lateralForceTemp;
	float distanceXForceTemp;

	WheelHit tempWheelHit;

	Quaternion tempRotStabilizers;
	float leftFrontForce;
	float rightFrontForce;
	float leftRearForce;
	float rightRearForce;
	float ajustInclinacaoRoolForces;
	float roolForce1;
	float roolForce2;

	float gravityValueFixedUpdate;
	float downForceValueFixedUpdate;
	float inclinationFactorFixedUpdate;

	bool isBraking;
	float brakeVerticalInput;
	float handBrake_Input;
	float totalFootBrake;
	float totalHandBrake;
	float absBrakeInput;
	float absSpeedFactor;

	bool wheelFDIsGrounded;
	bool wheelFEIsGrounded;
	bool wheelTDIsGrounded;
	bool wheelTEIsGrounded;

	int[] arrayTrianglesSkids;
	float alphaTempSKids;
	float alphaAplicSkids;
	float distanceTempSkids;
	Color tempColorSkids;
	Color[] arrayColorSkids;
	Vector2[] tempUVSKids;
	Vector3 tempSKidOfSkidMarks;
	Vector3[] arrayVerticesSkids;
	Vector3[] arrayNormalsSkids;
	int verLenghtSkids;

	bool changeTypeCamera;
	float timeScaleSpeed;
	float minDistanceOrbitalCamera;
	Quaternion xQuaternionCameras;
	Quaternion yQuaternionCameras;
	Quaternion newRotationCameras;
	Quaternion currentRotationCameras;
	Vector3 newDistanceCameras;
	Vector3 currentPositionCameras;
	Vector3 positionCameras;
	RaycastHit hitCameras;

	int indexCamera = 0;
	float rotationXETS = 0.0f;
	float rotationYETS = 0.0f;
	Quaternion[] startRotationCameras;
	float[] distanceOrbitCamera;
	Vector3[] startETSCamerasPosition;

	bool wheelFRSkidding;
	bool wheelFLSkidding;
	bool wheelRRSkidding;
	bool wheelRLSkidding;
	Mesh meshWheelFR;
	Mesh meshWheelFL;
	Mesh meshWheelRR;
	Mesh meshWheelRL;
	Vector3[] last;

	int bulletholes = 0;

	Vector3 vectorMeshPos1;
	Vector3 vectorMeshPos2;
	Vector3 vectorMeshPos3;
	Vector3 vectorMeshPos4;
	Quaternion quatMesh1;
	Quaternion quatMesh2;
	Quaternion quatMesh3;
	Quaternion quatMesh4;

	[HideInInspector]
	public bool isDestroyed = false;

	public float health = 100;
	[HideInInspector]
	public float KMh;
	[HideInInspector]
	public int currentGear;
	[HideInInspector]
	public bool disableVehicle = false;
	[HideInInspector] 
	public bool handBrakeTrue;
	[HideInInspector] 
	public bool isInsideTheCar;

	SceneController controls;

	void Awake(){
		DebugStartErrors ();
		SetCameras ();
	}

	void DebugStartErrors(){
		if (disableVehicle) {
			this.transform.gameObject.SetActive (false);
			return;
		}
		controls = GetComponent<SceneController>();
		if (!controls) {
			Debug.LogError ("There must be an object with the 'MSSceneController' component so that vehicles can be managed.");
			this.transform.gameObject.SetActive (false);
			return;
		}
		//
		bool isOnTheList =controls!=null ;

		if (!isOnTheList) {
			Debug.LogError ("This vehicle can not be controlled because it is not associated with the vehicle list of the scene controller (object that has the 'MSSceneController' component).");
			this.transform.gameObject.SetActive (false);
			return;
		}
		//
		if(!_wheels.rightFrontWheel.wheelCollider || !_wheels.leftFrontWheel.wheelCollider || !_wheels.rightRearWheel.wheelCollider || !_wheels.leftRearWheel.wheelCollider){
			Debug.LogError ("The vehicle must have at least the four main wheels associated with its variables, within class '_wheels'.");
			this.transform.gameObject.SetActive (false);
			return;
		}
		if(!_wheels.rightFrontWheel.wheelMesh || !_wheels.leftFrontWheel.wheelMesh || !_wheels.rightRearWheel.wheelMesh || !_wheels.leftRearWheel.wheelMesh){
			Debug.LogError ("The meshes of the four main wheels must be associated with their respective variables within the class '_wheels'.");
			this.transform.gameObject.SetActive (false);
			return;
		}
	}

	void Start(){
		SetValues ();
		if (skidMarksShader) {
			SetSkidMarksValues ();
		}
	}

	void SetValues(){
		forceEngineBrake = 0.6f * _vehicleSettings.vehicleMass;
		last = new Vector3[4];

		if (doorPosition.Length == 0) {
			doorPosition = new GameObject[1];
		}
		for (int x = 0; x < doorPosition.Length; x++) {
			if (!doorPosition[x]) {
				doorPosition[x] = new GameObject ("doorPos");
				doorPosition[x].transform.position = transform.position;
			}
			doorPosition[x].transform.rotation = transform.rotation;
			doorPosition[x].transform.parent = transform;
		}

		if (!isInsideTheCar) {
			EnableCameras (-1);
			_vehicleSettings.startOn = false;
		} else {
			EnableCameras (indexCamera);
		}

		currentDownForceVehicle = _vehicleSettings.improveControl.downForce;
		startDownForceVehicle = _vehicleSettings.improveControl.downForce;

		speedLerpSound = 5;
		youCanCall = true;
		handBrakeTrue = false;

		if (isInsideTheCar) {
			theEngineIsRunning = _vehicleSettings.startOn;
			if (theEngineIsRunning) {
				StartCoroutine ("StartEngineCoroutine", true);
				StartCoroutine ("TurnOffEngineTime");
			} else {
				StartCoroutine ("StartEngineCoroutine", false);
				StartCoroutine ("TurnOffEngineTime");
			}
		} else {
			theEngineIsRunning = false;
			StartCoroutine ("StartEngineCoroutine", false);
			StartCoroutine ("TurnOffEngineTime");
		}

		ms_Rigidbody = GetComponent <Rigidbody> ();
		ms_Rigidbody.useGravity = true;
		ms_Rigidbody.mass = _vehicleSettings.vehicleMass;
		ms_Rigidbody.drag = 0.0f;
		ms_Rigidbody.angularDrag = 0.05f;
		ms_Rigidbody.maxAngularVelocity = 14.0f;
		ms_Rigidbody.maxDepenetrationVelocity = 8.0f;
		additionalCurrentGravity = 4.0f * ms_Rigidbody.mass;
		ms_Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		ms_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

		WheelCollider WheelColliders = GetComponentInChildren<WheelCollider>();
		WheelColliders.ConfigureVehicleSubsteps(1000.0f, 30, 30);

		if (_vehicleSettings.centerOfMass) {
			ms_Rigidbody.centerOfMass = transform.InverseTransformPoint(_vehicleSettings.centerOfMass.position);
		} else {
			ms_Rigidbody.centerOfMass = Vector3.zero;
		}
		if (_vehicleSettings.volant) {
			volantStartRotation = _vehicleSettings.volant.transform.localEulerAngles.z;
		}

		enableEngineSound = false;
		if (_sounds.engineSound) {
			engineSoundAUD = GenerateAudioSource ("Sound of engine", 10, 0, _sounds.engineSound, true, true, true);
		}
		if (_sounds.wheelImpactSound) {
			beatsOnWheelSoundAUD = GenerateAudioSource ("Sound of wheel beats", 10, 0.25f, _sounds.wheelImpactSound, false, false, false);
		}
		if (_sounds.skiddingSound) {
			skiddingSoundAUD = GenerateAudioSource ("Sound of skid", 10, 1, _sounds.skiddingSound, false, false, false);
		}
		if (_sounds.collisionSounds.Length > 0) {
			if (_sounds.collisionSounds [0]) {
				beatsSoundAUD = GenerateAudioSource ("Sound of beats", 10, 1, _sounds.collisionSounds [UnityEngine.Random.Range (0, _sounds.collisionSounds.Length)], false, false, false);
			}
		}
		skiddingSoundAUD.clip = _sounds.skiddingSound;

		lastRightForwardPositionY = _wheels.rightFrontWheel.wheelMesh.transform.localPosition.y;
		lastLeftForwardPositionY = _wheels.leftFrontWheel.wheelMesh.transform.localPosition.y;
		lastRightRearPositionY = _wheels.rightRearWheel.wheelMesh.transform.localPosition.y;
		lastLeftRearPositionY = _wheels.leftRearWheel.wheelMesh.transform.localPosition.y;

		sensImpactFR = 0.075f * (2.65f * _wheels.rightFrontWheel.wheelCollider.radius);
		sensImpactFL = 0.075f * (2.65f * _wheels.leftFrontWheel.wheelCollider.radius);
		sensImpactRR = 0.075f * (2.65f * _wheels.rightRearWheel.wheelCollider.radius);
		sensImpactRL = 0.075f * (2.65f * _wheels.leftRearWheel.wheelCollider.radius);
	}

	void SetCameras(){
		startRotationCameras = new Quaternion[_cameras.cameras.Length];
		startETSCamerasPosition = new Vector3[_cameras.cameras.Length];
		distanceOrbitCamera = new float[_cameras.cameras.Length];
		for (int x = 0; x < _cameras.cameras.Length; x++) {
			distanceOrbitCamera[x] = 5.0f;
			if (!_cameras.cameras [x]._camera) {
				Debug.LogError ("No camera was associated with variable 'CamerasDoVeiculo.cameras [" + x + "]', therefore an orbital camera will be automatically created in its place.");
				GameObject newCamera = new GameObject ("OrbitalCamera" + x) as GameObject;
				newCamera.AddComponent (typeof(Camera));
				newCamera.AddComponent (typeof(FlareLayer));
				newCamera.AddComponent (typeof(GUILayer));
				newCamera.AddComponent (typeof(AudioListener));
				_cameras.cameras [x]._camera = newCamera.GetComponent<Camera>();
				newCamera.transform.parent = transform;
				newCamera.transform.localPosition = new Vector3 (0, 0, 0);
				_cameras.cameras [x].rotationType = CameraTypeClass.TipoRotac.ETS_StyleCamera;
			}
			if (_cameras.cameras [x].rotationType == CameraTypeClass.TipoRotac.ETS_StyleCamera) {
				startRotationCameras [x] = _cameras.cameras [x]._camera.transform.localRotation;
				startETSCamerasPosition [x] = _cameras.cameras [x]._camera.transform.localPosition;
			}
			AudioListener _audListner = _cameras.cameras [x]._camera.GetComponent<AudioListener> ();
			if (!_audListner) {
				_cameras.cameras [x]._camera.transform.gameObject.AddComponent (typeof(AudioListener));
			}
		}
		if (_cameras.cameras.Length > 0) {
			for (int x = 0; x < _cameras.cameras.Length; x++) {
				_cameras.cameras [x]._camera.transform.tag = "MainCamera";
				Camera componentCameraX = _cameras.cameras [x]._camera.GetComponent<Camera> ();
				if (_cameras.cameras [x].rotationType == CameraTypeClass.TipoRotac.ETS_StyleCamera) {
					componentCameraX.nearClipPlane = 0.03f;
				}
			}
		}
	}

	void AjustLayers(){
		transform.gameObject.layer = 2;
		foreach (Transform trans in this.gameObject.GetComponentsInChildren<Transform>(true)) {
			trans.gameObject.layer = 2;
		}
	}

	void EnableCameras(int nextIndex){
		if (_cameras.cameras.Length > 0) {
			if (nextIndex == -1) {
				for (int x = 0; x < _cameras.cameras.Length; x++) {
					_cameras.cameras [x]._camera.gameObject.SetActive (false);
				}
			} else {
				for (int x = 0; x < _cameras.cameras.Length; x++) {
					if (x == nextIndex) {
						_cameras.cameras [x]._camera.gameObject.SetActive (true);
					} else {
						_cameras.cameras [x]._camera.gameObject.SetActive (false);
					}
				}
			}
			changeTypeCamera = false;
		}
	}

	void InputsCameras(){
		if (isInsideTheCar && true) {
			if (Input.GetKeyDown (KeyCode.None) && indexCamera < (_cameras.cameras.Length - 1)) {
				indexCamera++;
				EnableCameras (indexCamera);
			} else if (Input.GetKeyDown (KeyCode.None) && indexCamera >= (_cameras.cameras.Length - 1)) {
				indexCamera = 0;
				EnableCameras (indexCamera);
			}
		}
	}

	void CamerasManager(){
		timeScaleSpeed = 1.0f / Time.timeScale;
		if (!changeTypeCamera) {
			AudioListener.volume = 1;
			changeTypeCamera = true;
		}

		switch (_cameras.cameras[indexCamera].rotationType ) {
		case CameraTypeClass.TipoRotac.ETS_StyleCamera:
			rotationXETS += mouseXInput * 10;
			rotationYETS += mouseYInput * 10;
			positionCameras = new Vector3 (startETSCamerasPosition [indexCamera].x + Mathf.Clamp ((rotationXETS / 50 + 0.7f), -0.7f, 0), startETSCamerasPosition [indexCamera].y, startETSCamerasPosition [indexCamera].z);
			_cameras.cameras [indexCamera]._camera.transform.localPosition = Vector3.Lerp (_cameras.cameras [indexCamera]._camera.transform.localPosition, positionCameras, Time.deltaTime * 10.0f);
			rotationXETS = ClampAngle (rotationXETS, -180, 80);
			rotationYETS = ClampAngle (rotationYETS, -60, 60);
			xQuaternionCameras = Quaternion.AngleAxis (rotationXETS, Vector3.up);
			yQuaternionCameras = Quaternion.AngleAxis (rotationYETS, -Vector3.right);
			newRotationCameras = startRotationCameras [indexCamera] * xQuaternionCameras * yQuaternionCameras;
			_cameras.cameras [indexCamera]._camera.transform.localRotation = Quaternion.Lerp (_cameras.cameras [indexCamera]._camera.transform.localRotation, newRotationCameras, Time.deltaTime * 10.0f * timeScaleSpeed);
			break;
		}
	}
	public static float ClampAngle (float angle, float min, float max){
		if (angle < -360F) { angle += 360F; }
		if (angle > 360F) { angle -= 360F; }
		return Mathf.Clamp (angle, min, max);
	}

	void OnCollisionEnter (Collision collision){
		if (collision.contacts.Length > 0) {
			if (collision.relativeVelocity.magnitude > 5 && collision.contacts [0].thisCollider.gameObject.transform != transform.parent) {
				if (_sounds.collisionSounds.Length > 0) {
					beatsSoundAUD.clip = _sounds.collisionSounds [UnityEngine.Random.Range (0, _sounds.collisionSounds.Length)];
					beatsSoundAUD.PlayOneShot (beatsSoundAUD.clip);
				}
			}
		}
		float otherMass;
		if (collision.rigidbody) {
			otherMass = collision.rigidbody.mass;
		} else {
			otherMass = 1000;
			float force = otherMass * (Mathf.Sqrt (Mathf.Pow (collision.relativeVelocity.x, 2) + Mathf.Pow (collision.relativeVelocity.y, 2) + Mathf.Pow (collision.relativeVelocity.z, 2)));
			if (force > 1) {
				health -= Mathf.RoundToInt (force / 5000);
			}
		}
	}

	void Update(){

		wheelFDIsGrounded = _wheels.rightFrontWheel.wheelCollider.isGrounded;
		wheelFEIsGrounded = _wheels.leftFrontWheel.wheelCollider.isGrounded;
		wheelTDIsGrounded = _wheels.rightRearWheel.wheelCollider.isGrounded;
		wheelTEIsGrounded = _wheels.leftRearWheel.wheelCollider.isGrounded;

		verticalInput = controls.verticalInput;
		horizontalInput = controls.horizontalInput;
		mouseXInput = controls.mouseXInput;
		mouseYInput = controls.mouseYInput;

		KMh = ms_Rigidbody.velocity.magnitude * 3.6f;
		//
		if (!changinGears) {
			engineInput = Mathf.Clamp01 (verticalInput);
		} else {
			engineInput = 0;
		}
		if (isInsideTheCar) {
			if (Input.GetKeyDown (controls.controls.handBrakeInput) && controls.controls.enable_handBrakeInput_Input && Time.timeScale > 0.2f) {
				handBrakeTrue = !handBrakeTrue;
			}
		}
		//
		DiscoverAverageRpm();
		InputsCameras ();
		TurnOnAndTurnOff();
		Sounds ();
		UpdateWheelMeshes ();

		if (isInsideTheCar) {
			AutomaticGears ();
		}

		int currentbullets = 0;
		foreach (Transform child in transform)
		{
			if (child.name.Contains ("Bullet_Hole"))
				currentbullets++;
		}
		if (health >= 1&&health!=0.01F)
			health -= currentbullets - bulletholes;
		bulletholes = currentbullets;
	}

	public void EnterInVehicle(){
		isInsideTheCar = true;
		EnableCameras (indexCamera);
	}

	public void ExitTheVehicle(){
		isInsideTheCar = false;
		EnableCameras (-1);
		handBrakeTrue = true;
	}

	void FixedUpdate(){
		ApplyTorque ();
		Brakes ();
		if (isInsideTheCar) {
			Volant ();
		}
		StabilizeWheelRPM ();
		StabilizeVehicleRotation ();
		StabilizeVehicleRollForces ();
		StabilizeAirRotation ();
		StabilizeAngularRotation ();
		//

		if ((wheelFDIsGrounded && wheelFEIsGrounded) || (wheelTDIsGrounded && wheelTEIsGrounded)) {
			inclinationFactorFixedUpdate = Mathf.Clamp(Mathf.Abs(Vector3.Dot (Vector3.up, transform.up)),_vehicleSettings._aerodynamics.horizontalDownForce,1.0f);
			ms_Rigidbody.AddForce (-transform.up * (ms_Rigidbody.mass*2.0f*inclinationFactorFixedUpdate + (_vehicleSettings._aerodynamics.verticalDownForce * inclinationFactorFixedUpdate * Mathf.Abs(KMh*3.0f) * (ms_Rigidbody.mass/125.0f))));
		}
		if (_vehicleSettings._aerodynamics.extraGravity) {
			gravityValueFixedUpdate = 0;
			if (wheelFDIsGrounded && wheelFEIsGrounded && wheelTDIsGrounded && wheelTEIsGrounded) {
				gravityValueFixedUpdate = 4.0f * ms_Rigidbody.mass * Mathf.Clamp((KMh / 350.0f),0.05f,1.0f);
			} else {
				gravityValueFixedUpdate = 4.0f * ms_Rigidbody.mass * 3.0f;
			}
			additionalCurrentGravity = Mathf.Lerp (additionalCurrentGravity, gravityValueFixedUpdate, Time.deltaTime);
			ms_Rigidbody.AddForce (Vector3.down * additionalCurrentGravity);
		}
		//
		if (_vehicleSettings.improveControl.stabilize) {
			SetWheelForces (_wheels.rightFrontWheel.wheelCollider);
			SetWheelForces (_wheels.leftFrontWheel.wheelCollider);
			SetWheelForces (_wheels.rightRearWheel.wheelCollider);
			SetWheelForces (_wheels.leftRearWheel.wheelCollider);
		}

		//forcaparaBaixo
		downForceValueFixedUpdate = ((startDownForceVehicle * (((KMh/10.0f) + 0.3f)/2.5f)))/_vehicleSettings.improveControl.slideOnSlopes;
		currentDownForceVehicle = Mathf.Clamp (Mathf.Lerp (currentDownForceVehicle, downForceValueFixedUpdate, Time.deltaTime * 2.0f), 0.1f, 4.0f);

		//brakes ABS
		if (wheelFDIsGrounded && wheelFEIsGrounded && wheelTDIsGrounded && wheelTEIsGrounded) {
			absSpeedFactor = Mathf.Clamp (KMh, 70, 150);
			if (currentGear > 0 && mediumRPM > 0) {
				absBrakeInput = Mathf.Abs (Mathf.Clamp (verticalInput, -1.0f, 0.0f));
			} else if (currentGear <= 0 && mediumRPM < 0) {
				absBrakeInput = Mathf.Abs (Mathf.Clamp (verticalInput, 0.0f, 1.0f)) * -1;
			} else {
				absBrakeInput = 0.0f;
			}
			if (isBraking && Mathf.Abs(KMh) > 1.2f) {
				ms_Rigidbody.AddForce (-transform.forward * absSpeedFactor * ms_Rigidbody.mass * _vehicleSettings.brakeForce * absBrakeInput);	
			}
		}
		if (health < 1&&health!=0.01F) 
		{
			isDestroyed = true;
			health = 0.01F;
			if(isInsideTheCar)
				InputBroker.SetKeyDown (KeyCode.T);
		}
	}

	void LateUpdate(){
		if (skidMarksShader) {
			CheckGroundForSKidMarks ();
		}
		if (_cameras.cameras.Length > 0 && Time.timeScale > 0.2f) {
			CamerasManager ();
		}
	}

	void DiscoverAverageRpm(){
		groundedWheels = 0;
		sumRPM = 0;
		if (wheelFDIsGrounded) {
			groundedWheels++;
			sumRPM += _wheels.rightFrontWheel.wheelCollider.rpm;
		}
		if (wheelFEIsGrounded) {
			groundedWheels++;
			sumRPM += _wheels.leftFrontWheel.wheelCollider.rpm;
		}
		if (wheelTDIsGrounded) {
			groundedWheels++;
			sumRPM += _wheels.rightRearWheel.wheelCollider.rpm;
		}
		if (wheelTEIsGrounded) {
			groundedWheels++;
			sumRPM += _wheels.leftRearWheel.wheelCollider.rpm;
		}
		mediumRPM = sumRPM / groundedWheels;
		if (Mathf.Abs (mediumRPM) < 0.01f) {
			mediumRPM = 0.0f;
		}
	}

	void SetWheelForces (WheelCollider wheelCollider){
		wheelCollider.GetGroundHit (out tempWheelHit);
		TireSlips (wheelCollider, tempWheelHit);
		if (wheelCollider.isGrounded){
			distanceXForceTemp = ms_Rigidbody.centerOfMass.y - transform.InverseTransformPoint(wheelCollider.transform.position).y + wheelCollider.radius + (1.0f - wheelCollider.suspensionSpring.targetPosition) * wheelCollider.suspensionDistance;
			lateralForcePointTemp = tempWheelHit.point + wheelCollider.transform.up * _vehicleSettings.improveControl.helpToStraightenOut * distanceXForceTemp;
			forwardForceTemp = tempWheelHit.forwardDir * (tireFO.y)*3.0f;
			lateralForceTemp = tempWheelHit.sidewaysDir * (tireFO.x);
			if (Mathf.Abs(horizontalInput)>0.1f && wheelCollider.steerAngle != 0.0f && Mathf.Sign(wheelCollider.steerAngle) != Mathf.Sign(tireSL.x)){
				lateralForcePointTemp += tempWheelHit.forwardDir * _vehicleSettings.improveControl.helpToTurn;
			}
			ms_Rigidbody.AddForceAtPosition(forwardForceTemp, tempWheelHit.point);
			ms_Rigidbody.AddForceAtPosition(lateralForceTemp, lateralForcePointTemp);
		}
	}

	public Vector2 WheelLocalVelocity(WheelHit wheelHit){
		tempLocalVelocityVector2 = new Vector2(0,0);
		tempWheelVelocityVector3 = ms_Rigidbody.GetPointVelocity(wheelHit.point);
		velocityLocalWheelTemp = tempWheelVelocityVector3 - Vector3.Project(tempWheelVelocityVector3, wheelHit.normal);
		tempLocalVelocityVector2.y = Vector3.Dot(wheelHit.forwardDir, velocityLocalWheelTemp);
		tempLocalVelocityVector2.x = Vector3.Dot(wheelHit.sidewaysDir, velocityLocalWheelTemp);
		return tempLocalVelocityVector2;
	}
	public float AngularVelocity(Vector2 localVelocityVector, WheelCollider wheelCollider){
		wheelCollider.GetGroundHit (out tempWheelHit);
		return (localVelocityVector.y + (tempWheelHit.sidewaysSlip * ((Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput))/2.0f)*(-2.0f))) / wheelCollider.radius;
	}
	public Vector2 LocalSurfaceForce(WheelHit wheelHit){
		wheelSpeedLocalSurface = ms_Rigidbody.GetPointVelocity(wheelHit.point);
		forceFactorTempLocalSurface = Mathf.InverseLerp(1.0f, 0.25f, (wheelSpeedLocalSurface - Vector3.Project(wheelSpeedLocalSurface, wheelHit.normal)).sqrMagnitude);
		if (forceFactorTempLocalSurface > 0.0f){
			normalTemp = Vector3.Dot(Vector3.up, wheelHit.normal);
			if (normalTemp > 0.000001f){
				downForceUPTemp = Vector3.up * wheelHit.force / normalTemp;
				surfaceLocalForceTemp = downForceUPTemp - Vector3.Project(downForceUPTemp, wheelHit.normal);
			}
			else{
				surfaceLocalForceTemp = Vector3.up * 1000000.0f;
			}
			surfaceLocalForce.y = Vector3.Dot(wheelHit.forwardDir, surfaceLocalForceTemp);
			surfaceLocalForce.x = Vector3.Dot(wheelHit.sidewaysDir, surfaceLocalForceTemp);
			surfaceLocalForce *= forceFactorTempLocalSurface;
		}
		else{
			surfaceLocalForce = Vector2.zero;
		}
		return surfaceLocalForce;
	}

	public void TireSlips(WheelCollider wheelCollider, WheelHit wheelHit){
		localVelocityWheelTireSlips = WheelLocalVelocity (wheelHit);
		localSurfaceForceDTireSlips = LocalSurfaceForce (wheelHit);
		if (KMh > 350.0f) {
			reverseForce = -5 * ms_Rigidbody.velocity.magnitude;
		} else {
			reverseForce = 0;
		}
		angularWheelVelocityTireSlips = AngularVelocity (localVelocityWheelTireSlips, wheelCollider);
		if (wheelCollider.isGrounded){
			estimatedSprungMass = Mathf.Clamp(wheelHit.force / -Physics.gravity.y, 0.0f, wheelCollider.sprungMass) * 0.5f;
			localRigForceTireSlips = (-estimatedSprungMass * localVelocityWheelTireSlips / Time.deltaTime) + localSurfaceForceDTireSlips;
			tireSlipTireSlips.x = localVelocityWheelTireSlips.x;
			tireSlipTireSlips.y = localVelocityWheelTireSlips.y - angularWheelVelocityTireSlips * wheelCollider.radius;
			downForceTireSlips = (currentDownForceVehicle * _vehicleSettings.vehicleMass);
			if (wheelCollider.brakeTorque > 10){
				wheelMaxBrakeSlip = Mathf.Max(Mathf.Abs(localVelocityWheelTireSlips.y * 0.2f),  0.3f);
				minSlipYTireSlips = Mathf.Clamp(Mathf.Abs(reverseForce * tireSlipTireSlips.x) / downForceTireSlips, 0.0f, wheelMaxBrakeSlip);
			}
			else{
				minSlipYTireSlips = Mathf.Min(Mathf.Abs(reverseForce * tireSlipTireSlips.x) / downForceTireSlips, Mathf.Clamp((verticalInput*2.5f),-2.5f,1.0f));
				if (reverseForce != 0.0f && minSlipYTireSlips < 0.1f) minSlipYTireSlips = 0.1f;
			}
			if (Mathf.Abs(tireSlipTireSlips.y) < minSlipYTireSlips) tireSlipTireSlips.y = minSlipYTireSlips * Mathf.Sign(tireSlipTireSlips.y);
			rawTireForceTireSlips = -downForceTireSlips * tireSlipTireSlips.normalized;
			rawTireForceTireSlips.x = Mathf.Abs(rawTireForceTireSlips.x);
			rawTireForceTireSlips.y = Mathf.Abs(rawTireForceTireSlips.y);
			tireForceTireSlips.x = Mathf.Clamp(localRigForceTireSlips.x, -rawTireForceTireSlips.x, +rawTireForceTireSlips.x);
			if (wheelCollider.brakeTorque > 10){
				maxFyTireSlips = Mathf.Min(rawTireForceTireSlips.y, reverseForce);
				tireForceTireSlips.y = Mathf.Clamp(localRigForceTireSlips.y, -maxFyTireSlips, +maxFyTireSlips);
			}
			else{
				tireForceTireSlips.y = Mathf.Clamp(reverseForce, -rawTireForceTireSlips.y, +rawTireForceTireSlips.y);
			}
		}
		else{
			tireSlipTireSlips = Vector2.zero;
			tireForceTireSlips = Vector2.zero;
		}
		tireSL = tireSlipTireSlips*_vehicleSettings.improveControl.tireSlipsFactor;
		tireFO = tireForceTireSlips*_vehicleSettings.improveControl.tireSlipsFactor;
	}

	void OnCollisionStay(){
		colliding = true;
	}
	void OnCollisionExit(){
		colliding = false;
	}

	void StabilizeAngularRotation(){
		if (_vehicleSettings.improveRotation.stabilize) {
			if (Mathf.Abs (horizontalInput) < 0.9f) {
				ms_Rigidbody.angularVelocity = Vector3.Lerp (ms_Rigidbody.angularVelocity, new Vector3 (ms_Rigidbody.angularVelocity.x, 0, ms_Rigidbody.angularVelocity.z), Time.deltaTime * _vehicleSettings.improveRotation.adjustment);
			}
		}
	}

	void StabilizeAirRotation(){
		if (!colliding) {
			if (!wheelFDIsGrounded && !wheelFEIsGrounded && !wheelTDIsGrounded && !wheelTEIsGrounded) {
				axisFromRotate = Vector3.Cross( transform.up, Vector3.up);
				torqueForceAirRotation = axisFromRotate.normalized * axisFromRotate.magnitude * 2.5f;
				torqueForceAirRotation -= ms_Rigidbody.angularVelocity;
				ms_Rigidbody.AddTorque(torqueForceAirRotation * ms_Rigidbody.mass * 0.02f, ForceMode.Impulse);
				if (Mathf.Abs (horizontalInput) > 0.1f) {
					ms_Rigidbody.AddTorque(transform.forward * (-horizontalInput*3.0f)*(_vehicleSettings.vehicleMass/4.0f));
				} 
				if (Mathf.Abs (verticalInput) > 0.1f) {
					ms_Rigidbody.AddTorque(transform.right * (verticalInput*2.25f)*(_vehicleSettings.vehicleMass/4.0f));
				} 
			}
		}
	}

	void StabilizeWheelRPM(){
		if (currentGear > 0) {
			if (KMh > (_vehicleTorque.maxVelocityGears [currentGear - 1] * _vehicleTorque.speedOfGear) && Mathf.Abs (verticalInput) < 0.5f) {
				if (_wheels.rightFrontWheel.wheelDrive) {
					_wheels.rightFrontWheel.wheelCollider.brakeTorque = forceEngineBrake;
				}
				if (_wheels.leftFrontWheel.wheelDrive) {
					_wheels.leftFrontWheel.wheelCollider.brakeTorque = forceEngineBrake;
				}
				if (_wheels.rightRearWheel.wheelDrive) {
					_wheels.rightRearWheel.wheelCollider.brakeTorque = forceEngineBrake;
				}
				if (_wheels.leftRearWheel.wheelDrive) {
					_wheels.leftRearWheel.wheelCollider.brakeTorque = forceEngineBrake;
				}
			} 
		} else if (currentGear == -1) {
			if (KMh > (_vehicleTorque.maxVelocityGears [0] * _vehicleTorque.speedOfGear) && Mathf.Abs (verticalInput) < 0.5f) {
				if (_wheels.rightFrontWheel.wheelDrive) {
					_wheels.rightFrontWheel.wheelCollider.brakeTorque = forceEngineBrake / 5.0f;
				}
				if (_wheels.leftFrontWheel.wheelDrive) {
					_wheels.leftFrontWheel.wheelCollider.brakeTorque = forceEngineBrake / 5.0f;
				}
				if (_wheels.rightRearWheel.wheelDrive) {
					_wheels.rightRearWheel.wheelCollider.brakeTorque = forceEngineBrake / 5.0f;
				}
				if (_wheels.leftRearWheel.wheelDrive) {
					_wheels.leftRearWheel.wheelCollider.brakeTorque = forceEngineBrake / 5.0f;
				}
			}
		}
	}

	void StabilizeVehicleRotation(){
		_wheels.rightFrontWheel.wheelCollider.GetGroundHit(out tempWheelHit);
		if (tempWheelHit.normal == Vector3.zero) { return; }
		_wheels.leftFrontWheel.wheelCollider.GetGroundHit(out tempWheelHit);
		if (tempWheelHit.normal == Vector3.zero) { return; }
		_wheels.rightRearWheel.wheelCollider.GetGroundHit(out tempWheelHit);
		if (tempWheelHit.normal == Vector3.zero) { return; }
		_wheels.leftRearWheel.wheelCollider.GetGroundHit(out tempWheelHit);
		if (tempWheelHit.normal == Vector3.zero) { return; }

		if (_vehicleSettings.stabilizeSlippage.stabilize) {
			if (Mathf.Abs (previousRotation - transform.eulerAngles.y) < 10f) {
				var tempQuaternion = (transform.eulerAngles.y - previousRotation) * _vehicleSettings.stabilizeSlippage.adjustment;
				tempRotStabilizers = Quaternion.AngleAxis (tempQuaternion, Vector3.up);
				ms_Rigidbody.velocity = tempRotStabilizers * ms_Rigidbody.velocity;
			}
		}
		previousRotation = transform.eulerAngles.y;
	}

	void StabilizeVehicleRollForces(){
		leftFrontForce = 1.0f;
		rightFrontForce = 1.0f;
		leftRearForce = 1.0f;
		rightRearForce = 1.0f;
		//CHECAR COLISOES
		//rodasTraz
		bool isGround1 = _wheels.leftRearWheel.wheelCollider.GetGroundHit(out tempWheelHit);
		if (isGround1) {
			leftRearForce = (-_wheels.leftRearWheel.wheelCollider.transform.InverseTransformPoint (tempWheelHit.point).y - _wheels.leftRearWheel.wheelCollider.radius) / _wheels.leftRearWheel.wheelCollider.suspensionDistance;
		}
		bool isGround2 = _wheels.rightRearWheel.wheelCollider.GetGroundHit(out tempWheelHit);
		if (isGround2) {
			rightRearForce = (-_wheels.rightRearWheel.wheelCollider.transform.InverseTransformPoint (tempWheelHit.point).y - _wheels.rightRearWheel.wheelCollider.radius) / _wheels.rightRearWheel.wheelCollider.suspensionDistance;
		}
		//rodasFrente
		bool isGround3 = _wheels.leftFrontWheel.wheelCollider.GetGroundHit(out tempWheelHit);
		if (isGround3) {
			leftFrontForce = (-_wheels.leftFrontWheel.wheelCollider.transform.InverseTransformPoint (tempWheelHit.point).y - _wheels.leftFrontWheel.wheelCollider.radius) / _wheels.leftFrontWheel.wheelCollider.suspensionDistance;
		}
		bool isGround4 = _wheels.rightFrontWheel.wheelCollider.GetGroundHit(out tempWheelHit);
		if (isGround4) {
			rightFrontForce = (-_wheels.rightFrontWheel.wheelCollider.transform.InverseTransformPoint (tempWheelHit.point).y - _wheels.rightFrontWheel.wheelCollider.radius) / _wheels.rightFrontWheel.wheelCollider.suspensionDistance;
		}

		//APLICAR FORCAS DESCOBERTAS
		ajustInclinacaoRoolForces = Mathf.Clamp(Mathf.Abs(Vector3.Dot (Vector3.up, transform.up)),_vehicleSettings._aerodynamics.horizontalDownForce,1.0f);
		roolForce1 = (leftRearForce - rightRearForce) * _vehicleSettings._aerodynamics.feelingHeavy*_vehicleSettings.vehicleMass*ajustInclinacaoRoolForces;
		roolForce2 = (leftFrontForce - rightFrontForce) * _vehicleSettings._aerodynamics.feelingHeavy*_vehicleSettings.vehicleMass*ajustInclinacaoRoolForces;
		//rodasTraz
		if (isGround1) {
			ms_Rigidbody.AddForceAtPosition (_wheels.leftRearWheel.wheelCollider.transform.up * -roolForce1, _wheels.leftRearWheel.wheelCollider.transform.position); 
		}
		if (isGround2) {
			ms_Rigidbody.AddForceAtPosition (_wheels.rightRearWheel.wheelCollider.transform.up * roolForce1, _wheels.rightRearWheel.wheelCollider.transform.position); 
		}
		//rodasFrente
		if (isGround3) {
			ms_Rigidbody.AddForceAtPosition (_wheels.leftFrontWheel.wheelCollider.transform.up * -roolForce2, _wheels.leftFrontWheel.wheelCollider.transform.position); 
		}
		if (isGround4) {
			ms_Rigidbody.AddForceAtPosition (_wheels.rightFrontWheel.wheelCollider.transform.up * roolForce2, _wheels.rightFrontWheel.wheelCollider.transform.position); 
		}
	}

	public AudioSource GenerateAudioSource(string name, float minDistance, float volume, AudioClip audioClip, bool loop, bool playNow, bool playAwake){
		GameObject audioSource = new GameObject(name);
		audioSource.transform.position = transform.position;
		audioSource.transform.parent = transform;
		AudioSource temp = audioSource.AddComponent<AudioSource>() as AudioSource;
		temp.minDistance = minDistance;
		temp.volume = volume;
		temp.clip = audioClip;
		temp.loop = loop;
		temp.playOnAwake = playAwake;
		temp.spatialBlend = 1.0f;
		temp.dopplerLevel = 0.0f;
		if (playNow) {
			temp.Play ();
		}
		return temp;
	}

	void Sounds(){
		sensibilitySkid1 = (1.0f / _skidMarks.sensibility);
		sensibilitySKid2 = (3.0f / _skidMarks.sensibility);
		maxSKidSensibility = (1.2f/_skidMarks.sensibility);

		//SOM DO MOTOR
		engineSoundFactor = 1;
		if (changinGears) {
			engineSoundFactor = Mathf.Lerp (engineSoundFactor, 0.5f, Time.deltaTime*0.5f);
		}
		if (currentGear == -1 || currentGear == 0) {
			velxCurrentRPM = (Mathf.Clamp (KMh, (_vehicleTorque.minVelocityGears [0] * _vehicleTorque.speedOfGear), (_vehicleTorque.maxVelocityGears [0] * _vehicleTorque.speedOfGear)));
			pitchAUD = Mathf.Clamp (((velxCurrentRPM / (_vehicleTorque.maxVelocityGears [0] * _vehicleTorque.speedOfGear))*_sounds.speedOfEngineSound*engineSoundFactor), 0.85f, _sounds.speedOfEngineSound);
		} else {
			velxCurrentRPM = (Mathf.Clamp (KMh, (_vehicleTorque.minVelocityGears [currentGear-1] * _vehicleTorque.speedOfGear), (_vehicleTorque.maxVelocityGears [currentGear-1] * _vehicleTorque.speedOfGear)));
			nextPitchAUD = ((velxCurrentRPM / (_vehicleTorque.maxVelocityGears [currentGear-1] * _vehicleTorque.speedOfGear)) * _sounds.speedOfEngineSound*engineSoundFactor);
			if (KMh < (_vehicleTorque.minVelocityGears [currentGear-1] * _vehicleTorque.speedOfGear)) {
				nextPitchAUD = 0.85f;
				speedLerpSound = 0.5f;
			} else {
				if (speedLerpSound < 4.9f) {
					speedLerpSound = Mathf.Lerp (speedLerpSound, 5.0f, Time.deltaTime);
				}
			}
			pitchAUD = Mathf.Clamp (nextPitchAUD, 0.85f, _sounds.speedOfEngineSound);
		}
		if (_sounds.engineSound) {
			if (theEngineIsRunning) {
				engineSoundAUD.volume = Mathf.Lerp (engineSoundAUD.volume, Mathf.Clamp (Mathf.Abs (engineInput), 0.35f, 0.85f), Time.deltaTime*5.0f);
				if (handBrakeTrue || currentGear == 0) {
					engineSoundAUD.pitch = Mathf.Lerp (engineSoundAUD.pitch, 0.85f + Mathf.Abs(verticalInput)*(_sounds.speedOfEngineSound*0.7f-0.85f), Time.deltaTime * 5.0f);
				} else {
					engineSoundAUD.pitch = Mathf.Lerp (engineSoundAUD.pitch, pitchAUD, Time.deltaTime * speedLerpSound);
				}
			} else {
				if (enableEngineSound) {
					engineSoundAUD.volume = 1;
					engineSoundAUD.pitch = Mathf.Lerp (engineSoundAUD.pitch, 0.7f, Time.deltaTime);
				} else {
					engineSoundAUD.volume = Mathf.Lerp (engineSoundAUD.volume, 0f, Time.deltaTime);
					engineSoundAUD.pitch = Mathf.Lerp (engineSoundAUD.pitch, 0f, Time.deltaTime);
				}
			}
		}

		//SOM IMPACTO RODA
		if (_sounds.wheelImpactSound) {
			if (Mathf.Abs (lastRightForwardPositionY - _wheels.rightFrontWheel.wheelMesh.transform.localPosition.y) > sensImpactFR) {
				beatsOnWheelSoundAUD.PlayOneShot (beatsOnWheelSoundAUD.clip);
			}
			if (Mathf.Abs (lastLeftForwardPositionY - _wheels.leftFrontWheel.wheelMesh.transform.localPosition.y) > sensImpactFL) {
				beatsOnWheelSoundAUD.PlayOneShot (beatsOnWheelSoundAUD.clip);
			}
			if (Mathf.Abs (lastRightRearPositionY - _wheels.rightRearWheel.wheelMesh.transform.localPosition.y) > sensImpactRR) {
				beatsOnWheelSoundAUD.PlayOneShot (beatsOnWheelSoundAUD.clip);
			}
			if (Mathf.Abs (lastLeftRearPositionY - _wheels.leftRearWheel.wheelMesh.transform.localPosition.y) > sensImpactRL) {
				beatsOnWheelSoundAUD.PlayOneShot (beatsOnWheelSoundAUD.clip);
			}
			//
			lastRightForwardPositionY = _wheels.rightFrontWheel.wheelMesh.transform.localPosition.y;
			lastLeftForwardPositionY = _wheels.leftFrontWheel.wheelMesh.transform.localPosition.y;
			lastRightRearPositionY = _wheels.rightRearWheel.wheelMesh.transform.localPosition.y;
			lastLeftRearPositionY = _wheels.leftRearWheel.wheelMesh.transform.localPosition.y;
		}
		// SONS DE DERRAPAGEM------------------------------------------------------------------------------------------------------------------------------------
		if (_sounds.skiddingSound) {

			if (wheelFDIsGrounded) {
				_wheels.rightFrontWheel.wheelCollider.GetGroundHit (out tempWheelHit);
				skidLatFD = Mathf.Abs (tempWheelHit.sidewaysSlip);
				skidForwardFD = Mathf.Abs (tempWheelHit.forwardSlip);
			}

			if (wheelFEIsGrounded) {
				_wheels.leftFrontWheel.wheelCollider.GetGroundHit (out tempWheelHit);
				skidLatFE = Mathf.Abs (tempWheelHit.sidewaysSlip);
				skidForwardFE = Mathf.Abs (tempWheelHit.forwardSlip);
			}

			if (wheelTDIsGrounded) {
				_wheels.rightRearWheel.wheelCollider.GetGroundHit (out tempWheelHit);
				skidLatTD = Mathf.Abs (tempWheelHit.sidewaysSlip);
				skidForwardTD = Mathf.Abs (tempWheelHit.forwardSlip);
			}

			if (wheelTEIsGrounded) {
				_wheels.leftRearWheel.wheelCollider.GetGroundHit (out tempWheelHit);
				skidLatTE = Mathf.Abs (tempWheelHit.sidewaysSlip);
				skidForwardTE = Mathf.Abs (tempWheelHit.forwardSlip);
			}

			rpmFD = Mathf.Abs(_wheels.rightFrontWheel.wheelCollider.rpm);
			rpmFE = Mathf.Abs(_wheels.leftFrontWheel.wheelCollider.rpm);
			rpmTD = Mathf.Abs(_wheels.rightRearWheel.wheelCollider.rpm);
			rpmTE = Mathf.Abs(_wheels.leftRearWheel.wheelCollider.rpm);

			//DERRAPAGEM HORIZONTAL
			skiddingSoundAUD.volume = ((skidLatFD + skidLatFE + skidLatTD + skidLatTE) / 4.0f) * 1.5f;

			//PATINANDO OU RODA TRAVADA=
			forwardSKid = false;
			if (KMh > (75.0f/_skidMarks.sensibility)) {
				if ((rpmFD < sensibilitySKid2) || (rpmFE < sensibilitySKid2) || (rpmTD < sensibilitySKid2) || (rpmTE < sensibilitySKid2)) {
					if (wheelFDIsGrounded || wheelFEIsGrounded || wheelTDIsGrounded || wheelTEIsGrounded) {
						forwardSKid = true;
						skiddingSoundAUD.volume = 0.45f;
					}
				}
			}
			if (KMh < 20.0f * (Mathf.Clamp(_skidMarks.sensibility,1,3))){
				if ((skidForwardFD > maxSKidSensibility)||(skidForwardFE > maxSKidSensibility)||(skidForwardTD > maxSKidSensibility)||(skidForwardTE > maxSKidSensibility)) {
					if (wheelFDIsGrounded || wheelFEIsGrounded || wheelTDIsGrounded || wheelTEIsGrounded) {
						forwardSKid = true;
						skiddingSoundAUD.volume = 0.45f;
					}
				}
			}

			skiddingSoundAUD.clip = _sounds.skiddingSound;
			//TOCAR OS SONS
			if (((skidLatFD > sensibilitySkid1) && wheelFDIsGrounded) ||
				((skidLatFE > sensibilitySkid1) && wheelFEIsGrounded) ||
				((skidLatTD > sensibilitySkid1) && wheelTDIsGrounded) ||
				((skidLatTE > sensibilitySkid1) && wheelTEIsGrounded) || (forwardSKid)) {
				if (!skiddingSoundAUD.isPlaying) {
					skiddingSoundAUD.Play ();
				}
			}
			else {
				skiddingSoundAUD.volume = Mathf.Lerp (skiddingSoundAUD.volume, 0, Time.deltaTime * 30.0f);
				if (skiddingSoundAUD.volume < 0.3f) {
					skiddingSoundAUD.Stop ();
				}
			}
		}
	}

	void TurnOnAndTurnOff(){
		if (youCanCall && isInsideTheCar && true) {
			if ((Input.GetKeyDown (KeyCode.None) && !theEngineIsRunning) || (Mathf.Abs(verticalInput) > 0.5f && !theEngineIsRunning)) {
				enableEngineSound = true;
				if (_sounds.engineSound) {
					engineSoundAUD.pitch = 0.5f;
				}
				StartCoroutine ("StartEngineCoroutine", true);
				StartCoroutine ("StartEngineTime");
			}
			if (Input.GetKeyDown (KeyCode.None) && theEngineIsRunning) {
				StartCoroutine ("StartEngineCoroutine", false);
				StartCoroutine ("TurnOffEngineTime");
			}
		}
		if (!isInsideTheCar && theEngineIsRunning) {
			StartCoroutine ("StartEngineCoroutine", false);
			StartCoroutine ("TurnOffEngineTime");
		}
	}
	IEnumerator StartEngineTime(){
		youCanCall = false;
		yield return new WaitForSeconds (3);
		youCanCall = true;
	}
	IEnumerator TurnOffEngineTime(){
		youCanCall = false;
		yield return new WaitForSeconds (1);
		youCanCall = true;
	}
	IEnumerator StartEngineCoroutine(bool startOn){
		if (startOn) {
			yield return new WaitForSeconds (1.5f);
			theEngineIsRunning = true;
		} else {
			enableEngineSound = false;
			theEngineIsRunning = false;
		}
	}

	IEnumerator ChangeGears(int gear){
		changinGears = true;
		yield return new WaitForSeconds(0.4f);
		changinGears = false;
		currentGear = gear;
	}
		
	void AutomaticGears(){
		if (currentGear == 0 || (mediumRPM < 5 && mediumRPM >= 0)) {
			currentGear = 1;
		}
		if (mediumRPM < -0.1f && Mathf.Abs (verticalInput) < 0.1f) {
			currentGear = -1;
		}
		if (Mathf.Abs (verticalInput) < 0.1f && mediumRPM >= 0 && currentGear < 2) {
			currentGear = 1;
		}
		if (Mathf.Abs (verticalInput) < 0.1 && KMh < 1.0f && currentGear < 2) {
			currentGear = 1;
		}
		if (((Mathf.Abs (Mathf.Clamp (verticalInput, -1f, 0f))) > 0.8f) && KMh < 5) {
			currentGear = -1;
		}
		if (((Mathf.Abs (Mathf.Clamp (verticalInput, 0f, 1f))) > 0.8f) && KMh < 5) {
			currentGear = 1;
		}

		//
		if (currentGear > 0) {
			if (KMh > (_vehicleTorque.idealVelocityGears [currentGear - 1] * _vehicleTorque.speedOfGear + 7 * _vehicleTorque.speedOfGear)) {
				if (currentGear < _vehicleTorque.numberOfGears && !changinGearsAuto && currentGear != -1) {
					timeAutoGear = 1.5f;
					StartCoroutine ("TimeAutoGears", currentGear + 1);
				}
			} else if (KMh < (_vehicleTorque.idealVelocityGears [currentGear - 1] * _vehicleTorque.speedOfGear - 15 * _vehicleTorque.speedOfGear)) {
				if (currentGear > 1 && !changinGearsAuto) {
					timeAutoGear = 0;
					StartCoroutine ("TimeAutoGears", currentGear - 1);
				}
			}
			if (verticalInput > 0.1f && KMh > (_vehicleTorque.idealVelocityGears [currentGear - 1] * _vehicleTorque.speedOfGear + 7 * _vehicleTorque.speedOfGear)) {
				if (currentGear < _vehicleTorque.numberOfGears && currentGear != -1) {
					timeAutoGear = 0.0f;
					StartCoroutine ("TimeAutoGears", currentGear + 1);
				}
			}
		}
	}
	IEnumerator TimeAutoGears(int gear){
		changinGearsAuto = true;
		lastKnownTorque = lastKnownTorque*0.85f;
		yield return new WaitForSeconds(0.4f);
		currentGear = gear;
		yield return new WaitForSeconds(timeAutoGear);
		changinGearsAuto = false;
	}

	void Volant(){
		angle1Ref = Mathf.MoveTowards(angle1Ref, horizontalInput, 2*Time.deltaTime);
		angle2Volant = Mathf.MoveTowards(angle2Volant, horizontalInput, 2*Time.deltaTime);
		//
		maxAngleVolant = 35.0f * _vehicleSettings.angle_x_Velocity.Evaluate (KMh);
		angleRefVolant = Mathf.Clamp (angle1Ref * maxAngleVolant, -maxAngleVolant, maxAngleVolant);

		//APLICAR ANGULO NAS RODAS--------------------------------------------------------------------------------------------------------------
		if (angle1Ref > 0.2f) {
			if (_wheels.rightFrontWheel.wheelTurn) {
				_wheels.rightFrontWheel.wheelCollider.steerAngle = angleRefVolant * 1.2f;
			}
			if (_wheels.leftFrontWheel.wheelTurn) {
				_wheels.leftFrontWheel.wheelCollider.steerAngle = angleRefVolant;
			}
			if (_wheels.rightRearWheel.wheelTurn) {
				_wheels.rightRearWheel.wheelCollider.steerAngle = angleRefVolant * 1.2f;
			}
			if (_wheels.leftRearWheel.wheelTurn) {
				_wheels.leftRearWheel.wheelCollider.steerAngle = angleRefVolant;
			}
		} 
		else if (angle1Ref < -0.2f) {
			if (_wheels.rightFrontWheel.wheelTurn) {
				_wheels.rightFrontWheel.wheelCollider.steerAngle = angleRefVolant;
			}
			if (_wheels.leftFrontWheel.wheelTurn) {
				_wheels.leftFrontWheel.wheelCollider.steerAngle = angleRefVolant * 1.2f;
			}
			if (_wheels.rightRearWheel.wheelTurn) {
				_wheels.rightRearWheel.wheelCollider.steerAngle = angleRefVolant;
			}
			if (_wheels.leftRearWheel.wheelTurn) {
				_wheels.leftRearWheel.wheelCollider.steerAngle = angleRefVolant * 1.2f;
			}
		} else {
			if (_wheels.rightFrontWheel.wheelTurn) {
				_wheels.rightFrontWheel.wheelCollider.steerAngle = angleRefVolant;
			}
			if (_wheels.leftFrontWheel.wheelTurn) {
				_wheels.leftFrontWheel.wheelCollider.steerAngle = angleRefVolant;
			}
			if (_wheels.rightRearWheel.wheelTurn) {
				_wheels.rightRearWheel.wheelCollider.steerAngle = angleRefVolant;
			}
			if (_wheels.leftRearWheel.wheelTurn) {
				_wheels.leftRearWheel.wheelCollider.steerAngle = angleRefVolant;
			}
		}

		if (_vehicleSettings.volant) {
			_vehicleSettings.volant.transform.localEulerAngles = new Vector3 (_vehicleSettings.volant.transform.localEulerAngles.x, _vehicleSettings.volant.transform.localEulerAngles.y, -(volantStartRotation + (angle2Volant * 540.0f)));
		}
	}

	public float VehicleTorque(WheelCollider wheelCollider){
		torqueM = 0;
		if (!isInsideTheCar) {
			return 0;
		}
		if ((Mathf.Abs (verticalInput) < 0.5f) || KMh > 350.0f) {
			return 0;
		}
		if (Mathf.Abs (wheelCollider.rpm*wheelCollider.radius) > (50.0f * _vehicleTorque.numberOfGears*_vehicleTorque.speedOfGear)){
			return 0;
		}
		if (KMh < 0.5f){
			if(Mathf.Abs (wheelCollider.rpm) > (25.0f / wheelCollider.radius)) {
				return 0;
			}
		}
		if (!theEngineIsRunning) {
			return 0;
		}
		if (handBrakeTrue) {
			return 0;
		}
		if (isBraking) {
			return 0;
		}
		if (currentBrakeValue > 0.1f) {
			return 0;
		}
		if (Input.GetKey (controls.controls.handBrakeInput)&& controls.controls.enable_handBrakeInput_Input) {
			return 0;
		}
		if(changinGears){
			return 0;
		}else{
			if (currentGear < 0) {
				torqueM = (500.0f * _vehicleTorque.engineTorque) * (Mathf.Abs(Mathf.Clamp(verticalInput, -1f, 0f))) * (_vehicleTorque.gears [0].Evaluate ((KMh / _vehicleTorque.speedOfGear))) * -0.8f;
			} else if (currentGear == 0) {
				return 0;
			} else {
				torqueM = (500.0f*_vehicleTorque.engineTorque) * (Mathf.Clamp(engineInput, 0f, 1f)) * _vehicleTorque.gears[currentGear-1].Evaluate((KMh/_vehicleTorque.speedOfGear));
			}
		}
		//AJUSTE MANUAL DAS MARCHAS
		adjustTorque = 1;
		if (currentGear < _vehicleTorque.manualAdjustmentOfTorques.Length && currentGear > 0) {
			if (currentGear == -1) {
				adjustTorque = _vehicleTorque.manualAdjustmentOfTorques [0];
			} else if (currentGear == 0) {
				adjustTorque = 0;
			} else if (currentGear > 0) {
				adjustTorque = _vehicleTorque.manualAdjustmentOfTorques [currentGear - 1];
			}
		} else {
			adjustTorque = 1;
		}

		lastKnownTorque = torqueM * adjustTorque;

		return lastKnownTorque;
	}

	void ApplyTorque(){
		leftDifferential = 1+Mathf.Abs((0.2f * Mathf.Abs(Mathf.Clamp (horizontalInput, 0, 1)))*(angleRefVolant/60)); 
		rightDifferential = 1+Mathf.Abs((0.2f * Mathf.Abs(Mathf.Clamp (horizontalInput, -1, 0)))*(angleRefVolant/60)); 
		//torque do motor
		if (theEngineIsRunning && isInsideTheCar) {
			if (_wheels.rightFrontWheel.wheelDrive) {
				_wheels.rightFrontWheel.wheelCollider.motorTorque = VehicleTorque (_wheels.rightFrontWheel.wheelCollider) * rightDifferential;
			}
			if (_wheels.leftFrontWheel.wheelDrive) {
				_wheels.leftFrontWheel.wheelCollider.motorTorque = VehicleTorque (_wheels.leftFrontWheel.wheelCollider) * leftDifferential;
			}
			if (_wheels.rightRearWheel.wheelDrive) {
				_wheels.rightRearWheel.wheelCollider.motorTorque = VehicleTorque (_wheels.rightRearWheel.wheelCollider) * rightDifferential;
			}
			if (_wheels.leftRearWheel.wheelDrive) {
				_wheels.leftRearWheel.wheelCollider.motorTorque = VehicleTorque (_wheels.leftRearWheel.wheelCollider) * leftDifferential;
			}
		} else {
			if (_wheels.rightFrontWheel.wheelDrive) {
				_wheels.rightFrontWheel.wheelCollider.motorTorque = 0;
			}
			if (_wheels.leftFrontWheel.wheelDrive) {
				_wheels.leftFrontWheel.wheelCollider.motorTorque = 0;
			}
			if (_wheels.rightRearWheel.wheelDrive) {
				_wheels.rightRearWheel.wheelCollider.motorTorque = 0;
			}
			if (_wheels.leftRearWheel.wheelDrive) {
				_wheels.leftRearWheel.wheelCollider.motorTorque = 0;
			}
		}
	}

	void Brakes(){
		brakeVerticalInput = 0.0f;
		if (isInsideTheCar) {
			brakeVerticalInput = verticalInput;
		}
		//Freio de pé

		if (currentGear > 0) {
			currentBrakeValue = Mathf.Abs (Mathf.Clamp (brakeVerticalInput, -1.0f, 0.0f))*1.5f;
		} 
		else if (currentGear < 0){
			currentBrakeValue = Mathf.Abs(Mathf.Clamp(brakeVerticalInput, 0.0f, 1.0f))*1.5f;
		}
		else if (currentGear == 0) {
			if (mediumRPM > 0) {
				currentBrakeValue = Mathf.Abs (Mathf.Clamp (brakeVerticalInput, -1.0f, 0.0f))*1.5f;
			} else {
				currentBrakeValue = Mathf.Abs(Mathf.Clamp(brakeVerticalInput, 0.0f, 1.0f))*1.5f;
			}
		}

		// FREIO DE MÃO
		handBrake_Input = 0.0f;
		if (handBrakeTrue) {
			if (Mathf.Abs (brakeVerticalInput) < 0.9f) {
				handBrake_Input = 2;
			} else {
				handBrake_Input = 0;
				handBrakeTrue = false;
			}
		} else {
			handBrake_Input = 0;
		}
		if (Input.GetKey (controls.controls.handBrakeInput) && controls.controls.enable_handBrakeInput_Input) {
			handBrake_Input = 2;
		}
		handBrake_Input = handBrake_Input * 1000;
		//FREIO TOTAL
		totalFootBrake = currentBrakeValue * 0.4f*_vehicleSettings.vehicleMass;
		totalHandBrake = handBrake_Input * 0.4f*_vehicleSettings.vehicleMass;

		if (isInsideTheCar) {
			if (Mathf.Abs (mediumRPM) < 15 && Mathf.Abs (brakeVerticalInput) < 0.05f && !handBrakeTrue && (totalFootBrake + totalHandBrake) < 100) {
				brakingAuto = true;
				totalFootBrake = 3 * 0.4f * _vehicleSettings.vehicleMass;
			} else {
				brakingAuto = false;
			}
		} else {
			brakingAuto = false;
		}

		//freiar\/
		if (totalFootBrake > 10) {
			isBraking = true;
		} else {
			isBraking = false;
		}
		if (!brakingAuto) {
			totalFootBrake = 0;
		}

		_wheels.rightFrontWheel.wheelCollider.brakeTorque = totalFootBrake;
		_wheels.leftFrontWheel.wheelCollider.brakeTorque = totalFootBrake;
		_wheels.rightRearWheel.wheelCollider.brakeTorque = totalFootBrake + totalHandBrake;
		_wheels.leftRearWheel.wheelCollider.brakeTorque = totalFootBrake + totalHandBrake;

		//evitar RPM, freio ou torques invalidos, EvitarRotacaoSemTorque
		BrakeWithoutTorque(_wheels.rightFrontWheel.wheelCollider);
		BrakeWithoutTorque(_wheels.leftFrontWheel.wheelCollider);
		BrakeWithoutTorque(_wheels.rightRearWheel.wheelCollider);
		BrakeWithoutTorque(_wheels.leftRearWheel.wheelCollider);
	}

	void BrakeWithoutTorque(WheelCollider wheelCollider){
		if (!wheelCollider.isGrounded && Mathf.Abs (wheelCollider.rpm) > 0.5f && Mathf.Abs (verticalInput) < 0.05f && wheelCollider.motorTorque < 5.0f) {
			wheelCollider.brakeTorque += _vehicleSettings.vehicleMass * 0.4f;
		}
		if (KMh < 0.5f && Mathf.Abs (verticalInput) < 0.05f) {
			if (wheelCollider.rpm > (25 / wheelCollider.radius)) {
				wheelCollider.brakeTorque += 0.4f * _vehicleSettings.vehicleMass*Mathf.Abs (wheelCollider.rpm)*0.05f;
			}
		}
	}

	void UpdateWheelMeshes(){
		_wheels.rightFrontWheel.wheelCollider.GetWorldPose(out vectorMeshPos1, out quatMesh1);
		_wheels.rightFrontWheel.wheelWorldPosition = _wheels.rightFrontWheel.wheelMesh.position = vectorMeshPos1;
		_wheels.rightFrontWheel.wheelMesh.rotation = quatMesh1;
		//
		_wheels.leftFrontWheel.wheelCollider.GetWorldPose(out vectorMeshPos2, out quatMesh2);
		_wheels.leftFrontWheel.wheelWorldPosition = _wheels.leftFrontWheel.wheelMesh.position = vectorMeshPos2;
		_wheels.leftFrontWheel.wheelMesh.rotation = quatMesh2;
		//
		_wheels.rightRearWheel.wheelCollider.GetWorldPose(out vectorMeshPos3, out quatMesh3);
		_wheels.rightRearWheel.wheelWorldPosition = _wheels.rightRearWheel.wheelMesh.position = vectorMeshPos3;
		_wheels.rightRearWheel.wheelMesh.rotation = quatMesh3;
		//
		_wheels.leftRearWheel.wheelCollider.GetWorldPose(out vectorMeshPos4, out quatMesh4);
		_wheels.leftRearWheel.wheelWorldPosition = _wheels.leftRearWheel.wheelMesh.position = vectorMeshPos4;
		_wheels.leftRearWheel.wheelMesh.rotation = quatMesh4;
	}

	void CheckGroundForSKidMarks(){
		if (_wheels.rightFrontWheel.wheelCollider) {
			if (wheelFDIsGrounded) {
				wheelFRSkidding = GenerateSkidMarks (_wheels.rightFrontWheel.wheelCollider, _wheels.rightFrontWheel.wheelWorldPosition, meshWheelFR, wheelFRSkidding, _wheels.rightFrontWheel.skidMarkShift, 0);
			} else {
				wheelFRSkidding = false;
			}
		}
		//
		if (_wheels.leftFrontWheel.wheelCollider) {
			if (wheelFEIsGrounded) {
				wheelFLSkidding = GenerateSkidMarks (_wheels.leftFrontWheel.wheelCollider,_wheels.leftFrontWheel.wheelWorldPosition , meshWheelFL, wheelFLSkidding, _wheels.leftFrontWheel.skidMarkShift, 1);
			} else {
				wheelFLSkidding = false;
			}
		}
		//
		if (_wheels.rightRearWheel.wheelCollider) {
			if (wheelTDIsGrounded) {
				wheelRRSkidding = GenerateSkidMarks (_wheels.rightRearWheel.wheelCollider,_wheels.rightRearWheel.wheelWorldPosition , meshWheelRR, wheelRRSkidding, _wheels.rightRearWheel.skidMarkShift, 2);
			} else {
				wheelRRSkidding = false;
			}
		}
		//
		if (_wheels.leftRearWheel.wheelCollider) {
			if (wheelTEIsGrounded) {
				wheelRLSkidding = GenerateSkidMarks (_wheels.leftRearWheel.wheelCollider,_wheels.leftRearWheel.wheelWorldPosition , meshWheelRL, wheelRLSkidding, _wheels.leftRearWheel.skidMarkShift , 3);
			} else {
				wheelRLSkidding = false;
			}
		}
	}

	private bool GenerateSkidMarks(WheelCollider wheelCollider,Vector3 wheelPos ,Mesh wheelMesh, bool generateBool, float lateralDisplacement, int indexLastMark) {
		wheelCollider.GetGroundHit (out tempWheelHit);
		tempWheelHit.point = wheelPos - Vector3.up * wheelCollider.radius;

		arrayVerticesSkids = wheelMesh.vertices;
		arrayNormalsSkids = wheelMesh.normals;
		arrayTrianglesSkids = wheelMesh.triangles;
		arrayColorSkids = wheelMesh.colors;
		tempUVSKids = wheelMesh.uv;
		alphaTempSKids = Mathf.Abs(tempWheelHit.sidewaysSlip);
		tempSKidOfSkidMarks = tempWheelHit.sidewaysDir * _skidMarks.brandWidth / 2f * Vector3.Dot(wheelCollider.attachedRigidbody.velocity.normalized, tempWheelHit.forwardDir);
		tempSKidOfSkidMarks -= tempWheelHit.forwardDir * _skidMarks.brandWidth * 0.1f * Vector3.Dot(wheelCollider.attachedRigidbody.velocity.normalized, tempWheelHit.sidewaysDir);

		if(KMh > (75.0f/_skidMarks.sensibility) && Mathf.Abs(wheelCollider.rpm) < (3.0f / _skidMarks.sensibility)){ 
			if (wheelCollider.isGrounded) {
				alphaTempSKids = 10;
			}
		}
		if (KMh < 20.0f * (Mathf.Clamp (_skidMarks.sensibility, 1, 3))) {
			if (Mathf.Abs(tempWheelHit.forwardSlip) > (1.2f/_skidMarks.sensibility)) {
				if (wheelCollider.isGrounded) {
					alphaTempSKids = 10;
				}
			}
		}
		if (alphaTempSKids < (1 / _skidMarks.sensibility)) {
			return false;
		}
		distanceTempSkids = (last[indexLastMark] - tempWheelHit.point).sqrMagnitude;
		alphaAplicSkids = Mathf.Clamp (alphaTempSKids, 0.0f, 1.0f);
		if(generateBool) {
			if (distanceTempSkids < 0.02f) {
				return true;
			}
			Array.Resize(ref arrayTrianglesSkids, arrayTrianglesSkids.Length + 6);
		}
		Array.Resize(ref arrayVerticesSkids, arrayVerticesSkids.Length + 2);
		Array.Resize(ref arrayNormalsSkids, arrayNormalsSkids.Length + 2);
		Array.Resize(ref arrayColorSkids, arrayColorSkids.Length + 2);
		Array.Resize(ref tempUVSKids, tempUVSKids.Length + 2);
		verLenghtSkids = arrayVerticesSkids.Length;
		arrayVerticesSkids[verLenghtSkids - 1] = tempWheelHit.point + tempWheelHit.normal * 0.02f - tempSKidOfSkidMarks + tempWheelHit.sidewaysDir * lateralDisplacement;
		arrayVerticesSkids[verLenghtSkids - 2] = tempWheelHit.point + tempWheelHit.normal * 0.02f + tempSKidOfSkidMarks + tempWheelHit.sidewaysDir * lateralDisplacement;
		arrayNormalsSkids[verLenghtSkids - 1] = arrayNormalsSkids[verLenghtSkids - 2] = tempWheelHit.normal;
		//
		tempColorSkids = new Color(0.15f,0.15f,0.15f,0);
		tempColorSkids.a = Mathf.Clamp (alphaAplicSkids * 0.9f, 0.1f, 1.0f);
		arrayColorSkids[verLenghtSkids - 1] = arrayColorSkids[verLenghtSkids - 2] = tempColorSkids;
		//
		if(generateBool) {
			arrayTrianglesSkids[arrayTrianglesSkids.Length - 1] = verLenghtSkids - 2;
			arrayTrianglesSkids[arrayTrianglesSkids.Length - 2] = verLenghtSkids - 1;
			arrayTrianglesSkids[arrayTrianglesSkids.Length - 3] = verLenghtSkids - 3;
			arrayTrianglesSkids[arrayTrianglesSkids.Length - 4] = verLenghtSkids - 3;
			arrayTrianglesSkids[arrayTrianglesSkids.Length - 5] = verLenghtSkids - 4;
			arrayTrianglesSkids[arrayTrianglesSkids.Length - 6] = verLenghtSkids - 2;
			tempUVSKids[verLenghtSkids - 1] = tempUVSKids[verLenghtSkids - 3] + Vector2.right * distanceTempSkids*0.01f;
			tempUVSKids[verLenghtSkids - 2] = tempUVSKids[verLenghtSkids - 4] + Vector2.right * distanceTempSkids*0.01f;
		}
		else {
			tempUVSKids[verLenghtSkids - 1] = Vector2.zero;
			tempUVSKids[verLenghtSkids - 2] = Vector2.up;
		}
		last[indexLastMark] = arrayVerticesSkids [arrayVerticesSkids.Length - 1];
		wheelMesh.vertices = arrayVerticesSkids;
		wheelMesh.normals = arrayNormalsSkids;
		wheelMesh.triangles = arrayTrianglesSkids;
		wheelMesh.colors = arrayColorSkids;
		wheelMesh.uv = tempUVSKids;
		return true;
	}

	void SetSkidMarksValues(){
		Material skidmarkMaterial = new Material(skidMarksShader);
		skidmarkMaterial.mainTexture = CreateTexture ();
		Color skidColor = new Color(0.15f,0.15f,0.15f,0);
		skidColor.a = 0.7f; 
		skidmarkMaterial.color = skidColor;
		//
		var RDSMFD = new GameObject("RenderDasSkidMarksFD");
		var filterFD = RDSMFD.AddComponent<MeshFilter>();
		var rendFD = RDSMFD.AddComponent<MeshRenderer>();
		RDSMFD.hideFlags = HideFlags.HideAndDontSave;
		filterFD.mesh = meshWheelFR = new Mesh();
		rendFD.material = skidmarkMaterial;
		rendFD.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		meshWheelFR.name = "Skidmark Mesh FD";
		meshWheelFR.MarkDynamic();
		//
		var RDSMFE = new GameObject("RenderDasSkidMarksFE");
		var filterFE = RDSMFE.AddComponent<MeshFilter>();
		var rendFE = RDSMFE.AddComponent<MeshRenderer>();
		RDSMFE.hideFlags = HideFlags.HideAndDontSave;
		filterFE.mesh = meshWheelFL = new Mesh();
		rendFE.material = skidmarkMaterial;
		rendFE.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		meshWheelFL.name = "Skidmark Mesh FE";
		meshWheelFL.MarkDynamic();
		//
		var RDSMTD = new GameObject("RenderDasSkidMarksTD");
		var filterTD = RDSMTD.AddComponent<MeshFilter>();
		var rendTD = RDSMTD.AddComponent<MeshRenderer>();
		RDSMTD.hideFlags = HideFlags.HideAndDontSave;
		filterTD.mesh = meshWheelRR = new Mesh();
		rendTD.material = skidmarkMaterial;
		rendTD.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		meshWheelRR.name = "Skidmark Mesh TD";
		meshWheelRR.MarkDynamic();
		//
		var RDSMTE = new GameObject("RenderDasSkidMarksTE");
		var filterTE = RDSMTE.AddComponent<MeshFilter>();
		var rendTE = RDSMTE.AddComponent<MeshRenderer>();
		RDSMTE.hideFlags = HideFlags.HideAndDontSave;
		filterTE.mesh = meshWheelRL = new Mesh();
		rendTE.material = skidmarkMaterial;
		rendTE.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		meshWheelRL.name = "Skidmark Mesh TE";
		meshWheelRL.MarkDynamic();
	}

	public Texture CreateTexture(){
		var texture = new Texture2D(32, 32, TextureFormat.ARGB32, false);
		Color transparentColor1 = new Color (1.0f, 1.0f, 1.0f, 0.15f);
		Color transparentColor2 = new Color (1.0f, 1.0f, 1.0f, 0.6f);
		for (int x = 0; x < 32; x++) {
			for (int y = 0; y < 32; y++) {
				texture.SetPixel(x, y, Color.white);
			}
		}
		for (int y = 0; y < 32; y++) {
			for (int x = 0; x < 32; x++) {
				if (y == 0 || y == 1 || y == 30 || y == 31) {
					texture.SetPixel (x, y, transparentColor1);
				}
				if (y == 6 || y == 7 || y == 15 || y == 16 || y == 24 || y == 25) {
					texture.SetPixel (x, y, transparentColor2);
				}
			}
		}
		texture.Apply();
		return texture;
	}

	public void Explode()
	{
		_explosionParticle.Play ();
		_carLight.SetActive (false);
		AudioSource.PlayClipAtPoint (_explosionAudio, transform.position);
	}
}
