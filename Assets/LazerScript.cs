using UnityEngine;
using System.Collections;

public class LazerScript : MonoBehaviour {

	public Vector3 startPos;
	public Vector3 endPos;

	public float timeTotal = 0.1f;
	public float timeWait = 0.1f;
	public float timeCounter = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		timeCounter += Time.deltaTime;

		float part;
		if (timeCounter > timeTotal)
			part = 1;
		else
			part = timeCounter / timeTotal;

		transform.position = startPos + (endPos - startPos) * part;

		if (timeCounter >= timeTotal + timeWait)
		{
			Destroy (this.gameObject);
		}
	}
}
