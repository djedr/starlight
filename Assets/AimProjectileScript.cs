using UnityEngine;
using System.Collections;

public class AimProjectileScript : MonoBehaviour {

	private PlayerScript player;
	public float timer = 0.5f;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player").GetComponent<PlayerScript> ();
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.tag == "Rock")
		{
			player.targetedRock = other.gameObject;
			Destroy(this.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;

		if (timer <= 0)
		{
			player.targetedRock = null;
			Destroy(this.gameObject);
		}
	}
}
