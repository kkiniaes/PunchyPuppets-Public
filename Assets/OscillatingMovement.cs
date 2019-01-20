using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillatingMovement : MonoBehaviour {

	public Vector3 translationOscillation, rotationOscillation;
	public float oscillateSpeed;
	private Vector3 startingPosition, startingRotation;

	// Use this for initialization
	void Start () {
		startingPosition = transform.position;
		startingRotation = transform.eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = startingPosition + Mathf.Sin(Time.time*oscillateSpeed)*translationOscillation;
		transform.eulerAngles = startingRotation + Mathf.Sin(Time.time*oscillateSpeed)*rotationOscillation;
	}
}
