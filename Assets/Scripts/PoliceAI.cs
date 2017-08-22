using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class PoliceAI : MonoBehaviour {

	public GameObject policeMan;

	[Tooltip("The ParticleSystem to be used for explosion events.")]
	public ParticleSystem _explosionParticle;

	[Tooltip("The sound effect to be used for explosion events.")]
	public AudioClip _explosionAudio;

	[Space(5)][Tooltip("Collision sounds should be associated with this list.")]
	public AudioClip[] collisionSounds;

	[Tooltip("The lights of the car.")]
	public GameObject _carLight;

	private CarAIControl aiControl;

	public float health = 50;

	public AudioSource siren;

	private bool temp = false;

	private int bulletholes = 0;

	Rigidbody rb;

	// Use this for initialization
	void Start () {
		policeMan.SetActive (false);
		aiControl = GetComponent<CarAIControl> ();
	}

	public void Explode()
	{
		rb = GetComponent<Rigidbody> ();
		_explosionParticle.Play ();
		_carLight.SetActive (false);
		AudioSource.PlayClipAtPoint (_explosionAudio, transform.position);
		health = -0.01f;
		siren.Stop ();
	}

	// Update is called once per frame
	void Update () {
		if (aiControl.m_CarController.CurrentSpeed<1) {
			policeMan.SetActive (true);
		}
		if (health < 1&&health!=-0.01f) 
		{
			Explode ();
			aiControl.m_Driving = false;
		}
		Vector3 localTarget = policeMan.transform.InverseTransformPoint(aiControl.m_Target.position);
		// if appropriate, stop driving when we're close enough to the target.
		if (localTarget.magnitude < aiControl.m_ReachTargetThreshold)
		{
			aiControl.m_CarController.Move (0, 0, -100000000, 0);
			aiControl.enabled = false;
		}
		if (localTarget.magnitude > 3*aiControl.m_ReachTargetThreshold&&policeMan.activeSelf&&health!=-0.01f) {
			aiControl.enabled = true;
			policeMan.SetActive (false);
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

	void OnCollisionEnter (Collision collision){
		if (collision.contacts.Length > 0) {
			if (collision.relativeVelocity.magnitude > 5 && collision.contacts [0].thisCollider.gameObject.transform != transform.parent) {
				if (collisionSounds.Length > 0) {
							AudioSource.PlayClipAtPoint(collisionSounds [UnityEngine.Random.Range (0, collisionSounds.Length)],transform.position);
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
}
