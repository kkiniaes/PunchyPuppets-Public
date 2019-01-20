using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {
	private Vector3 offset;

	private Transform player1, player2;
    private bool offSetSet = false;

	private float trauma;
	private Vector3 shakeVector;
	public float maxScreenshake = 5f;
	public float screenshakeRecovery = 1f;
	private float screenshakeTime;

	private Camera cam;
	private Camera bgCam;

	void OnEnable() {
		cam = GetComponent<Camera>();
		bgCam = transform.GetChild(0).GetComponent<Camera>();
	}

	public void SetTargets(Transform player1, Transform player2) {
		this.player1 = player1;
		this.player2 = player2;
        if (offSetSet) return;
        offset = transform.position - (player1.position + player2.position) / 2f;
        offSetSet = true;
    }
	
	// Update is called once per frame
	void Update () {
		screenshakeTime += GameManager.Instance.DeltaTime(-1) * 10f;
		if(player1 != null) {
			Vector3 targetPosition = Vector3.Lerp(transform.position, offset + (player1.position + player2.position)/2f, Time.deltaTime*5f);
			if(targetPosition.y < 1f) {
				targetPosition = new  Vector3(targetPosition.x, 1f, targetPosition.z);
			}
			trauma = Mathf.Lerp(trauma, 0f, GameManager.Instance.DeltaTime(-1) * screenshakeRecovery);
			shakeVector = new Vector3(Mathf.PerlinNoise(screenshakeTime,0f)-0.5f,Mathf.PerlinNoise(0f,screenshakeTime)-0.5f, 0f) * trauma/8f;
			cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60f - trauma*2f, Time.deltaTime*screenshakeRecovery);
			bgCam.fieldOfView = Mathf.Lerp(bgCam.fieldOfView, 60f - trauma*2f, Time.deltaTime*screenshakeRecovery);
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime*screenshakeRecovery);
			transform.position = targetPosition + shakeVector;
		}
	}

	public void Screenshake(float intensity) {
		cam.fieldOfView -= 3f;
		bgCam.fieldOfView -= 3f;
		transform.eulerAngles = Vector3.forward*Random.Range(-5f,5f);
		trauma += intensity;
		if(trauma > maxScreenshake) {
			trauma = maxScreenshake;
		}
	}
}
