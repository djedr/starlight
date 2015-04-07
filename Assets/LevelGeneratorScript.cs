using UnityEngine;
using System.Collections;

public class LevelGeneratorScript : MonoBehaviour {


	public float roadStartOffset = 0;
	public float roadLength = 100;
	public float roadWidth = 5;
	public float totalWidth = 15;
	public float stepDistance = 2;
	public float stationWidth = 10;

	public float generationSpeed = 20;
	public float turnHSpeed = 5;
	public float turnHDampen = 5;
	public float turnHLength = 1;
	public float straightLength = 2;

	public int turn = 0;
	public float turnTimer = 4;

	private Vector3 forwardDir;
	private Vector3 rightDir;

	public enum generatorStates
	{
		straight,
		turns,
		other
	};

	public generatorStates states = generatorStates.straight;

	private Vector3[] lastStepRockss;

	public GameObject staticRock;

	// Use this for initialization
	void Start () {

		forwardDir = new Vector3(0f, 0f, 1f);
		rightDir = new Vector3(1f, 0f, 1f);
		rightDir.Normalize ();

		float rot;
		float dist;

		int sum = 0;

		/*
		for (float i = roadStartOffset; i < roadLength; i += stepDistance)
		{
			rot = Random.Range(0f, 360f);
			dist = Random.Range(roadWidth, totalWidth);

			Vector3 newPos = new Vector3(transform.position.x + Mathf.Cos(rot) * roadWidth, transform.position.y + Mathf.Sin(rot) * roadWidth, transform.position.z + i);
			RaycastHit tempRay = new RaycastHit();
			if (Physics.SphereCast(newPos, 300, Vector3.up, out tempRay) == false)
				Instantiate(staticRock, newPos, transform.rotation);
			//Instantiate(staticRock, transform.position + Vector3.up * i, transform.rotation);
		}
		*/



		for (float i = roadStartOffset; i < roadLength; i += stepDistance)
		{
			for (float j = -totalWidth; j < totalWidth; j += stepDistance)
			{
				for (float k = -totalWidth; k < totalWidth; k += stepDistance)
				{
					Vector3 newPos = new Vector3(transform.position.x + j, transform.position.y + k, transform.position.z + i);
					Instantiate(staticRock, newPos, transform.rotation);

					sum++;

					if (sum > 600)
					{
						i = roadLength;
						j = totalWidth;
						k = totalWidth;
					}
				}
			}
		}
	}

	// When hit something:
	void OnTriggerEnter(Collider other) {
		Destroy(other.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		turnTimer -= Time.deltaTime;
		if (turnTimer <= 0)
		{
			if (turn != 0)
			{
				turn = 0;

				turnTimer = straightLength;
			}
			else
			{
				turn = (int)Mathf.Floor(Random.Range(0f, 2f));
				turn *= 2;
				turn--;

				turnTimer = turnHLength;
			}
		}

		float step = turnHSpeed * Time.deltaTime;
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forwardDir + rightDir * turn, Vector3.up), step);

		GetComponent<Rigidbody> ().velocity = transform.forward * generationSpeed;
	}
}
