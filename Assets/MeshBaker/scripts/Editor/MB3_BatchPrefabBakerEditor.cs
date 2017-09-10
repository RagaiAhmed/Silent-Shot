using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DigitalOpus.MB.Core;
using System.Text.RegularExpressions;

[CustomEditor(typeof(MB3_BatchPrefabBaker))]
public class MB3_BatchPrefabBakerEditor : Editor {
	
	public class UnityTransform{
		public Vector3 p;
		public Quaternion q;
		public Vector3 s;
		public Transform t;
		
		public UnityTransform(Transform t){
			this.t  = t;
			p = t.localPosition;
			q = t.localRotation;
			s = t.localScale;
		}
	}

	SerializedObject prefabBaker=null;

    SerializedProperty prefabRows, outputFolder;

	Color buttonColor = new Color(.8f, .8f, 1f, 1f);

	[MenuItem("GameObject/Create Other/Mesh Baker/Batch Prefab Baker",false,1000)]
	public static void CreateNewBatchPrefabBaker(){
		if (MB3_MeshCombiner.EVAL_VERSION){
			Debug.LogError("The prefab baker is only available in the full version of MeshBaker.");
			return;
		}
		MB3_TextureBaker[] mbs = (MB3_TextureBaker[]) Editor.FindObjectsOfType(typeof(MB3_TextureBaker));
		Regex regex = new Regex(@"\((\d+)\)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		int largest = 0;
		try{
			for (int i = 0; i < mbs.Length; i++){
				Match match = regex.Match(mbs[i].name);
				if (match.Success){
					int val = Convert.ToInt32(match.Groups[1].Value);
					if (val >= largest)
						largest = val + 1;
				}
			}
		} catch(Exception e){
			if (e == null) e = null; //Do nothing supress compiler warning
		}
		GameObject nmb = new GameObject("BatchPrefabBaker (" + largest + ")");
		nmb.transform.position = Vector3.zero;
		
		nmb.AddComponent<MB3_BatchPrefabBaker>();
		nmb.AddComponent<MB3_TextureBaker>();
		nmb.AddComponent<MB3_MeshBaker>();
	}	
	
	void OnEnable() {
		prefabBaker = new SerializedObject(target);
        prefabRows = prefabBaker.FindProperty("prefabRows");
        outputFolder = prefabBaker.FindProperty("outputPrefabFolder");
	}

	void OnDisable() {
		prefabBaker = null;
	}
	
	public override void OnInspectorGUI(){
		prefabBaker.Update();
		
		EditorGUILayout.HelpBox(
								"This tool speeds up the process of preparing prefabs " +
								" for static and dynamic batching. It creates duplicate prefab assets and meshes " +
								"that share a combined material. Source assets are not touched.\n\n" +
								"1) bake the textures to be used by prefabs using the MB3_TextureBaker attached to this game object\n" +
								"2) enter the number of prefabs to bake in the 'Prefab Rows Size' field\n" +
								"3) drag source prefab assets to the 'Source Prefab' slots. These should be project assets not scene objects. Renderers" +
								" do not need to be in the root of the prefab. There can be more than one" +
								" renderer in each prefab.\n" +
								"4) choose a folder where the result prefabs will be stored and click 'Create Empty Result Prefabs'\n" +
								"5) click 'Batch Bake Prefabs'\n" +
								"6) Check the console for messages and errors",MessageType.Info);

        EditorGUILayout.PropertyField(prefabRows, true);

        EditorGUILayout.LabelField("Output Folder", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(outputFolder.stringValue);

        if (GUILayout.Button("Browse For Output Folder"))
        {
            string path = EditorUtility.OpenFolderPanel("Browse For Output Folder", "", "");
            outputFolder.stringValue = path;
        }

        if (GUILayout.Button("Create Empty Result Prefabs"))
        {
            _createEmptyOutputPrefabs();
        }

		Color oldColor = GUI.backgroundColor;
		GUI.backgroundColor = buttonColor;
        if (GUILayout.Button("Batch Bake Prefabs"))
        {
            _bakePrefabs();
        }
		GUI.backgroundColor = oldColor;

        if (GUILayout.Button("Poplate Prefab Rows From Texture Baker"))
        {
            _populatePrefabRowsFromTextureBaker((MB3_BatchPrefabBaker) prefabBaker.targetObject);
        }
        prefabBaker.ApplyModifiedProperties();		
		prefabBaker.SetIsDifferentCacheDirty();	
	}
	
    public void _populatePrefabRowsFromTextureBaker(MB3_BatchPrefabBaker prefabBaker)
    {
        Undo.RecordObject(prefabBaker, "Populate prefab rows");
        MB3_TextureBaker texBaker = prefabBaker.GetComponent<MB3_TextureBaker>();
        List<GameObject> prefabs = new List<GameObject>();
        List<GameObject> gos = texBaker.GetObjectsToCombine();
        for (int i = 0; i < gos.Count; i++)
        {
            GameObject go = (GameObject) PrefabUtility.FindPrefabRoot(gos[i]);
            UnityEngine.Object obj = PrefabUtility.GetPrefabParent(go);
            
            if (obj != null && obj is GameObject)
            {
                if (!prefabs.Contains((GameObject)obj)) prefabs.Add((GameObject)obj);
            } else
            {
                Debug.LogWarning(String.Format("Object {0} did not have a prefab", gos[i]));
            }
           
        }
        List<MB3_BatchPrefabBaker.MB3_PrefabBakerRow> newRows = new List<MB3_BatchPrefabBaker.MB3_PrefabBakerRow>();
        for (int i = 0; i < prefabs.Count; i++)
        {
            MB3_BatchPrefabBaker.MB3_PrefabBakerRow row = new MB3_BatchPrefabBaker.MB3_PrefabBakerRow();
            row.sourcePrefab = prefabs[i];
            newRows.Add(row);
        }
        prefabBaker.prefabRows = newRows.ToArray();
    }


    public void _bakePrefabs(){
		Debug.Log("Batch baking prefabs");
		if (Application.isPlaying) {
			Debug.LogError ("The BatchPrefabBaker cannot be run in play mode.");
			return;
		}
		MB3_BatchPrefabBaker pb = (MB3_BatchPrefabBaker) target;
		MB3_MeshBaker mb = pb.GetComponent<MB3_MeshBaker>();
		if (mb == null){
			Debug.LogError("Prefab baker needs to be attached to a Game Object with a MB3_MeshBaker component.");
			return;
		}
		
		if (mb.textureBakeResults == null){
			Debug.LogError("Texture Bake Results is not set");
			return;
		}
		
		if (mb.meshCombiner.outputOption != MB2_OutputOptions.bakeMeshAssetsInPlace){
            mb.meshCombiner.outputOption = MB2_OutputOptions.bakeMeshAssetsInPlace;		
		}

		MB2_TextureBakeResults tbr = mb.textureBakeResults;
		
		HashSet<Mesh> sourceMeshes = new HashSet<Mesh>();
		HashSet<Mesh> allResultMeshes = new HashSet<Mesh>();
		
		//validate prefabs
		for (int i = 0; i < pb.prefabRows.Length; i++){
			if (pb.prefabRows[i] == null || pb.prefabRows[i].sourcePrefab == null){
				Debug.LogError("Source Prefab on row " + i + " is not set.");
				return;
			}
			if (pb.prefabRows[i].resultPrefab == null){
				Debug.LogError("Result Prefab on row " + i + " is not set.");
				return;
			}
			for (int j = i+1; j < pb.prefabRows.Length; j++){
				if (pb.prefabRows[i].sourcePrefab == pb.prefabRows[j].sourcePrefab){
					Debug.LogError("Rows " + i + " and " + j + " contain the same source prefab");
					return;
				}
			}
			for (int j = 0; j < pb.prefabRows.Length; j++){
				if (pb.prefabRows[i].sourcePrefab == pb.prefabRows[j].resultPrefab){
					Debug.LogError("Row " + i + " source prefab is the same as row " + j + " result prefab");
					return;
				}
			}
			if (PrefabUtility.GetPrefabType(pb.prefabRows[i].sourcePrefab) != PrefabType.ModelPrefab &&
				PrefabUtility.GetPrefabType(pb.prefabRows[i].sourcePrefab) != PrefabType.Prefab){
				Debug.LogError ("Row " + i + " source prefab is not a prefab asset");
				return;
			}
			if (PrefabUtility.GetPrefabType(pb.prefabRows[i].resultPrefab) != PrefabType.ModelPrefab &&
				PrefabUtility.GetPrefabType(pb.prefabRows[i].resultPrefab) != PrefabType.Prefab){
				Debug.LogError ("Row " + i + " result prefab is not a prefab asset");
				return;
			}
			
			GameObject so = (GameObject) Instantiate(pb.prefabRows[i].sourcePrefab);
			GameObject ro = (GameObject) Instantiate(pb.prefabRows[i].resultPrefab);
			Renderer[] rs = (Renderer[]) so.GetComponentsInChildren<Renderer>();
			
			for (int j = 0; j < rs.Length; j++){
				if (IsGoodToBake(rs[j],tbr)){
					sourceMeshes.Add (MB_Utility.GetMesh(rs[j].gameObject));	
				}
			}
			rs = (Renderer[]) ro.GetComponentsInChildren<Renderer>();
			
			for (int j = 0; j < rs.Length; j++){
				Renderer r = rs[j];
				if (r is MeshRenderer || r is SkinnedMeshRenderer){
					Mesh m = MB_Utility.GetMesh(r.gameObject);
					if (m != null){
						allResultMeshes.Add (m);	
					}
				}
			}
			DestroyImmediate(so); //todo should cache these and have a proper cleanup at end
			DestroyImmediate(ro);
		}
		
		sourceMeshes.IntersectWith(allResultMeshes);
        HashSet<Mesh> sourceMeshesThatAreUsedByResult = sourceMeshes;
		if (sourceMeshesThatAreUsedByResult.Count > 0){
			foreach(Mesh m in sourceMeshesThatAreUsedByResult)
            {
				Debug.LogWarning("Mesh " + m + " is used by both the source and result prefabs. New meshes will be created.");	
			}
			//return;
		}
		
		Dictionary<string,string> createdMeshPaths = new Dictionary<string, string>();
		// Bake the meshes using the meshBaker component one prefab at a time
		for (int prefabIdx = 0; prefabIdx < pb.prefabRows.Length; prefabIdx++){
			Debug.Log ("==== Processing Source Prefab " + pb.prefabRows[prefabIdx].sourcePrefab);
			GameObject sceneObj = (GameObject) Instantiate(pb.prefabRows[prefabIdx].sourcePrefab);
			GameObject resultPrefab = (GameObject) Instantiate(pb.prefabRows[prefabIdx].resultPrefab);

			Renderer[] rs = sceneObj.GetComponentsInChildren<Renderer>();
			if (rs.Length < 1){
				Debug.LogWarning("Prefab " + prefabIdx + " does not have a renderer");
				DestroyImmediate(sceneObj);
				DestroyImmediate(resultPrefab);
				continue;
			}

			List<Mesh> usedMeshes = new List<Mesh>();
			List<UnityTransform> unityTransforms = new List<UnityTransform>();
			for (int j = 0; j < rs.Length; j ++){
				unityTransforms.Clear ();
				Renderer r = rs[j];
				
				if (!IsGoodToBake(r,tbr)){
					continue;
				}
				
				//find the corresponding mesh in the result prefab
				string resultFolderPath = AssetDatabase.GetAssetPath(pb.prefabRows[prefabIdx].resultPrefab);
				resultFolderPath = Path.GetDirectoryName(resultFolderPath);
				
				Mesh m = null;
				Transform tRes = FindCorrespondingTransform(sceneObj.transform,r.transform, resultPrefab.transform);
				if (tRes != null) m = MB_Utility.GetMesh(tRes.gameObject);
				
                //if the meshes on source and result are the same we want to remove mesh so will create a new one
                if (sourceMeshesThatAreUsedByResult.Contains(m))
                {
                    Debug.LogWarning("Source and result prefabs share a mesh. Creating a new mesh for " + m);
                    MB_Utility.SetMesh(tRes.gameObject, null);
                    m = null;
                }


				string meshPath;
				//check that the mesh is an asset and that we have not used it already
				if (m != null && AssetDatabase.IsMainAsset(m.GetInstanceID()) && !usedMeshes.Contains(m)){
					meshPath = AssetDatabase.GetAssetPath(m);
					if (createdMeshPaths.ContainsKey(meshPath)){
						Debug.LogWarning("Different result prefabs share a mesh." + meshPath);	
					}
				} else { //create a new mesh asset with a unique name
					string resultPrefabFilename = AssetDatabase.GetAssetPath(pb.prefabRows[prefabIdx].resultPrefab);
					resultPrefabFilename = resultPrefabFilename.Substring(0,resultPrefabFilename.Length - ".prefab".Length) + ".asset";
					meshPath = AssetDatabase.GenerateUniqueAssetPath(resultPrefabFilename);
					m = new Mesh();
					AssetDatabase.CreateAsset(m,meshPath);
					m = (Mesh) AssetDatabase.LoadAssetAtPath(meshPath,typeof(Mesh));
				}
				Debug.Log ("  creating new mesh asset at path " + meshPath);				
				if (!createdMeshPaths.ContainsKey(meshPath)) createdMeshPaths.Add (meshPath,meshPath);

				// position rotation and scale are baked into combined mesh.
				// Remember all the transforms settings then
				// record transform values to root of hierarchy
				Transform t = r.transform;
				if (t != t.root){
					do {
						unityTransforms.Add (new UnityTransform(t));
						t = t.parent;
					} while (t != null && t != t.root);
				}
				//add the root
				unityTransforms.Add (new UnityTransform(t.root));
				
				//position at identity
				for (int k = 0; k < unityTransforms.Count; k++){
					unityTransforms[k].t.localPosition = Vector3.zero;
					unityTransforms[k].t.localRotation = Quaternion.identity;
					unityTransforms[k].t.localScale = Vector3.one;
				}

                //throw new Exception("");
                //bake the mesh
                mb.ClearMesh();
                MB3_MeshCombiner mc = mb.meshCombiner;
				m = MB3_BakeInPlace.BakeOneMesh((MB3_MeshCombinerSingle) mc,meshPath,r.gameObject);
				
				//replace the mesh
				if (r is MeshRenderer){
					MeshFilter mf = r.gameObject.GetComponent<MeshFilter>();
					mf.sharedMesh = m;
				} else { //skinned mesh
					SkinnedMeshRenderer smr = r.gameObject.GetComponent<SkinnedMeshRenderer>();
					smr.sharedMesh = m;
					smr.bones = ((SkinnedMeshRenderer)mc.targetRenderer).bones;
				}
				
				//replace the result material(s)
				if (mb.textureBakeResults.doMultiMaterial){
					Material[] rss = new Material[mb.textureBakeResults.resultMaterials.Length];
					for (int k = 0; k < rss.Length; k++){
						rss[k] = mb.textureBakeResults.resultMaterials[k].combinedMaterial;	
					}
					r.sharedMaterials = rss;
				} else {
                    Material[] originalMats = r.sharedMaterials;
                    Material[] rss = new Material[originalMats.Length];
                    for (int k = 0; k < originalMats.Length; k++)
                    {
                        if (tbr.ContainsMaterial(originalMats[k]))
                        {
                            rss[k] = mb.textureBakeResults.resultMaterial;
                        } else
                        {
                            rss[k] = originalMats[k];
                        }
                    }
                    
                    r.sharedMaterials = rss;
                }
				
				//restore the transforms
				for (int k = 0; k < unityTransforms.Count; k++){
					unityTransforms[k].t.localPosition = unityTransforms[k].p;
					unityTransforms[k].t.localRotation = unityTransforms[k].q;
					unityTransforms[k].t.localScale = unityTransforms[k].s;
				}				
			}
			
			//replace the result prefab with the source object
			//duplicate the sceneObj so we can replace the clone into the prefab, not the source
			GameObject clone = (GameObject) Instantiate(sceneObj);
			PrefabUtility.ReplacePrefab(clone,pb.prefabRows[prefabIdx].resultPrefab,ReplacePrefabOptions.ReplaceNameBased);		
			DestroyImmediate(clone);
			DestroyImmediate(sceneObj);
			DestroyImmediate(resultPrefab);
		}
		AssetDatabase.Refresh();
		mb.ClearMesh();
	}

	bool IsGoodToBake(Renderer r, MB2_TextureBakeResults tbr){
		if (r == null) return false;
		if (!(r is MeshRenderer) && !(r is SkinnedMeshRenderer)){
			return false;	
		}
		Material[] mats = r.sharedMaterials;
		for (int i = 0; i < mats.Length; i++){
			if (!tbr.ContainsMaterial(mats[i])){
				Debug.LogWarning("Mesh on " + r + " uses a material " + mats[i] + " that is not in the list of materials. This mesh will not be baked. The original mesh and material will be used in the result prefab.");
				//todo assign the source assets to the result
				return false;
			}
		}
		if (MB_Utility.GetMesh(r.gameObject) == null){
			return false;
		}
		return true;
	}
	
	Transform FindCorrespondingTransform(Transform srcRoot, Transform srcChild,
	                                     Transform targRoot){
		if (srcRoot == srcChild) return targRoot;
		
//		Debug.Log ("start ============");
		//build the path to the root in the source prefab
		List<Transform> path_root2child = new List<Transform>();
		Transform t = srcChild;
		do {
			path_root2child.Insert (0,t);
			t = t.parent;
		} while (t != null && t != t.root && t != srcRoot);
		if (t == null){
			Debug.LogError ("scrChild was not child of srcRoot " + srcRoot + " " + srcChild);
			return null;
		}
		path_root2child.Insert (0, srcRoot);
//		Debug.Log ("path to root for " + srcChild + " " + path_root2child.Count);
		 
		//try to find a matching path in the target prefab
		t = targRoot;
		for (int i = 1; i < path_root2child.Count; i++){
			Transform tSrc = path_root2child[i - 1];
			//try to find child in same position with same name
			int srcIdx = TIndexOf(tSrc,path_root2child[i]);
			if (srcIdx < t.childCount && path_root2child[i].name.Equals(t.GetChild (srcIdx).name)){
				t = t.GetChild (srcIdx);
//				Debug.Log ("found child in same position with same name " + t);
				continue;
			}
			//try to find child with same name
			for (int j = 0; j < t.childCount; j++){
				if (t.GetChild (j).name.Equals(path_root2child[i].name)){
					t = t.GetChild (j);
//					Debug.Log ("found child with same name " + t);
					continue;
				}
			}
			t = null;
			break;
		}
//		Debug.Log ("end =============== " + t);
		return t;
	}

	int TIndexOf(Transform p, Transform c){
		for (int i = 0; i < p.childCount; i++){
			if (c == p.GetChild (i)){
				return i;
			}
		}
		return -1;
	}

    void _createEmptyOutputPrefabs()
    {
        if (outputFolder.stringValue == null)
        {
            Debug.LogError("Output folder must be set");
            return;
        }
        if (outputFolder.stringValue.StartsWith(Application.dataPath))
        {
            string relativePath = "Assets" + outputFolder.stringValue.Substring(Application.dataPath.Length);
            string gid = AssetDatabase.AssetPathToGUID(relativePath);
            if (gid == null)
            {
                Debug.LogError("Output folder must be a folder in the Unity project Asset folder");
                return;
            }
        }
        else {
            Debug.LogError("Output folder must be a folder in the Unity project Asset folder");
            return;
        }

        int numCreated = 0;
        int numSkippedSrcNull = 0;
        int numSkippedAlreadyExisted = 0;
        MB3_BatchPrefabBaker prefabBaker = (MB3_BatchPrefabBaker)target;
        for (int i = 0; i < prefabBaker.prefabRows.Length; i++)
        {
            if (prefabBaker.prefabRows[i].sourcePrefab != null)
            {
                if (prefabBaker.prefabRows[i].resultPrefab == null) {
                    string outName = outputFolder.stringValue + "/" + prefabBaker.prefabRows[i].sourcePrefab.name + ".prefab";
                    outName = outName.Replace(Application.dataPath, "");
                    outName = "Assets" + outName;
                    //GameObject go = PrefabUtility.CreatePrefab(outputFolder.stringValue + "/" + outName + ".prefab", new GameObject(outName));
                    GameObject go = new GameObject(prefabBaker.prefabRows[i].sourcePrefab.name);
                    prefabBaker.prefabRows[i].resultPrefab = PrefabUtility.CreatePrefab(outName, go);
                    DestroyImmediate(go);
                    numCreated++;
                } else
                {
                    numSkippedAlreadyExisted++;
                }
            } else
            {
                numSkippedSrcNull++;
            }
        }
        Debug.Log(String.Format("Created {0} prefabs. Skipped {1} because source prefab was null. Skipped {2} because the result prefab was already assigned", numCreated, numSkippedSrcNull, numSkippedAlreadyExisted));
    }
}
