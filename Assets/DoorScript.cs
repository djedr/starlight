using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour {

	public int activated = 1;

	public float[] dists;
	public float[] speeds;
	public float countdown = 1;

	private Vector3 startPoint;
	private int step = 0;
	private float currentDistance = 0;

	public GameObject audioObject = null;

	// To activate the door, you can call this or just make the public var active = 1:
	public void Activate()
	{
		activated = 1;
	}

	// Use this for initialization
	void Start () {
		startPoint = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (activated == 1)
		{

			if (countdown <= 0)
			{
				if (Mathf.Abs (dists [step] - currentDistance) < speeds [step] * Time.deltaTime)
				{
					currentDistance = dists [step];
					if (step < dists.Length - 1)
						step++;
				}
				else if (currentDistance < dists [step])
				{
					currentDistance += speeds [step] * Time.deltaTime;
				}
				else
				{
					currentDistance -= speeds [step] * Time.deltaTime;
				}

				transform.position = transform.forward * currentDistance + startPoint;
			}
			else
			{
				countdown -= Time.deltaTime;

				if (countdown <= 0 && audioObject != null)
					audioObject.GetComponent<AudioSource>().Play();
			}
		}
	}
}
