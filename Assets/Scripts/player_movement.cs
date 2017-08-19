using System.Collections;
using UnityEngine;

public class player_movement : MonoBehaviour 
{
	public float speed = 6f;
	public float rotationSpeed = 0.1f;
	public float jump_height = 1f;
	public float crouchingHeight = 0.4f;

	// Update is called once per frame
	void Update () 
	{
		float Rotation_x = Input.GetAxis ("Mouse X") * rotationSpeed;
		transform.Rotate (Vector3.up, Rotation_x); 

		float x_movement = -1 * speed * Input.GetAxisRaw ("Vertical") * Time.deltaTime;
		float z_movement = speed * Input.GetAxisRaw ("Horizontal") * Time.deltaTime;
		transform.Translate (x_movement, 0f, z_movement); //for passing through object

		//code for crouching and jumping(well, maybe not jumping)
		if (Input.GetKeyDown (KeyCode.C)) {
			speed = 2f;
			crouchingHeight = crouchingHeight * -1f;
			transform.Translate (0f, crouchingHeight, 0f); 
		} 
		else {
			speed = 5f;
		}

		/*
		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			float i = 0f;
			for (i = 0f; i < jump_height; i += 0.025f) // as the increment value decreases the jumping height will decrease, 0.05 & 0.025 are the best
			{
				transform.Translate (0f, i * Time.deltaTime, 0f);
			}
		}
		if (Input.GetKeyDown (KeyCode.K)) 
		{
			
		}*/
	}
}
