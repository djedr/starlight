﻿using UnityEngine;
using System.Collections;

public class SpaceRockScript : MonoBehaviour {

	public float rotationMaxValue = 1f;

	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody> ().angularVelocity = new Vector3(Random.Range(0f, rotationMaxValue), Random.Range(0f, rotationMaxValue), Random.Range(0f, rotationMaxValue));
		//GetComponent<Rigidbody>().AddTorque(new Vector3())
		//transform.Rotate (new Vector3 (0, 100 * Time.deltaTime, 0));
	}
	
	// Update is called once per frame
	void Update () {
		//transform.Rotate (Vector3.up);
	}
}
