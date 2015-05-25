using UnityEngine;
using System.Collections;

public class LevelGeneratorScript : MonoBehaviour {


	public float roadStartOffset = 0;
	public float roadLength = 100;
	public int roadPartLength = 600;
	public int roadLightSpeedLength = 400;
	public int roadRocklessLength = 50;
	public int roadDockingLength = 100;
	public float dockingSpeedDampen = 10;
	public float lightSpeedWidth = 10;
	public float roadWidthMin = 5;
	public float roadWidthMax = 10;
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
	public float turnVSpeed = 3;
	public float turnVLength = 10;
	public float widthLength = 40;
	public float straightLength = 2;

	public int rocksDistMin = 50;
	public int rocksDistMax = 70;

	public int rocksAmountMin = 1;
	public int rocksAmountMax = 3;

	public int mobileRocksAmountMin = 1;
	public int mobileRocksAmountMax = 2;
	public int mobileRocksDistMin = 20;
	public int mobileRocksDistMax = 30;

	public float rocksAngleStepMin = 45;
	public float rocksAngleStepMax = 70;

	private float startX;

	public int turn = 0;
	public float turnTimer = 4;
	public Vector3 landingPointOffset;

	private Vector3 forwardDir;
	private Vector3 rightDir;

	public enum generatorStates
	{
		straight,
		turns,
		other
	};

	public generatorStates states = generatorStates.straight;

	public float rockRadius = 6;
	public float rockAngle = 10;

	public GameObject[] staticRocks;
	public GameObject[] kinematicRocks;
	public GameObject[] movingRocks;
	public GameObject gate;
	public GameObject station;
	public GameObject[] miscObjs;
	public Vector3[] miscPoses;
	private GameObject player;

	private Vector3[] roadPoints;
	private Vector3[] previousRocks;
	private float[] roadSizes;
	private Vector3[] gatePoints;
	private int gateAmount = 0;
	private int previousRocksAmount = 0;
	private GameObject endStation;

	private int generatedSecondPart = 0;

	// Use this for initialization
	void Start () {

		// Initialize values:
		forwardDir = new Vector3(0f, 0f, 1f);
		rightDir = new Vector3(1f, 0f, 1f);
		rightDir.Normalize ();
		startX = transform.position.x;

		roadPoints = new Vector3[(int)Mathf.Ceil(roadLength)];
		previousRocks = new Vector3[(int)(360/stepAngleMin) + 2];
		roadSizes = new float[(int)Mathf.Ceil(roadLength)];
		gatePoints = new Vector3[(int)(roadLength / straightLength)];

		player = GameObject.Find ("Player");

		float rot;
		float startRot;
		float dist;

		int sum = 0;

		// Initialize first road point:
		roadPoints [0] = transform.position;
		roadSizes[0] = roadWidthMin;

		GenerateRoadCurve (1, roadPartLength);

		roadSizes[roadPartLength + roadLightSpeedLength - 1] = roadWidthMin;
		GenerateRoadCurve (roadPartLength + roadLightSpeedLength, (int)roadLength - roadDockingLength);




	}

	public void Begin()
	{
		#region Create rocks around the road
		
		// Generate static rocks around the road:
		GenerateRocks(0, stepAngleMax, 0, roadPartLength, (int)stepDistance, (int)stepDistance, stepAngleMin, stepAngleMax, 0, 0, 1.1f, 1.4f, staticRocks, 3);
		
		// Generate big static rocks around the road:
		GenerateRocks(0, 359, 0, roadPartLength, (int)stepDistance * 5, (int)stepDistance * 5, stepAngleMin * 5, stepAngleMax * 5, 0, 0, 1.7f, 1.9f, staticRocks, 9);
		
		// Generate kinematic rocks inside the road:
		GenerateRocks(0, 359, roadRocklessLength, roadPartLength, rocksDistMin, rocksDistMax, rocksAngleStepMin, rocksAngleStepMax, rockRadius, rockRadius, 0f, 0.9f, kinematicRocks, 3, rocksAmountMin, rocksAmountMax);
		
		
		// Generate rocks around lightspeed path:
		GenerateRocks(0, 359, roadPartLength + roadRocklessLength, roadPartLength + roadLightSpeedLength, (int)stepDistance, (int)stepDistance * 2, stepAngleMin, stepAngleMax, 0, roadWidthMin * 2, 1.2f, 1.2f, staticRocks, 3, 1, 3);

		Instantiate(miscObjs[1], roadPoints[roadPartLength + roadRocklessLength] + miscPoses[1], transform.rotation);
		
		/*
		GenerateRocks(0, stepAngleMax, roadPartLength + roadLightSpeedLength, (int)roadLength, (int)stepDistance, (int)stepDistance, stepAngleMin, stepAngleMax, 0, 0, 1f, 1.4f, staticRock);
		
		GenerateRocks(0, 359, roadPartLength + roadLightSpeedLength, (int)roadLength, rocksDistMin, rocksDistMax, rocksAngleStepMin, rocksAngleStepMax, rockRadius, rockRadius, 0f, 0.9f, kinematicRocks, rocksAmountMin, rocksAmountMax);
		*/
		#endregion
	}


	// When hit something:
	void OnTriggerEnter(Collider other) {
		Destroy(other.gameObject);
	}


	// arg0 - arg1: start and end point to work with.
	void GenerateRoadCurve(int arg0, int arg1){

		// Initialize vars for generation:
		int counter = (int)straightLength;
		float sinMultiplier = 0;
		
		// Generate path points:
		for (int i = arg0; i < arg1; i++)
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
		
		counter = (int)straightLength;
		sinMultiplier = 0;
		
		// Generate y path points:
		for (int i = arg0; i < arg1; i++)
		{
			counter--;
			
			// If in this step, path mode is changed:
			if (counter == 0)
			{
				// Mostly, decisions here are made, so just take x from previous point:
				roadPoints[i].y = roadPoints[i - 1].y;
				
				// If until now, it was straight road:
				if (sinMultiplier == 0)
				{
					// Set counter and sinMultiplier values:
					counter = (int)turnVLength + 1;
					sinMultiplier = (int)turnVSpeed;
					
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
					roadPoints[i].y = roadPoints[i - 1].y;
				}
				// If not, calculate new value on the curve:
				else
				{
					// passed stands for nuber of steps since beginning of the curve:
					int passed = (int)turnVLength - counter;
					
					// Now, make the curve look like cosinus:
					roadPoints[i].y = roadPoints[i - 1 - passed].y + Mathf.Cos(((float)passed / turnVLength) * Mathf.PI) * sinMultiplier;
					roadPoints[i].y -= sinMultiplier;
				}
			}
		}
		
		counter = (int)straightLength;
		sinMultiplier = 0;

		for (int i = arg0; i < arg1; i++)
		{
			counter--;
			
			if (counter == 0)
			{
				roadSizes[i] = roadSizes[i - 1];
				
				if (roadSizes[i] == roadWidthMin)
				{
					sinMultiplier = Random.Range(roadWidthMin, roadWidthMax);
				}
				else
				{
					sinMultiplier = roadWidthMin;

					if (i + (int)widthLength < arg1)
					{
						gatePoints[gateAmount] = roadPoints[i + (int)widthLength];
						gateAmount++;
					}
				}
				
				float temp = sinMultiplier - roadSizes[i];
				
				for (int j = 1; j <= widthLength && i + j < arg1; j++)
				{
					roadSizes[i + j] = roadSizes[i + j - 1] + temp / widthLength;
				}
				
				i += (int)widthLength;
				
				counter = (int)straightLength;
			}
			else
			{
				roadSizes[i] = roadSizes[i - 1];
			}
		}

		for (int i = arg1; i < (int)roadLength; i++)
		{
			roadPoints[i] = roadPoints[i - 1];
			roadPoints[i] += new Vector3(0, 0, 1);
			roadSizes[i] = lightSpeedWidth;
		}
		
		for (int i = 0; i < gateAmount; i++)
		{
			//Instantiate(gate, gatePoints[i], transform.rotation);
		}
	}


	
	private int DoesHitARock(Vector3 pos, float rockSize)
	{
		for (int i = 0; i < previousRocksAmount; i++)
		{
			if (Vector3.SqrMagnitude(pos - previousRocks[i]) < rockRadius * rockRadius * rockSize * rockSize)
				return i;
		}

		return -1;
	}

	void GenerateRocks(float rotInitMin, float rotInitMax, int stepInit, int stepFinish, int stepMin, int stepMax, float angleStepMin, float angleStepMax, float distMin, float distMax, float distMultMin, float distMultMax, GameObject[] objs, float rockSize, int rocksPerStepMin = 500, int rocksPerStepMax = 500)
	{
		Vector3 lastPos = Vector3.zero;

		// For each step on main road:
		for (int i = stepInit; i < stepFinish; i += (int)Random.Range(stepMin, stepMax))
		{
			// Take initial rotation value:
			float rot = Random.Range(rotInitMin, rotInitMax);
			
			if (i % 2 == 0)
			{
				rot += (angleStepMin) / 2;
			}
			float dist = Random.Range(distMin, distMax) + Random.Range(distMultMin, distMultMax) * roadSizes[i];
			
			// Record this starting value:
			float startRot = rot;
			
			// Reset currentRocks container:
			Vector3[] currentRocks = new Vector3[previousRocks.Length];
			int currentRocksAmount = 0;

			int sum = 0;

			int rocksPerStep = Random.Range(rocksPerStepMin, rocksPerStepMax);
			
			// Generate rocks around this point:
			while (rot < startRot + 360 - rockAngle && sum < rocksPerStep)
			{
				// Calculate new rock's position:
				Vector3 newPos = new Vector3(roadPoints[i].x + Mathf.Cos(rot * Mathf.Deg2Rad) * dist, roadPoints[i].y + Mathf.Sin(rot * Mathf.Deg2Rad) * dist, roadPoints[i].z);
				
				// Check if the new location doesn't collide with old ones from previous ring:
				if (Vector3.SqrMagnitude(newPos - lastPos) > rockRadius * rockRadius * rockSize * rockSize && DoesHitARock(newPos, rockSize) == -1)
				{
					GameObject temp = (GameObject)Instantiate(objs[Random.Range(0, objs.Length)], newPos, transform.rotation);
					temp.transform.localScale = new Vector3(rockSize, rockSize, rockSize);

					currentRocks[currentRocksAmount] = newPos;
					currentRocksAmount++;
					sum++;

					lastPos = newPos;
					
					rot += Random.Range(angleStepMin, angleStepMax);
					dist = Random.Range(distMin, distMax) + Random.Range(distMultMin, distMultMax) * roadSizes[i];
				}
				// If it does, change rotation slightly and do nothing else:
				else
				{
					rot += Random.Range(0, angleStepMax - angleStepMin);
				}
				
				
			}
			
			previousRocksAmount = currentRocksAmount;
			previousRocks = currentRocks;
			
		}
	}

	public Vector3 MiddleRoadPoint(Vector3 arg)
	{
		float temp = arg.z - transform.position.z;
		
		temp = Mathf.Floor (temp);

		if (temp < 0 || temp > roadPoints.Length - 1 || roadPoints == null)
			return Vector3.zero;
		else
			return roadPoints [(int)temp];
	}

	public Vector3 OffRoad(Vector3 arg)
	{
		float temp = arg.z - transform.position.z;

		temp = Mathf.Floor (temp);

		if (temp < 0 || temp > roadPoints.Length - 1)
			return Vector3.zero;

		if (Vector3.SqrMagnitude (arg - roadPoints [(int)temp]) < roadSizes [(int)temp] * roadSizes [(int)temp] * 0.7f * 0.7f)
			return Vector3.zero;

		Vector3 norm = arg - roadPoints[(int)temp];
		norm.Normalize ();

		Vector3 toReturn = Vector3.zero;

		if (norm.x > 0.5)
			toReturn.x = 1;
		else if (norm.x < -0.5)
			toReturn.x = -1;

		if (norm.y > 0.5)
			toReturn.y = 1;
		else if (norm.y < -0.5)
			toReturn.y = -1;

		return toReturn;
	}

	public void EndGameSequence()
	{
		endStation.GetComponent<SpaceShuttleScript> ().Close ();

		player.transform.parent = endStation.GetComponent<SpaceShuttleScript>().armParts[2].transform;

		player.GetComponent<SpaceShuttleScript> ().Close ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (generatedSecondPart == 0 && player.transform.position.z - transform.position.z > roadPartLength)
		{
			generatedSecondPart = 1;

			// Generate static rocks around the road:
			GenerateRocks(0, stepAngleMax, roadPartLength + roadLightSpeedLength, (int)roadLength - roadDockingLength, (int)stepDistance, (int)stepDistance, stepAngleMin, stepAngleMax, 0, 0, 1f, 1.4f, staticRocks, 3);

			// Generate big static rocks around the road:
			GenerateRocks(0, 359, roadPartLength + roadLightSpeedLength, (int)roadLength - roadDockingLength, (int)stepDistance * 5, (int)stepDistance * 5, stepAngleMin * 5, stepAngleMax * 5, 0, 0, 1.7f, 1.9f, staticRocks, 9);

			// Generate kinematic rocks inside the road:
			GenerateRocks(0, 359, roadPartLength + roadLightSpeedLength + roadRocklessLength, (int)roadLength - roadDockingLength, rocksDistMin, rocksDistMax, rocksAngleStepMin, rocksAngleStepMax, rockRadius, rockRadius, 0f, 0.9f, kinematicRocks, 3, rocksAmountMin, rocksAmountMax);

			// Generate mobile rocks inside the road:
			GenerateRocks(0, 359, roadPartLength + roadLightSpeedLength + roadRocklessLength, (int)roadLength - roadDockingLength, mobileRocksDistMin, mobileRocksDistMax, rocksAngleStepMin, rocksAngleStepMax, rockRadius, rockRadius, 0.9f, 0.9f, movingRocks, 3, mobileRocksAmountMin, mobileRocksAmountMax);


			endStation = (GameObject)Instantiate(station, roadPoints[(int)roadLength - 1], transform.rotation);

			player.GetComponent<PlayerScript> ().landingDestination = roadPoints[(int)roadLength - 1] + landingPointOffset;

			player.GetComponent<PlayerScript> ().EnterLightSpeedMode();
		}

		else if (generatedSecondPart == 1 && player.transform.position.z - transform.position.z + player.GetComponent<Rigidbody>().velocity.z * Time.deltaTime > roadPartLength + roadLightSpeedLength)
		{
			generatedSecondPart = 2;

			player.GetComponent<PlayerScript> ().LeaveLightSpeedMode();
			player.transform.position = new Vector3(
				player.transform.position.x,
				player.transform.position.y,
				transform.position.z + roadPartLength + roadLightSpeedLength);
		}

		else if (generatedSecondPart == 2 && player.transform.position.z - transform.position.z > roadLength - roadDockingLength)
		{
			generatedSecondPart = 3;
			
			player.GetComponent<PlayerScript> ().EnterDockingMode();
		}

		else if (generatedSecondPart == 3 && player.transform.position.z - transform.position.z > roadLength)
		{
			player.GetComponent<PlayerScript> ().movementMaxSpeed -= dockingSpeedDampen * Time.deltaTime;

			if (player.GetComponent<PlayerScript> ().movementMaxSpeed < 0)
			{
				player.GetComponent<PlayerScript> ().movementMaxSpeed = 0;
			}
		}
	}
}
