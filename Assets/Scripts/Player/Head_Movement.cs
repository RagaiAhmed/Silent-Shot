using UnityEngine;

public class Head_Movement : MonoBehaviour 
{
	public Vector3 new_rotation; // holding the new rotation according to mouse input

	public float Sensitivity; // mouse sensetivity

	public int[] root_path;
	public int[] chest_path;
	public int[] torso_path ;
	public int[] head_path;

	 Transform root;
	 Transform chest;
	 Transform torso ;
	 Transform head;

	public float yshift ;
	private float temp_x;
	private float temp_y;
	public float side_shift;



	Animator anim;


	void Start()
	{
		root = get_from_path (root_path);
		chest = get_from_path (chest_path);
		torso = get_from_path (torso_path);
		head = get_from_path (head_path);

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false; // hides mouse
		anim=GetComponent<Animator>(); 
	}

	void Update()
	{
		if (Time.timeScale > 0)
		{
			temp_x *= 0.5f;
			temp_y *= 0.5f;

			new_rotation += new Vector3 (temp_x,temp_y);

			float turn=-Input.GetAxis ("Mouse Y");

			if (turn > 0) 
				anim.SetBool ("Turn_R",true);
			else 
				anim.SetBool ("Turn_R",false);
			if (turn < 0)
				anim.SetBool ("Turn_L",true);
			else
				anim.SetBool ("Turn_L",false);

			new_rotation += new Vector3( turn, Input.GetAxis( "Mouse X" ), 0)*Sensitivity ;	// adds the input to the current looking state

		}
	}

	void LateUpdate () 
	{
			new_rotation        = new Vector3( Mathf.Clamp( new_rotation.x, -90, 60), new_rotation.y%360, 0); // clamps it
			transform.rotation  = Quaternion.Euler( 0, new_rotation.y, 0); // applying portion of rotation to body
			root.localRotation  = Quaternion.Euler(0,yshift,0);
			torso.localRotation = Quaternion.Euler( new_rotation.x/4*3 , -yshift+side_shift/2, 0); // applying portion of rotation to torso
			chest.localRotation = Quaternion.Euler( Mathf.Abs( new_rotation.x/4 ),side_shift/2, 0 ); // applying portion of rotation to chest
			head.localRotation  = Quaternion.Euler( 0, -side_shift, 0 ); 
			head.rotation = Quaternion.Euler(head.eulerAngles.x , head.eulerAngles.y , 0 ); ;
		
	}
	public float apply_x(float val)
	{
		temp_x += val;
		if (temp_x + new_rotation.x < -60) 
		{
			return -60 - new_rotation.x;
		}
		return temp_x;
	}

	public void apply_y(float val)
	{
		temp_y += val;
	}

	Transform get_from_path(int[] path)
	{
		Transform t = transform;
		foreach (int i in path)
		{
			t = t.GetChild (i);
		}
		return t;
	}
		
}
