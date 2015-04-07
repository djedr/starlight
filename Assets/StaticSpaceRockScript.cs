using UnityEngine;
using System.Collections;

public class StaticSpaceRockScript : MonoBehaviour {

	public float rotationMaxValue = 1f;

	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody> ().angularVelocity = new Vector3(Random.Range(0f, rotationMaxValue), Random.Range(0f, rotationMaxValue), Random.Range(0f, rotationMaxValue));
		//GetComponent<Rigidbody> ().angularVelocity = Vector3.up;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
