using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public enum StateTypes
	{
		InControl,
		Bounced,
		OnStart,
		OnLaunch,
		BeforeLightSpeed,
		LightSpeed
	};

	// player properties

	public StateTypes state = StateTypes.OnStart;

	public float movementSpeed = 0;
	public float movementMaxSpeed = 10;
	public float movementAcceleration = 1;
	public float movementAttenuation = 0.1f;

	public float rotationHSpeed = 0;
	public float rotationVSpeed = 0;
	public float rotationHAcceleration = 20;
	public float rotationVAcceleration = 20;
	public float rotationHDampen = 20;
	public float rotationVDampen = 20;
	public float rotationHMaxSpeed = 40;
	public float rotationVMaxSpeed = 40;
	public float shipRotation = 0;

	public float shipMaxRotationH = 0.5f;
	public float shipMaxRotationV = 0.4f;

	public float timer = 1;
	public float stunTime = 0.5f;
	public float launchTime = 1;
	public float pushForceMultiplier = 2;
	public float pushForceConstant = 5;

	public Vector3 additionalSpeed = Vector3.zero;

	public float additionalSpeedDampen = 20;

	// camera properties
	public float cameraMaxAngle = 90;
	public float cameraMinAngle = 0;

	public float cameraXMultiplier = 100;
	public float cameraRotation = 0;

	// key bindings

	// variables used in code
	private bool movementHPressed = false;
	private bool movementVPressed = false;

	private GameObject levelGenerator;

	public GameObject camera = null;

	// Use this for initialization
	void Start () {
		Screen.lockCursor = true;

		camera = GameObject.Find ("Main Camera");

		timer = launchTime;

		levelGenerator = GameObject.Find ("LevelGenerator");
	}


	// When hit something:
	void OnCollisionEnter(Collision collision)
	{
		Vector3 temp = transform.position - collision.gameObject.transform.position;

		temp.Normalize ();

		// dot2: 0 means touching with side, +value direct hit
		// dot: 0 means direct hit, +value touching with side:
		float dot2 = Mathf.Abs(Vector3.Dot(transform.forward, temp));
		float dot = 1 - dot2;

		// Push away from hit rock:
		additionalSpeed = temp * (pushForceMultiplier * movementSpeed * dot2 + pushForceConstant);
		state = StateTypes.Bounced;
		timer = stunTime;

		movementSpeed = movementSpeed * dot * 1f;
		//movementSpeed = movementSpeed / 2f;
	}


	// Update is called once per frame
	void Update () {

		// Temp rotationVector for steering the vehicle
		var rotationVector = GetComponent<Rigidbody>().rotation;

		#region states

		if (timer > 0)
		{
			timer -= Time.deltaTime;
			if (timer <= 0)
			{
				timer = 0;
				if (state == StateTypes.OnStart)
				{
					state = StateTypes.OnLaunch;
					timer = launchTime;
				}
				else
					state = StateTypes.InControl;
			}
		}

		#endregion


		#region rotation

		// Variables to keep track if player is turning in horizontal and vertical plane this frame:
		movementHPressed = false;
		movementVPressed = false;
	
		// Get directional input:
		//if (state == StateTypes.InControl)
		//{
			Vector3 tooFar = levelGenerator.GetComponent<LevelGeneratorScript> ().OffRoad(transform.position);

			if ( (Input.GetKey(KeyCode.D) && transform.forward.x < shipMaxRotationH && tooFar.x != 1 && state == StateTypes.InControl) || (tooFar.x == -1 && transform.forward.x < 0.1f))
			{
				rotationHSpeed += rotationHAcceleration * Time.deltaTime;
				if (rotationHSpeed > rotationHMaxSpeed)
					rotationHSpeed = rotationHMaxSpeed;

				movementHPressed = true;

				//transform.Rotate (new Vector3(0, Time.deltaTime * rotationSpeed, 0));
			}
			else if ( (Input.GetKey(KeyCode.A) && transform.forward.x > -shipMaxRotationH && tooFar.x != -1 && state == StateTypes.InControl) || (tooFar.x == 1 && transform.forward.x > -0.1f))
			{
				rotationHSpeed -= rotationHAcceleration * Time.deltaTime;
				if (rotationHSpeed < -rotationHMaxSpeed)
					rotationHSpeed = -rotationHMaxSpeed;

				movementHPressed = true;

				//transform.Rotate (new Vector3(0, -Time.deltaTime * rotationSpeed, 0));
			}

			if ( (Input.GetKey(KeyCode.W) && transform.forward.y > -shipMaxRotationV && tooFar.y != -1 && state == StateTypes.InControl) || (tooFar.y == 1 && transform.forward.y > -0.1f))
			{
				rotationVSpeed += rotationVAcceleration * Time.deltaTime;
				if (rotationVSpeed > rotationVMaxSpeed)
					rotationVSpeed = rotationVMaxSpeed;

				movementVPressed = true;

				//transform.Rotate (new Vector3(Time.deltaTime * rotationSpeed, 0, 0));
			}
			else if ( (Input.GetKey(KeyCode.S) && transform.forward.y < shipMaxRotationV && tooFar.y != 1 && state == StateTypes.InControl) || (tooFar.y == -1 && transform.forward.y < 0.1f))
			{
				rotationVSpeed -= rotationVAcceleration * Time.deltaTime;
				if (rotationVSpeed < -rotationVMaxSpeed)
					rotationVSpeed = -rotationVMaxSpeed;

				movementVPressed = true;

				//transform.Rotate (new Vector3(-Time.deltaTime * rotationSpeed, 0, 0));
			}
		//}


		// Slow down rotation movement if the player is not turning in given plane:
		// Horizontal:
		if (movementHPressed == false)
		{
			if (Mathf.Abs(rotationHSpeed) < rotationHDampen * Time.deltaTime)
				rotationHSpeed = 0;
			else
				rotationHSpeed -= rotationHSpeed / Mathf.Abs(rotationHSpeed) * rotationHDampen * Time.deltaTime;
		}
		// Vertical:
		if (movementVPressed == false)
		{
			if (Mathf.Abs(rotationVSpeed) < rotationVDampen * Time.deltaTime)
				rotationVSpeed = 0;
			else
			rotationVSpeed -= rotationVSpeed / Mathf.Abs(rotationVSpeed) * rotationVDampen * Time.deltaTime;
		}

		// Restrict the rotations:
		/*
		if (transform.forward.x > shipMaxRotationH && rotationHSpeed > 0)
			rotationHSpeed = 0;
		if (transform.forward.x < -shipMaxRotationH && rotationHSpeed < 0)
			rotationHSpeed = 0;

		if (transform.forward.y > shipMaxRotationV && rotationVSpeed < 0)
			rotationVSpeed = 0;
		if (transform.forward.y < -shipMaxRotationV && rotationVSpeed > 0)
			rotationVSpeed = 0;
		*/

		// Apply rotations:
		transform.Rotate (new Vector3(0, Time.deltaTime * rotationHSpeed, 0));
		transform.Rotate (new Vector3(Time.deltaTime * rotationVSpeed, 0, 0));
		
		transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);

		#endregion

		#region speed

		// Speed up, depending on state:
		if (state == StateTypes.InControl || state == StateTypes.OnLaunch)
		{
			movementSpeed += movementAcceleration * Time.deltaTime;
			if (movementSpeed > movementMaxSpeed)
				movementSpeed = movementMaxSpeed;
		}

		// Slow down if forward is not pushed:
		if (false)
		{
			if (movementSpeed < 0.5f)
			{
				movementSpeed = 0;
			}
			else
			{
				movementSpeed -= movementSpeed * movementAttenuation * Time.deltaTime;
			}
		}


		// Reduce additional speed vector, representing knokback from space rock:
		if (additionalSpeed.sqrMagnitude < additionalSpeedDampen * additionalSpeedDampen * Time.deltaTime)
		{
			additionalSpeed = Vector3.zero;
		}
		else
		{
			additionalSpeed -= additionalSpeed.normalized * additionalSpeedDampen * Time.deltaTime;
		}

		// Apply calculated speed to rigidbody:
		GetComponent<Rigidbody>().velocity = transform.forward * movementSpeed;
		
		GetComponent<Rigidbody>().velocity += additionalSpeed;
		
		//GetComponent<Rigidbody>().AddForce (transform.forward * movementAcceleration * Time.deltaTime);

		#endregion

		#region Camera

		cameraRotation += Input.GetAxis("Mouse X") * Time.deltaTime * cameraXMultiplier;

		camera.transform.Rotate (new Vector3(-Input.GetAxis("Mouse Y") * Time.deltaTime * cameraXMultiplier, 0, 0));
		camera.transform.Rotate (new Vector3(0, Input.GetAxis("Mouse X") * Time.deltaTime * cameraXMultiplier, 0));

		//rotationVector = Quaternion.AngleAxis(cameraRotation, Vector3.up);

		camera.transform.rotation = Quaternion.LookRotation(camera.transform.forward, Vector3.up);
		//camera.transform.rotation = Quaternion.LookRotation(camera.transform.forward, transform.up);

		//if (cameraRotation > Mathf.PI)
		//	cameraRotation -= Mathf.PI;
		//if (cameraRotation < -1)
			//cameraRotation -= 1;


		//rotationVector.y = cameraRotation;
		//GetComponent<Rigidbody>().rotation = rotationVector;

		//GetComponent<Rigidbody> ().rotation = Quaternion.AngleAxis(shipRotation, Vector3.up);

		#endregion
	}
}
