using UnityEngine;
using System.Collections;

public class DestroyerScript : MonoBehaviour {

	public Vector3 distance;

	private GameObject target;

	private Vector3 targetLastPos;

	// Use this for initialization
	void Start () {
		target = GameObject.Find ("Player");

		targetLastPos = target.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = targetLastPos + distance;

		targetLastPos = target.transform.position;
	}

	// When hit something:
	void OnTriggerEnter(Collider other) {
		Destroy(other.gameObject);
	}
}
