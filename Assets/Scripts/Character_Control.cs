using System.Collections;
using UnityEngine;

public class Character_Control : MonoBehaviour 
{

	Rigidbody engine0 ; 

	public float crouch; // adjustable speed variable
	public float walk; // adjustable speed variable
	public float run; // adjustable speed variable
	public float jump; // adjustable jump force
	public float footstepadjust;
	public Transform Rightfoot;
	public Transform Leftfoot;


	public AudioClip Jump;
	public AudioClip Land;
	public AudioClip[] FootSteps;

	int state;
	bool was_crouch;
	bool was_idle;
	bool wasinair=true;
	bool step;
	bool isGrounded;

	private Animator anim;
	private AudioSource Audio;
	private float[] speed_array;
	delegate void action ();
	private action[] state_functions;
	void Start ()
	{
		speed_array = new  float[] {crouch,walk,run};
		state_functions = new action[] {Crouch,Walk,Sprint};
		engine0 = GetComponent<Rigidbody>(); 
		anim    = GetComponent<Animator>();
		Audio   = GetComponent<AudioSource>();
	}


	void Update()
	{
		if (Time.timeScale > 0) 
		{
			isGrounded = isgrounded();
			state = (int)Input.GetAxisRaw ("Sprint") + 1;
			Move (); // calls the move function
		}
	}


	void Move()
	{
		Vector3 Movement=Vector3.zero ; // to store main movement input

		Movement.x = Input.GetAxis ("Horizontal");
		Movement.z = Input.GetAxis ("Vertical")  ;  // stores input

		animate_direction (Movement);  // sends the animator the direction of movement

		Movement = Movement.x * transform.right + Movement.z * transform.forward; // stores the direction of movement
		Movement.Normalize (); // normalizes the vector


		Movement *= getSpeed ();

		Movement.y = engine0.velocity.y; // stores the y axis velocity

		if (isGrounded)  // if the character is grounded
		{
			if (wasinair&& going_down()) // if it was previously in air
			{
				wasinair = false; // its now not in air
				anim.SetTrigger("Idle");
				was_idle = true;
				Audio.PlayOneShot(Land);  // playing land Audio clip
			}
			else if (Input.GetAxisRaw ("Jump") == 1 && !wasinair) 
			{
				Movement.y += jump; // adds velocity in y axis
				anim.SetTrigger("InAir");
				wasinair = true;
				Audio.PlayOneShot(Jump); // plays jump audio clip
			}


			engine0.velocity=Movement; // applying the movement
		} 

	}


	void animate_direction (Vector3 Movement)
	{
		// checks the direction of the movement and sets it to the animator

		action current_state = check_state(); // checks state of the character like sprint, crouch, etc..
		current_state();

		anim.SetBool ("Right"   ,Movement.x > 0);
		anim.SetBool ("Left"    ,Movement.x < 0);
		anim.SetBool ("Forward" ,Movement.z > 0);
		anim.SetBool ("Backward",Movement.z < 0);


		if (Movement.magnitude > 0) 
		{
			was_idle = false;

			if (!step && isGrounded && state>1) 
			{
				Audio.PlayOneShot (FootSteps [Random.Range (0, FootSteps.Length)]);
				StartCoroutine (step_wait(getSpeed()));
			}

		}
		else 
		{
			if (!was_idle && !wasinair )  // if wasnt previously idle
			{
				anim.SetTrigger ("Idle"); // set the state to idle
				was_idle = true;
			}
		}

		if (!isGrounded && !wasinair) // if moving in y axis
		{
			wasinair=true;
			was_idle = false;
			anim.SetTrigger ("InAir");
		}
	}
		

	IEnumerator step_wait(float time)
	{
		step = true;
		yield return new WaitForSeconds(1/time*footstepadjust);
		step = false;
	}


	float getSpeed()
	{
		return speed_array[state];
	}


	bool isgrounded ()
	{
		return Physics.Raycast (Rightfoot.position,-transform.up,0.5f)||Physics.Raycast (Leftfoot.position,-transform.up,0.5f);
	}


	action check_state()
	{ // checks current state
		return state_functions [state];
	}

	void Crouch()
	{
		if (!was_crouch) 
		{
			anim.SetBool ("Crouch", true);
			anim.SetBool ("Sprint",false);
			anim.SetTrigger ("Idle");
			was_crouch = true;
		}
	}
	void EndCrouch()
	{
		if (was_crouch)
		{				
			anim.SetTrigger ("Idle");
			was_crouch = false;
		}		
	}
	void Sprint()
	{
		EndCrouch ();
		anim.SetBool ("Sprint", true);
		anim.SetBool ("Crouch", false);
	}
	void Walk()
	{
		EndCrouch ();
		anim.SetBool ("Crouch", false);
		anim.SetBool ("Sprint",false) ;
	}
	bool going_down()
	{
		return engine0.velocity.y < 1;
	}
}
