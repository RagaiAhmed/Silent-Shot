using System.Collections;
using UnityEngine;

public class shooting1 : MonoBehaviour {
	public float range = 1000f;
	public float damage = 100f;
	public float rate = 0.25f;
	public float hitforce = 100f;
	public int magazine_size = 35;
	public Transform gunEnd;
	private Camera fpsCam;
	private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);
	private LineRenderer laserLine;
	private float nextFire;
	public bool rayVisible = true;
	public int fullMagazineSize = 35;
	// Use this for initialization
	void Start () {
		laserLine = GetComponent<LineRenderer> ();
		fpsCam = GetComponentInParent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.R)) 
		{
			int i = fullMagazineSize - magazine_size;
			magazine_size += i;
		}
		if (Input.GetButtonDown ("Fire1") && Time.time >= nextFire && magazine_size > 0) //newly added magazine code
		{
			nextFire = Time.time + rate; 
			magazine_size -= 1;
			shoot ();
			StartCoroutine (shotEffect());
			Vector3 rayOrigin = fpsCam.ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, 0f));
			RaycastHit hit;
			laserLine.SetPosition (0, gunEnd.position);
			if (Physics.Raycast (rayOrigin, fpsCam.transform.forward, out hit, range)) 
			{
				laserLine.SetPosition (1, hit.point);

				being_shootable health = hit.collider.GetComponent<being_shootable> ();

				if (health != null) 
				{
					health.damage_object(damage);
				}

				if (hit.rigidbody != null) 
				{
					hit.rigidbody.AddForce (-hit.normal * hitforce);
				}
			} 
			else 
			{
				laserLine.SetPosition (1, rayOrigin + (fpsCam.transform.forward * range));
			}
		}
	}
	void shoot()
	{
		
	}
	private IEnumerator shotEffect()
	{
		if (rayVisible == true) {
			laserLine.enabled = true;  // change bool here if you want to make the ray invisible
			yield return shotDuration;
			laserLine.enabled = false;
		} 
		else 
		{
			laserLine.enabled = false;  // change bool here if you want to make the ray invisible
			yield return shotDuration;
			laserLine.enabled = false;
		}
	}
}
