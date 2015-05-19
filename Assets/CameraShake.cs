using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {
	public float shake = 0;
	public float shakeAmount = 0.1f;
	public float decreaseFactor = 1;

	private Vector3 initialPosition;
	private bool shakeStarted = false;
	
	// Use this for initialization
	void Start () {
		initialPosition = this.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		if (shake > 0) {
			if (shakeStarted == false) {
				initialPosition = this.transform.localPosition;
				shakeStarted = true;
			}

			this.transform.localPosition = initialPosition + Random.insideUnitSphere * shakeAmount;
			shake -= Time.deltaTime * decreaseFactor;
		} else {
			shake = 0;
			this.transform.localPosition = initialPosition;
			shakeStarted = false;
		}
	}
}