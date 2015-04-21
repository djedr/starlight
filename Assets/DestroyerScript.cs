using UnityEngine;
using System.Collections;

public class DestroyerScript : MonoBehaviour {

	public Vector3 distance;

	private GameObject target;

	// Use this for initialization
	void Start () {
		target = GameObject.Find ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = target.transform.position + distance;
	}

	// When hit something:
	void OnTriggerEnter(Collider other) {
		Destroy(other.gameObject);
	}
}
