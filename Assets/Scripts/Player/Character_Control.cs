
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class Character_Control : NetworkBehaviour 
{

	Rigidbody engine0 ; 

	public float crouch; // adjustable speed variable
	public float walk; // adjustable speed variable
	public float run; // adjustable speed variable
	public float jump; // adjustable jump force
	public float footstepadjust;
	public int[] Rightfoot_path;
	public int[] Leftfoot_path;

	Transform Rightfoot;
	Transform Leftfoot;
	public AudioClip[] floor;
	private int Floor_Type;


	int state;
	bool was_crouch;
	bool was_idle;
	bool wasinair=true;
	bool step;
	bool isGrounded;

	public float fall_damage;

	private Animator anim;
	private Multi_Sound Audio;
	private float[] speed_array;
	delegate void action ();
	private action[] state_functions;
	private Head_Movement player;
	private float air_time;
	private Main_Health health;

	void Start ()
	{
		Rightfoot = transform;
		foreach (int i in Rightfoot_path)
		{
			Rightfoot = Rightfoot.GetChild (i);
		}
		Leftfoot = transform;
		foreach (int i in Leftfoot_path)
		{
			Leftfoot = Leftfoot.GetChild (i);
		}
		speed_array = new  float[] {crouch,walk,run};
		state_functions = new action[] {Crouch,Walk,Sprint};
		engine0 = GetComponent<Rigidbody>(); 
		anim    = GetComponent<Animator>();
		Audio   = GetComponent<Multi_Sound>();
		player  = GetComponent<Head_Movement> ();
		health  = GetComponent<Main_Health> ();
	}


	void Update()
	{
		if (!isLocalPlayer)
			return;
		isGrounded = isgrounded();
		if(locker.isPlaying)	
			state = (int)Input.GetAxisRaw ("Sprint") + 1;
		Move (); // calls the move function
	}


	void Move()
	{
		Vector3 Movement=Vector3.zero ; // to store main movement input
		if (locker.isPlaying)
		{
			Movement.x = Input.GetAxis ("Horizontal");
			Movement.z = Input.GetAxis ("Vertical")  ;  // stores input
		}


		animate_direction (Movement);  // sends the animator the direction of movement

		if (locker.isPlaying)
		{
			Movement = Movement.x * transform.right + Movement.z * transform.forward; // stores the direction of movement
			Movement.Normalize (); // normalizes the vector


			Movement *= getSpeed ();
		}
			Movement.y = engine0.velocity.y; // stores the y axis velocity


		if (isGrounded)  // if the character is grounded
		{
			if (air_time > 1.5) 
			{
				health.RpcDecrease(air_time*fall_damage);
			}
			air_time = 0;
			if (wasinair&& going_down()) // if it was previously in air
			{
				wasinair = false; // its now not in air
				anim.SetTrigger("Idle");
				was_idle = true;
				Audio.Cmdplay("Character_Control","floor",false,Floor_Type*(floor.Length/2));  // playing land Audio clip
			}
			else if (Input.GetAxisRaw ("Jump") == 1 && !wasinair && locker.isPlaying) 
			{
				Movement.y += jump; // adds velocity in y axis
				anim.SetTrigger("InAir");
				wasinair = true;
				Audio.Cmdplay("Character_Control","floor",false,Floor_Type*(floor.Length/2)+1); // plays jump audio clip
			}
				
			engine0.velocity=Movement; // applying the movement
		}
		else
		{
			air_time += Time.deltaTime;
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

		player.yshift = 45 * Movement.x;
		if (Movement.z != 0)
			player.yshift *= Movement.z;

		if (Movement.magnitude > 0) 
		{
			was_idle = false;

			if (!step && isGrounded && state>1) 
			{
				Audio.Cmdplay ("Character_Control","floor",false,Floor_Type*(floor.Length/2)+Random.Range(2,floor.Length/2));
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
		RaycastHit info;
		if ( Physics.Raycast (Rightfoot.position, -transform.up, out info, 0.5f) ) 
		{
			Floor_Type = (info.collider.gameObject.CompareTag ("Grass")) ? 1 : 0; 
			return true;
		}
		else if ( Physics.Raycast (Leftfoot.position, -transform.up, out info, 0.5f) )
		{
			Floor_Type = (info.collider.gameObject.CompareTag ("Grass")) ? 1 : 0; 
			return true;
		}
		return false;
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
