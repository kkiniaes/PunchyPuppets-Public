using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverGroup : MonoBehaviour {

	private List<CanvasGroup> cGroups;

	private int hoveredIndex = -1;

	// Use this for initialization
	void Start () {
		cGroups = new List<CanvasGroup>();
		for (int i = 0; i < transform.childCount; i++) {
			CanvasGroup cGroup = transform.GetChild(i).gameObject.AddComponent<CanvasGroup>();
			cGroups.Add(cGroup);
		}
	}

	void Update() {
		if(hoveredIndex != -1) {
			for (int i = 0; i < cGroups.Count; i++) {
				if(i != hoveredIndex) {
					cGroups[i].alpha = Mathf.Lerp(cGroups[i].alpha, 0.5f, Time.deltaTime*10f);
				} else {
					cGroups[i].alpha = Mathf.Lerp(cGroups[i].alpha, 1f, Time.deltaTime*10f);
				}
			}
		} else {
			for (int i = 0; i < cGroups.Count; i++) {
				cGroups[i].alpha = Mathf.Lerp(cGroups[i].alpha, 1f, Time.deltaTime*10f);
			}
		}
	}
	
	public void ButtonHovered(int index) {
		hoveredIndex = index;
		// cGroups[index].alpha = 1f;
		// for (int i = 0; i < cGroups.Count; i++) {
		// 	if(i != index) {
		// 		cGroups[i].alpha = 0.5f;
		// 	}
		// }
	}

	public void ButtonExit(int index) {
		hoveredIndex = -1;
		// for (int i = 0; i < cGroups.Count; i++) {
		// 	if(i != index) {
		// 		cGroups[i].alpha = 1f;
		// 	}
		// }
	}
}
