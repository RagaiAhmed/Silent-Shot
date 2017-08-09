using UnityEngine;

public class Head_Movement : MonoBehaviour 
{
	public Vector3 new_rotation; // holding the new rotation according to mouse input

	public float Sensitivity; // mouse sensetivity

	public Transform chest;
	public Transform torso;
	public Transform root;
	public float torso_shift;
	public float root_shift;

	Animator anim;


	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false; // hides mouse
		anim=GetComponent<Animator>(); 
	}


	void LateUpdate () 
	{
		if (Time.timeScale > 0)
		{
			// adds the input to the current looking state
			float turn=-Input.GetAxis ("Mouse Y");
			if (turn > 0) 
				anim.SetBool ("Turn_R",true);
			else 
				anim.SetBool ("Turn_R",false);
			if (turn < 0)
				anim.SetBool ("Turn_L",true);
			else
				anim.SetBool ("Turn_L",false);

			new_rotation       += new Vector3( turn, Input.GetAxis( "Mouse X" ), 0)*Sensitivity ; // takes input
			new_rotation        = new Vector3( Mathf.Clamp( new_rotation.x, -60, 60), new_rotation.y%360, 0); // clamps it
			transform.rotation  = Quaternion.Euler( 0, new_rotation.y-torso_shift, 0); // applying portion of rotation to body
			root .localRotation = Quaternion.Euler( root_shift, torso_shift, 0); // maintain torso shift
			torso.localRotation = Quaternion.Euler( new_rotation.x/4*3 , -torso_shift, 0); // applying portion of rotation to torso
			chest.localRotation = Quaternion.Euler( Mathf.Abs( new_rotation.x/4 ), 0, 0 ); // applying portion of rotation to chest
		}
	}
}
