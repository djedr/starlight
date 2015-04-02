using UnityEngine;
using System.Collections;

public class SpaceRockScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody> ().angularVelocity = Vector3.up;
		//GetComponent<Rigidbody>().AddTorque(new Vector3())
		//transform.Rotate (new Vector3 (0, 100 * Time.deltaTime, 0));
	}
	
	// Update is called once per frame
	void Update () {
		//transform.Rotate (Vector3.up);
	}
}
