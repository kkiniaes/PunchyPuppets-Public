using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BouncyButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler {

	private bool hovered, pressed;

	private Color initialColor;
	private Image img;
	private HoverGroup parentHoverGroup;

	public bool disableColorTransitions = false;

	public void OnPointerEnter(PointerEventData eventData) {
		hovered = true;
		transform.localScale = Vector3.one*1.1f;
		if(parentHoverGroup != null) {
			parentHoverGroup.ButtonHovered(transform.parent.GetSiblingIndex());
		}
		return;
	}

	public void OnPointerDown(PointerEventData eventData) {
		pressed = true;
		transform.localScale = Vector3.one*0.8f;
		return;
	}

	public void OnPointerUp(PointerEventData eventData) {
		pressed = false;
		transform.localScale = Vector3.one*1.1f;
		transform.parent.GetComponent<Button>().onClick.Invoke();
		return;
	}

	public void OnPointerExit(PointerEventData eventData) {
		hovered = false;
		pressed = false;
		if (parentHoverGroup != null) {
			parentHoverGroup.ButtonExit(transform.parent.GetSiblingIndex());
		}
		return;
	}

	// Use this for initialization
	void Start () {
		img = GetComponent<Image>();
		initialColor = img.color;
		parentHoverGroup = transform.parent.parent.GetComponent<HoverGroup>();
	}
	
	// Update is called once per frame
	void Update () {
		if(pressed) {
			if(!disableColorTransitions) {
				img.color = Color.Lerp(img.color, new Color(initialColor.r, initialColor.g, initialColor.b, 1f), Time.deltaTime*5f);
			}
			transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one*0.9f, Time.deltaTime*8f);
			return;
		}
		if(hovered) {
			if(!disableColorTransitions) {
				img.color = Color.Lerp(img.color, new Color(initialColor.r, initialColor.g, initialColor.b, 1f), Time.deltaTime*5f);
			}
			transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one*1.05f, Time.deltaTime*8f);
			return;
		}
		if(!disableColorTransitions) {
			img.color = Color.Lerp(img.color, new Color(initialColor.r, initialColor.g, initialColor.b, 0.75f), Time.deltaTime*5f);
		}
		transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime*8f);
	}
}
