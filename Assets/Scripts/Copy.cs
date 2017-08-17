using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Copy : MonoBehaviour {

	public GameObject frm;
	public GameObject to;
	public bool start;

	void Update () 
	{
		if (start)
		{
			start = false;
			copy (frm.transform,to.transform);
		}
		
	}
	void copy(Transform frm ,Transform to)
	{
		Component[] taken = frm.GetComponents (typeof(Component));
		foreach(Component cmp in taken)
		{
			if (cmp is Transform || cmp is SkinnedMeshRenderer || cmp is MeshFilter)
				continue;
			CopyComponent (cmp, to.gameObject);
		}
		for (int i = 0; i < frm.childCount; i++)
		{
			copy (frm.GetChild(i),to.GetChild(i));
		}
	}
	Component CopyComponent(Component original, GameObject destination)
	{
		System.Type type = original.GetType();
		Component copy = destination.AddComponent(type);
		System.Reflection.FieldInfo[] fields = type.GetFields(); 
		foreach (System.Reflection.FieldInfo field in fields)
		{
			field.SetValue(copy, field.GetValue(original));
		}
		return copy;
	}
}
