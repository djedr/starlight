using UnityEngine;
using System.Collections;

public class SpaceHomingRockScript : MonoBehaviour {

	public float activationDistance = 100;
	public Vector3 targetLocation;
	public float moveSpeed = 10;

	private bool activated = false;
	private GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");

		activationDistance = activationDistance * activationDistance;

		GameObject temp = GameObject.Find ("LevelGenerator");
		
		targetLocation = temp.GetComponent<LevelGeneratorScript> ().MiddleRoadPoint (transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		if (activated == false)
		{
			if (Vector3.SqrMagnitude(player.transform.position - transform.position) < activationDistance)
			{
				activated = true;

				GetComponent<Rigidbody>().velocity = (targetLocation - transform.position).normalized * moveSpeed;
			}
		}
	}
}
