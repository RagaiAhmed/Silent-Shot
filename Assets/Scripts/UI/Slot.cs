
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour,IDragHandler,IPointerUpHandler ,IPointerDownHandler,IDropHandler{

	Vector2 ofset;
	public int number;
	public Transform origin_parent;

	public void OnPointerDown (PointerEventData eventData) 
	{
		origin_parent = transform.parent;
		transform.SetParent(origin_parent.parent.parent);
		ofset = new Vector2(transform.position.x,transform.position.y)-eventData.position;
		GetComponent<CanvasGroup> ().blocksRaycasts = false;

	}

	public void OnDrag (PointerEventData eventData)
	{
		transform.position = eventData.position+ofset;	
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		transform.position = origin_parent.position;
		GetComponent<CanvasGroup> ().blocksRaycasts = true;
		transform.SetParent (origin_parent);
	}



	public void OnDrop(PointerEventData eventData)
	{
		if (GetComponent<Image> ()) 
		{
			Inventory inv = GetComponentInParent<Inventory> ();
			WeaponSwitch ws = inv.weapon_holder.GetComponentInParent<WeaponSwitch> ();
			ws.switch_in (eventData.pointerDrag.GetComponent<Slot>().number,number);
		}
	}



}
