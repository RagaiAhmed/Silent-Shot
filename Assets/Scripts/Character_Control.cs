using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Control : MonoBehaviour 
{

	CharacterController engine0 ; 
	public float speed; // adjustable speed variable
	public float gravity; // adjustable gravity quantity
	public float jump; // adjustable jump forcce

	void Start ()
	{
		engine0 = gameObject.GetComponent<CharacterController> (); // gets the charcater controller
	}
	
	void FixedUpdate () 
	{
		input (); // takes input of movement
		animations (); // takes care of animation
		sound_effects (); // moving sound effect and such
	}

	void input()
	{
		Vector3 Movement = 
			transform.right * Input.GetAxis ("Horizontal")  // directional vector multiplied by a quantity (input)
			+transform.forward * Input.GetAxis ("Vertical");

		Movement.Normalize (); // normalized to make walking diagonaly like walking straight without increasing speed
		Movement *= speed*(1.5f+Input.GetAxis ("Sprint")*0.5f) ; // taking care of speed and with respect to sprint and walk

		float up = engine0.velocity.y - gravity; // the up speed is decreased by a constant value making gravity

		if (engine0.isGrounded) 
		{
			if (Input.GetAxisRaw ("Jump") == 1){up += jump;} // if grounded and jump pressed add some speed to up value
		}

		engine0.Move(new Vector3(Movement.x,up,Movement.z)*Time.deltaTime); //applying the movement

	}

	void animations()
	{
		// Trigger animations
	}

	void sound_effects()
	{
		//Trigger sound effects
	}
}
