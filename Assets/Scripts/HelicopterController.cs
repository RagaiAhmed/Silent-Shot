
using UnityEngine;
using UnityEngine.UI;

public class HelicopterController : MonoBehaviour
{
    public AudioSource HelicopterSound;
    public Rigidbody HelicopterModel;
    public HeliRotorController MainRotorController;
    public HeliRotorController SubRotorController;
	[Tooltip("In this variable, empty objects must be associated with positions close to the vehicle doors.")]
	public GameObject[] doorPosition;
	[Tooltip("In this class must be configured the cameras that the vehicle has.")]
	public VehicleCamerasClass _cameras;

	[Tooltip("The ParticleSystem to be used for explosion events.")]
	public ParticleSystem _explosionParticle;

	[Tooltip("The sound effect to be used for explosion events.")]
	public AudioClip _explosionAudio;

	[Tooltip("The lights of the car.")]
	public GameObject _heliLight;

	[HideInInspector]
	public float sensitivity = 10;
	[HideInInspector] 
	public bool isInsideTheCar;
	[HideInInspector]
	public bool isDestroyed = false;

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
	float mouseXInput = 0;
	float mouseYInput = 0;
	int bulletholes;

	int indexCamera = 0;
	float rotationXETS = 0.0f;
	float rotationYETS = 0.0f;
	Quaternion[] startRotationCameras;
	float[] distanceOrbitCamera;
	Vector3[] startETSCamerasPosition;
	SceneControllerForHelicopters controls;

    public float TurnForce = 3f;
    public float ForwardForce = 10f;
    public float ForwardTiltForce = 20f;
    public float TurnTiltForce = 30f;
    public float EffectiveHeight = 100f;

    public float turnTiltForcePercent = 1.5f;
    public float turnForcePercent = 1.3f;

	public float health;

    private float _engineForce;
    public float EngineForce
    {
        get { return _engineForce; }
        set
        {
            MainRotorController.RotarSpeed = value * 80;
            SubRotorController.RotarSpeed = value * 40;
            HelicopterSound.pitch = Mathf.Clamp(value / 40, 0, 1.2f);
            _engineForce = value;
        }
    }

    private Vector2 hMove = Vector2.zero;
    private Vector2 hTilt = Vector2.zero;
    private float hTurn = 0f;
    public bool IsOnGround = true;
	  
	void Awake()
	{
		controls = GetComponent<SceneControllerForHelicopters>();
		if (!controls) {
			Debug.LogError ("There must be an object with the 'SceneController' component so that vehicles can be managed.");
			this.transform.gameObject.SetActive (false);
			return;
		}
		SetCameras ();
	}

	void LateUpdate()
	{
		if (_cameras.cameras.Length > 0 && Time.timeScale > 0.2f) {
			//CamerasManager ();
		}
	}

    void FixedUpdate()
    {
        LiftProcess();
        MoveProcess();
        TiltProcess();
		if (isInsideTheCar)
			OnKeyPressed ();
		else
			EngineForce = 0;
		if (health < 1&&health!=0.01F) 
		{
			isDestroyed = true;
			health = 0.01F;
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

    private void MoveProcess()
    {
        var turn = TurnForce * Mathf.Lerp(hMove.x, hMove.x * (turnTiltForcePercent - Mathf.Abs(hMove.y)), Mathf.Max(0f, hMove.y));
        hTurn = Mathf.Lerp(hTurn, turn, Time.fixedDeltaTime * TurnForce);
        HelicopterModel.AddRelativeTorque(0f, hTurn * HelicopterModel.mass, 0f);
        HelicopterModel.AddRelativeForce(Vector3.forward * Mathf.Max(0f, hMove.y * ForwardForce * HelicopterModel.mass));
    }

    private void LiftProcess()
    {
        var upForce = 1 - Mathf.Clamp(HelicopterModel.transform.position.y / EffectiveHeight, 0, 1);
        upForce = Mathf.Lerp(0f, EngineForce, upForce) * HelicopterModel.mass;
        HelicopterModel.AddRelativeForce(Vector3.up * upForce);
    }

    private void TiltProcess()
    {
        hTilt.x = Mathf.Lerp(hTilt.x, hMove.x * TurnTiltForce, Time.deltaTime);
        hTilt.y = Mathf.Lerp(hTilt.y, hMove.y * ForwardTiltForce, Time.deltaTime);
        HelicopterModel.transform.localRotation = Quaternion.Euler(hTilt.y, HelicopterModel.transform.localEulerAngles.y, -hTilt.x);
    }

    private void OnKeyPressed()
    {
        float tempY = 0;
        float tempX = 0;

        // stable forward
        if (hMove.y > 0)
            tempY = - Time.fixedDeltaTime;
        else
            if (hMove.y < 0)
                tempY = Time.fixedDeltaTime;

        // stable lurn
        if (hMove.x > 0)
            tempX = -Time.fixedDeltaTime;
        else
            if (hMove.x < 0)
                tempX = Time.fixedDeltaTime;

		if(Input.GetAxisRaw("Throttle") > 0) 
			EngineForce += 0.1f;
		
		if (Input.GetAxisRaw ("Throttle") < 0) {
			EngineForce -= 0.12f;
			if (EngineForce < 0)
				EngineForce = 0;
		}
		if (Input.GetAxisRaw ("Vertical") > 0) {
			if (!IsOnGround)
				tempY = Time.fixedDeltaTime;
		}
		if (Input.GetAxisRaw ("Vertical") < 0) {
			if (!IsOnGround)
				tempY = -Time.fixedDeltaTime;
		}
		if (Input.GetAxisRaw ("Horizontal") < 0) {
			if (!IsOnGround)
				tempX = -Time.fixedDeltaTime;
		}
		if (Input.GetAxisRaw ("Horizontal") > 0) {
			if (!IsOnGround)
				tempX = Time.fixedDeltaTime;
		}
		if(Input.GetAxisRaw("Yaw") > 0) 
        {
			if (!IsOnGround) {
				var force = (turnForcePercent - Mathf.Abs (hMove.y)) * HelicopterModel.mass;
				HelicopterModel.AddRelativeTorque (0f, force, 0);
			}
        }
		if(Input.GetAxisRaw("Yaw") < 0) 
        {
			if (!IsOnGround) {
				var force = -(turnForcePercent - Mathf.Abs (hMove.y)) * HelicopterModel.mass;
				HelicopterModel.AddRelativeTorque (0f, force, 0);
			}
        }

        hMove.x += tempX;
        hMove.x = Mathf.Clamp(hMove.x, -1, 1);

        hMove.y += tempY;
        hMove.y = Mathf.Clamp(hMove.y, -1, 1);

    }

    private void OnCollisionEnter()
    {
        IsOnGround = true;
    }

    private void OnCollisionExit()
    {
        IsOnGround = false;
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
	void CamerasManager(){
		timeScaleSpeed = 1.0f / Time.timeScale;
		if (!changeTypeCamera) {
			AudioListener.volume = 1;
			changeTypeCamera = true;
		}

		switch (_cameras.cameras[indexCamera].rotationType ) {
		case CameraTypeClass.TipoRotac.ETS_StyleCamera:
			rotationXETS += mouseXInput * sensitivity;
			rotationYETS += mouseYInput * sensitivity;
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

	Transform temp_parent;

	public void EnterInVehicle()
	{
		GameObject arrow = GameObject.FindGameObjectWithTag ("Arrow");
		temp_parent = arrow.transform.parent;
		arrow.transform.parent = transform;
		arrow.transform.localPosition = new Vector3 (0, 1, 2);
		isInsideTheCar = true;
		EnableCameras (indexCamera);
	}

	public void ExitTheVehicle()
	{
		if (temp_parent) 
		{
			GameObject arrow = GameObject.FindGameObjectWithTag ("Arrow");
			arrow.transform.parent = temp_parent;
			arrow.transform.localPosition = new Vector3 (0, 0.366118f, 1.048862f);
		}

		isInsideTheCar = false;
		EnableCameras (-1);
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
		
	public void Explode()
	{
		_explosionParticle.Play ();
		AudioSource.PlayClipAtPoint (_explosionAudio, transform.position);
		_heliLight.SetActive (false);
	}

	public void decreaseHealth(float value)
	{
		if (health >= 1 && health != 0.01F)
			health -= value;
	}
}