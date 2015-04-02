using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	public enum StateTypes
	{
		InControl,
		Bounced,
		OnStart
	};

	// player properties

	StateTypes state = StateTypes.OnStart;

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

	public float timer = 1;
	public float stunTime = 0.5f;
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

	public GameObject camera = null;

	// Use this for initialization
	void Start () {
		Screen.lockCursor = true;

		camera = GameObject.Find ("Main Camera");
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
				state = StateTypes.InControl;
			}
		}

		#endregion


		#region rotation

		// Variables to keep track if player is turning in horizontal and vertical plane this frame:
		movementHPressed = false;
		movementVPressed = false;
	
		// Get directional input:
		if (state == StateTypes.InControl)
		{
			if (Input.GetKey(KeyCode.D))
			{
				rotationHSpeed += rotationHAcceleration * Time.deltaTime;
				if (rotationHSpeed > rotationHMaxSpeed)
					rotationHSpeed = rotationHMaxSpeed;

				movementHPressed = true;

				//transform.Rotate (new Vector3(0, Time.deltaTime * rotationSpeed, 0));
			}
			else if (Input.GetKey(KeyCode.A))
			{
				rotationHSpeed -= rotationHAcceleration * Time.deltaTime;
				if (rotationHSpeed < -rotationHMaxSpeed)
					rotationHSpeed = -rotationHMaxSpeed;

				movementHPressed = true;

				//transform.Rotate (new Vector3(0, -Time.deltaTime * rotationSpeed, 0));
			}

			if (Input.GetKey(KeyCode.W))
			{
				rotationVSpeed += rotationVAcceleration * Time.deltaTime;
				if (rotationVSpeed > rotationVMaxSpeed)
					rotationVSpeed = rotationVMaxSpeed;

				movementVPressed = true;

				//transform.Rotate (new Vector3(Time.deltaTime * rotationSpeed, 0, 0));
			}
			else if (Input.GetKey(KeyCode.S))
			{
				rotationVSpeed -= rotationVAcceleration * Time.deltaTime;
				if (rotationVSpeed < -rotationVMaxSpeed)
					rotationVSpeed = -rotationVMaxSpeed;

				movementVPressed = true;

				//transform.Rotate (new Vector3(-Time.deltaTime * rotationSpeed, 0, 0));
			}
		}


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

		// Apply rotations:
		transform.Rotate (new Vector3(0, Time.deltaTime * rotationHSpeed, 0));
		transform.Rotate (new Vector3(Time.deltaTime * rotationVSpeed, 0, 0));
		
		transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);

		#endregion

		#region speed

		// Speed up, depending on state:
		if (state == StateTypes.InControl)
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
