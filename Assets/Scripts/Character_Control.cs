using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Control : MonoBehaviour 
{

	CharacterController engine0 ; 
	public float speed; // adjustable speed variable
	public float gravity; // adjustable gravity quantity
	public float jump; // adjustable jump force

	private bool wasinair;

	void Start ()
	{
		engine0 = gameObject.GetComponent<CharacterController> (); // gets the charcater controller
	}
	
	void Update () 
	{
		input (); // takes input 
	}

	void input()
	{
		// MOVEMENT
		move();

		// TODO Switch weapon

		// TODO Escape pressed
	}
	void move()
	{
		Vector3 Movement ; // stores main movement input
		Movement=transform.right * Input.GetAxis ("Horizontal");  
		Movement+=transform.forward * Input.GetAxis ("Vertical");

		if (Movement.magnitude > 0)
		{
			//TODO trigger movement animation **Stage 2
			//TODO Trigger moving sound effect  **Stage 2
		}

		Movement.Normalize (); // normalized to make walking diagonaly like walking straight without increasing speed
		Movement *= speed*(Mathf.Pow(2,Input.GetAxis ("Sprint"))) ; // taking care of speed and with respect to sprint and walk

		float up = engine0.velocity.y - gravity; // the up speed is decreased by a constant value making gravity

		if (engine0.isGrounded) 
		{
			if (Input.GetAxisRaw ("Jump") == 1) {
				up += jump; // if grounded and jump pressed add some speed to up value
				wasinair=true;
				// TODO Trigger jumping sound effect **Stage 2
				// TODO trigger in air animation **Stage 2
			} 
			else if (wasinair) 
			{
				wasinair = false;
				// TODO stop animation in air **Stage 2
				// TODO landing sound effect **Stage 2
			}



		} 

		engine0.Move(new Vector3(Movement.x,up,Movement.z)*Time.deltaTime); //applying the movement
	}

}
