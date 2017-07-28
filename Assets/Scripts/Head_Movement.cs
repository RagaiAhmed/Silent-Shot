using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head_Movement : MonoBehaviour 
{
	public Vector3 new_rotation; // holding the new rotation according to mouse input

	public float Sensitivity; // mouse sensetivity
	Transform body; // player body

	void Start()
	{
		Cursor.visible = false; // hides mouse
		Cursor.lockState = CursorLockMode.Locked; // locks mouse in screen
		body = transform.parent; // body of the player
			
	}


	void Update () 
	{
		// adds the input to the current looking state
		new_rotation +=  new Vector3(-Input.GetAxis ("Mouse Y"),Input.GetAxis("Mouse X"),0)*Sensitivity ;
		// handles the limits angles of looking
		new_rotation=new Vector3(Mathf.Clamp(new_rotation.x,-60,60),new_rotation.y%360,0);
		// casts the vector to a rotation dedicated type called quaternion
		transform.localRotation = Quaternion.Euler(new_rotation.x,0,0); // applying up , down rotation to head
		body.rotation = Quaternion.Euler(0,new_rotation.y,0); // applying right , left rotation to body
		
	}
}
