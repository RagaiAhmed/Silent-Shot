using System.Collections;
using UnityEngine;

public class Destructable : MonoBehaviour {
	public GameObject destroyed;
	public float explosion_multiplier;

	public	void destroy()
	{	
		if(destroyed)
		{
			Instantiate (destroyed, transform.position, transform.rotation); // put the destroyed
			Destroy (gameObject); // delete current
		}
		else
		{
			StartCoroutine(SplitMesh());
		}
	}

	public IEnumerator SplitMesh ()    
	{

		if(GetComponent<MeshFilter>() == null && GetComponent<SkinnedMeshRenderer>() == null) 
		{
			yield break;
		}

		GetComponent<Collider>().enabled = false;

		Mesh M = new Mesh();
		if(GetComponent<MeshFilter>()) 
		{
			M = GetComponent<MeshFilter>().mesh;
		}
		else if(GetComponent<SkinnedMeshRenderer>()) {
			M = GetComponent<SkinnedMeshRenderer>().sharedMesh;
		}

		Material[] materials = new Material[0];
		if(GetComponent<MeshRenderer>()) 
		{
			materials = GetComponent<MeshRenderer>().materials;
		}
		else if(GetComponent<SkinnedMeshRenderer>())
		{
			materials = GetComponent<SkinnedMeshRenderer>().materials;
		}

		Vector3[] verts = M.vertices;
		Vector3[] normals = M.normals;
		Vector2[] uvs = M.uv;
		for (int submesh = 0; submesh < M.subMeshCount; submesh++) 
		{

			int[] indices = M.GetTriangles(submesh);

			for (int i = 0; i < indices.Length; i += 3)    {
				Vector3[] newVerts = new Vector3[3];
				Vector3[] newNormals = new Vector3[3];
				Vector2[] newUvs = new Vector2[3];
				for (int n = 0; n < 3; n++)   
				{
					int index = indices[i + n];
					newVerts[n] = verts[index];
					newUvs[n] = uvs[index];
					newNormals[n] = normals[index];
				}

				Mesh mesh = new Mesh();
				mesh.vertices = newVerts;
				mesh.normals = newNormals;
				mesh.uv = newUvs;

				mesh.triangles = new int[] { 0, 1, 2, 2, 1, 0 };

				GameObject GO = new GameObject("Triangle " + (i / 3));
				GO.transform.parent = transform;
				GO.layer = LayerMask.NameToLayer("Particle");
				GO.transform.position = transform.position;
				GO.transform.rotation = transform.rotation;
				GO.transform.localScale = transform.localScale;

				GO.AddComponent<MeshRenderer>().material = materials[submesh];
				GO.AddComponent<MeshFilter>().mesh = mesh;

				MeshCollider mc = GO.AddComponent<MeshCollider>();
				mc.convex = true;
				mc.inflateMesh = true;
				mc.skinWidth = 0.01f;

				Vector3 explosionPos = new Vector3(transform.position.x + Random.Range(-0.5f, 0.5f), transform.position.y + Random.Range(0f, 0.5f), transform.position.z + Random.Range(-0.5f, 0.5f));
				Rigidbody rb = GO.AddComponent<Rigidbody> ();
				rb.AddExplosionForce(Random.Range(3,5*explosion_multiplier), explosionPos, 5);
				Destroy(GO, 5 + Random.Range(0.0f, 5.0f));
			}
			yield return null;

		}

		GetComponent<Renderer>().enabled = false;

		yield return new WaitForSeconds(10f);

		Destroy(gameObject);

	}
}