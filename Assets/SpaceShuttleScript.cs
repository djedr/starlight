using UnityEngine;
using System.Collections;

public class SpaceShuttleScript : MonoBehaviour {

	public GameObject[] armParts;
	public Vector3[] armAngleStart;
	//private Vector3[] armRots;
	public Vector3[] armAngleEnd;
	public float[] armTimesStart;
	public float[] armTimesEnd;
	public GameObject[] soundEmitters;
	public float[] soundTimes;
	public GameObject[] particleGens;
	public float[] particleTimes;
	public float armTimeMax = 10;

	public bool animateOnStart = false;

	private float timeCounter = -1;
	private int soundsPlayed = 0;
	private int particlesPlayed = 0;

	public void Close()
	{
		if (timeCounter == -1)
			timeCounter = 0;
	}

	// Use this for initialization
	void Start () {
		if (animateOnStart == true)
			Close ();

		//for (int i = 0; i < armParts.Length; i++)

	}
	
	// Update is called once per frame
	void Update () {
		// If rotation is in progress:
		if (timeCounter >= 0)
		{
			// timeCounter keeps track of total time flow:
			timeCounter += Time.deltaTime;

			// If timeCounter reaches end of total time of rotation, make sure, it will call
			// at least once for full value of armTimeMax, to set every rotating part in its destined place:
			if (timeCounter > armTimeMax)
				timeCounter = armTimeMax;
			else if (timeCounter == armTimeMax)
				timeCounter = -1;

			int i, j;

			// For each rotating part:
			for (i = 0; i < armParts.Length; i++)
			{
				// time will take values from 0 to 1 to represent rotation moment between armAngleStart and armAngleEnd:
				float time;
				if (timeCounter < armTimesStart[i])
					time = 0;
				else if (timeCounter > armTimesEnd[i])
					time = 1;
				else
					time = (timeCounter - armTimesStart[i]) / (armTimesEnd[i] - armTimesStart[i]);

				Vector3 temp = Vector3.zero;

				// Now, assign new rotation values:
				for (j = 0; j < 3; j++)
					temp[j] = armAngleStart[i][j] + (armAngleEnd[i][j] - armAngleStart[i][j]) * time;

				// Since rotations are stored in wuaternions, convert our vector into quaternion:
				armParts[i].transform.localRotation = Quaternion.Euler(temp);
			}

			if (soundsPlayed < soundEmitters.Length)
			{
				if (timeCounter >= soundTimes[soundsPlayed])
				{
					soundEmitters[soundsPlayed].GetComponent<AudioSource>().Play();

					soundsPlayed++;
				}
			}

			if (particlesPlayed < particleGens.Length)
			{
				if (timeCounter >= particleTimes[particlesPlayed])
				{
					particleGens[particlesPlayed].GetComponent<ParticleSystem>().Play();
					
					particlesPlayed++;
				}
			}
		}
	}
}
