using UnityEngine;
using System.Collections;

public class LevelGeneratorScript : MonoBehaviour {


	public float roadStartOffset = 0;
	public float roadLength = 100;
	public float roadWidth = 5;
	public float totalWidth = 15;
	public float totalHeight = 15;
	public float stepDistance = 2;
	public float stepAngleMin = 30;
	public float stepAngleMax = 40;
	public float wideStepAngleMin = 45;
	public float wideStepAngleMax = 65;
	public float stationWidth = 10;

	public float generationSpeed = 20;
	public float turnHSpeed = 5;
	public float turnHDampen = 5;
	public float turnHLength = 1;
	public float straightLength = 2;

	private float startX;

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

	private Vector3[] roadPoints;
	private Vector3[] previousRocks;
	private int previousRocksAmount = 0;
	public float rockRadius = 6;

	public GameObject staticRock;

	// Use this for initialization
	void Start () {

		// Initialize values:
		forwardDir = new Vector3(0f, 0f, 1f);
		rightDir = new Vector3(1f, 0f, 1f);
		rightDir.Normalize ();
		startX = transform.position.x;

		roadPoints = new Vector3[(int)Mathf.Ceil(roadLength)];
		previousRocks = new Vector3[(int)(360/stepAngleMin) + 2];

		float rot;
		float startRot;
		float dist;

		int sum = 0;

		#region Generate road curve
		// Initialize first road point:
		roadPoints [0] = transform.position;

		// Initialize vars for generation:
		int counter = (int)straightLength;
		float sinMultiplier = 0;

		// Generate path points:
		for (int i = 1; i < (int)roadLength; i++)
		{
			// Keep y and z values from previous position:
			roadPoints[i].z = transform.position.z + (float)i;

			roadPoints[i].y = roadPoints[i - 1].y;

			counter--;

			// If in this step, path mode is changed:
			if (counter == 0)
			{
				// Mostly, decisions here are made, so just take x from previous point:
				roadPoints[i].x = roadPoints[i - 1].x;

				// If until now, it was straight road:
				if (sinMultiplier == 0)
				{
					// Set counter and sinMultiplier values:
					counter = (int)turnHLength + 1;
					sinMultiplier = (int)turnHSpeed;

					// Temp value is meant to have value of either -1 or 1 after theese instructions:
					int temp = (int)Mathf.Floor(Random.Range(0f, 2f));
					// Now, temp is either 0, or 1
					temp *= 2;
					// Now, temp is either 0 or 2
					temp -= 1;
					// Now, temp is either -1 or 1

					// Now, we multiply sinMultiplier by -1 or 1 randomily:
					sinMultiplier *= temp;
				}
				// Else, if until now, we were turning, make further road straight:
				else
				{
					counter = (int)straightLength;
					sinMultiplier = 0;
				}
			}
			// If this step is just another one of many:
			else
			{
				// If this is traight road, repeat previous value:
				if (sinMultiplier == 0)
				{
					roadPoints[i].x = roadPoints[i - 1].x;
				}
				// If not, calculate new value on the curve:
				else
				{
					// passed stands for nuber of steps since beginning of the curve:
					int passed = (int)turnHLength - counter;

					// Now, make the curve look like cosinus:
					roadPoints[i].x = roadPoints[i - 1 - passed].x + Mathf.Cos(((float)passed / turnHLength) * Mathf.PI) * sinMultiplier;
					roadPoints[i].x -= sinMultiplier;
				}
			}
		}

		#endregion



		#region Create rocks creating the road

		for (int i = 0; i < (int)roadLength; i += (int)stepDistance)
		{
			rot = Random.Range(0f, stepAngleMax);

			if (i % 2 == 0)
			{
				rot += (stepAngleMin) / 2;
			}
			dist = roadWidth;

			startRot = rot;

			Vector3[] currentRocks = new Vector3[previousRocks.Length];
			int currentRocksAmount = 0;

			while (rot < startRot + 360 - rockRadius - rockRadius)
			{
				Vector3 newPos = new Vector3(roadPoints[i].x + Mathf.Cos(rot * Mathf.Deg2Rad) * roadWidth, roadPoints[i].y + Mathf.Sin(rot * Mathf.Deg2Rad) * roadWidth, roadPoints[i].z);

				if (DoesHitARock(newPos) == -1)
				{
					Instantiate(staticRock, newPos, transform.rotation);
					currentRocks[currentRocksAmount] = newPos;
					currentRocksAmount++;

					rot += Random.Range(stepAngleMin, stepAngleMax);
				}
				else
				{
					rot += Random.Range(0, stepAngleMax - stepAngleMin);
				}


			}

			previousRocksAmount = currentRocksAmount;
			previousRocks = currentRocks;

		}

		#endregion

		#region Create rocks around the road
		
		for (int i = 0; i < (int)roadLength; i += (int)stepDistance)
		{
			rot = Random.Range(0f, wideStepAngleMax);
			
			if (i % 2 == 0)
			{
				rot += (wideStepAngleMin) / 2;
			}
			dist = Random.Range(roadWidth + rockRadius + rockRadius, totalWidth);
			
			startRot = rot;
			
			Vector3[] currentRocks = new Vector3[previousRocks.Length];
			int currentRocksAmount = 0;
			
			while (rot < startRot + 360 - rockRadius - rockRadius)
			{
				Vector3 newPos = new Vector3(roadPoints[i].x + Mathf.Cos(rot * Mathf.Deg2Rad) * dist, roadPoints[i].y + Mathf.Sin(rot * Mathf.Deg2Rad) * dist, roadPoints[i].z);
				
				if (DoesHitARock(newPos) == -1)
				{
					Instantiate(staticRock, newPos, transform.rotation);
					currentRocks[currentRocksAmount] = newPos;
					currentRocksAmount++;
					
					rot += Random.Range(wideStepAngleMin, wideStepAngleMax);
					dist = Random.Range(roadWidth + rockRadius, totalWidth);
				}
				else
				{
					rot += Random.Range(0, wideStepAngleMax - wideStepAngleMin);
				}
				
				
			}
			
			previousRocksAmount = currentRocksAmount;
			previousRocks = currentRocks;
			
		}

		#endregion
	}


	// When hit something:
	void OnTriggerEnter(Collider other) {
		Destroy(other.gameObject);
	}

	private int DoesHitARock(Vector3 pos)
	{
		for (int i = 0; i < previousRocksAmount; i++)
		{
			if (Vector3.SqrMagnitude(pos - previousRocks[i]) < rockRadius * rockRadius)
				return i;
		}

		return -1;
	}
	
	// Update is called once per frame
	void Update () {
		/*
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
				turn = (int)Mathf.Floor(Random.Range(0f, 3f));
				//turn *= 2;
				turn--;

				turnTimer = turnHLength;
			}
		}

		if (transform.position.x - startX > totalWidth && turn > 0)
			turn = - turn;

		if (transform.position.x - startX < -totalWidth && turn < 0)
			turn = - turn;

		float step = turnHSpeed * Time.deltaTime;
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(forwardDir + rightDir * turn, Vector3.up), step);

		GetComponent<Rigidbody> ().velocity = transform.forward * generationSpeed;
		*/
	}
}
