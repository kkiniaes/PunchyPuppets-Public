using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringScript : MonoBehaviour {
	
	public Transform target;
	public float scaleFactor;

	private Vector3 offset;
	private float startingDistance;
	private Vector3 startingScale;

	// Use this for initialization
	void Start () {
		offset = transform.position - target.position;
		startingDistance = offset.magnitude;
		startingScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = new Vector3(startingScale.x, startingScale.y + (Vector3.Distance(transform.position, target.position) - startingDistance) * scaleFactor, startingScale.z);
		transform.up = target.position - transform.position;
	}
}
