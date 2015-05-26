using UnityEngine;
using System.Collections;

public class BlinkingLightScript : MonoBehaviour {

	public float timer = 0;
	public float timerMax = 0.5f;

	public bool active = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		timer -= Time.deltaTime;

		if (timer <= 0)
		{
			timer = timerMax;

			active = !active;
		}

		GetComponent<Light> ().enabled = active;
	}
}
