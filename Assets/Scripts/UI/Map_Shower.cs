using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[System.Serializable]
public class Map 
{
	public string Name;
	public Sprite OverView;
}


public class Map_Shower : MonoBehaviour {

	public GameObject slot;
	public Map[] maps;
	public Button start;
	AsyncOperation aso;
	bool loading;
	int current_map = -1;

	void Start ()
	{
		start.onClick.AddListener (start_play);

		foreach (Map m in maps)
		{
			GameObject go = Instantiate (slot, transform.GetChild(0));
			go.transform.GetChild (0).GetComponent<Image> ().sprite = m.OverView;
			go.transform.GetChild (1).GetComponent<Text> ().text = m.Name;
		}
	}

	void start_play()
	{
		if (current_map!=-1)
		{
			aso =	SceneManager.LoadSceneAsync (maps[current_map].Name);
			loading = true;
		}
	}

	public void Select(int num)
	{
		if (current_map != -1)
			transform.GetChild (0).GetChild (current_map).GetComponent<Image> ().color = Color.cyan;
		current_map = num;
		transform.GetChild (0).GetChild (current_map).GetComponent<Image> ().color = Color.green;

	}
	void Update()
	{
		if (loading) 
		{
			transform.GetChild (0).gameObject.SetActive (false);
			transform.GetChild (2).gameObject.SetActive (false);
			GameObject slider = transform.GetChild (1).gameObject;
			slider.SetActive (true);
			slider.GetComponent<Slider> ().value = aso.progress/0.9f;
			slider.transform.GetChild (2).GetComponent<Text> ().text = Mathf.Ceil(aso.progress / 0.9f *100) + "%";
		}
	}
}
