using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalStateHandler : MonoBehaviour {

	public bool inCriticalState;

	private SpriteRenderer defaultFace, hurtFace;

	private Quaternion startingRotation;

    [EnumsFlags]
    private GameManager.PauseMode pauseRegister = (GameManager.PauseMode)(-1);

	void Start() {
		defaultFace = transform.Find("FaceSprite").GetComponent<SpriteRenderer>();
		hurtFace = transform.Find("HurtFace").GetComponent<SpriteRenderer>();
		startingRotation = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {
		if(inCriticalState) {
			transform.localEulerAngles += Vector3.up*900f* GameManager.Instance.DeltaTime(pauseRegister);
		} else {
			transform.localRotation = Quaternion.Lerp(transform.localRotation, startingRotation, GameManager.Instance.DeltaTime(pauseRegister) * 5f);
		}
	}

	public void UpdateCriticalState(bool state) {
		inCriticalState = state;
		if(inCriticalState) {
			defaultFace.enabled = false;
			hurtFace.enabled = true;
		} else {
			defaultFace.enabled = true;
			hurtFace.enabled = false;
		}
	}
}
