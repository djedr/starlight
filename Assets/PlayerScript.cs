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
	public float lightSpeedMultiplier = 10;

	public float rotationHSpeed = 0;
	public float rotationVSpeed = 0;
	public float rotationHAcceleration = 20;
	public float rotationVAcceleration = 20;
	public float rotationHDampen = 20;
	public float rotationVDampen = 20;
	public float rotationHMaxSpeed = 40;
	public float rotationVMaxSpeed = 40;
	public float rotationPreLightSpeedMultiplier = 10;
	public float shipRotation = 0;

	public float shipMaxRotationH = 0.5f;
	public float shipMaxRotationHMult = 2;
	public float shipMaxRotationV = 0.4f;
	public float shipMaxRotationVMult = 2;

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

	private int enteringLightSpeed = 1;

	private GameObject levelGenerator;

	public GameObject camera = null;

	public Vector3 shipForward;

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

		if (state != StateTypes.BeforeLightSpeed && state != StateTypes.LightSpeed)
		{
			state = StateTypes.Bounced;
			timer = stunTime;
		}

		movementSpeed = movementSpeed * dot * 1f;
		//movementSpeed = movementSpeed / 2f;
	}


	public void EnterLightSpeedMode()
	{
		state = StateTypes.BeforeLightSpeed;
		timer = 0;

		enteringLightSpeed = 1;
	}

	// Docking is just like entering lightSpeed, only one var informs scripts to slow down instead of accelerating:
	public void EnterDockingMode()
	{
		state = StateTypes.BeforeLightSpeed;
		timer = 0;
		
		enteringLightSpeed = 0;
	}


	public void LeaveLightSpeedMode()
	{
		state = StateTypes.InControl;


		movementMaxSpeed /= lightSpeedMultiplier;
		movementAcceleration /= lightSpeedMultiplier;
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
				else if (state == StateTypes.BeforeLightSpeed)
				{
					state = StateTypes.LightSpeed;

					if (enteringLightSpeed == 1)
					{
						movementMaxSpeed *= lightSpeedMultiplier;
						movementAcceleration *= lightSpeedMultiplier;
					}
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

		if (state == StateTypes.BeforeLightSpeed)
		{
			state = state;
			int i = 0;
			i++;
		}
	
		// Get directional input:
		Vector3 tooFar = levelGenerator.GetComponent<LevelGeneratorScript> ().OffRoad(transform.position);

		//  && transform.forward.x < shipMaxRotationH
		if ( (Input.GetKey(KeyCode.D) && tooFar.x != 1 && state == StateTypes.InControl) || (tooFar.x == -1 && transform.forward.x < 0.1f))
		{
			rotationHSpeed += rotationHAcceleration * Time.deltaTime;
			if (rotationHSpeed > rotationHMaxSpeed)
				rotationHSpeed = rotationHMaxSpeed;

			movementHPressed = true;
		}
		//  && transform.forward.x > -shipMaxRotationH
		else if ( (Input.GetKey(KeyCode.A) && tooFar.x != -1 && state == StateTypes.InControl) || (tooFar.x == 1 && transform.forward.x > -0.1f))
		{
			rotationHSpeed -= rotationHAcceleration * Time.deltaTime;
			if (rotationHSpeed < -rotationHMaxSpeed)
				rotationHSpeed = -rotationHMaxSpeed;

			movementHPressed = true;
		}

		//  && transform.forward.y > -shipMaxRotationV
		if ( (Input.GetKey(KeyCode.W) && tooFar.y != -1 && state == StateTypes.InControl) || (tooFar.y == 1 && transform.forward.y > -0.1f))
		{
			rotationVSpeed += rotationVAcceleration * Time.deltaTime;
			if (rotationVSpeed > rotationVMaxSpeed)
				rotationVSpeed = rotationVMaxSpeed;

			movementVPressed = true;
		}
		//  && transform.forward.y < shipMaxRotationV
		else if ( (Input.GetKey(KeyCode.S) && tooFar.y != 1 && state == StateTypes.InControl) || (tooFar.y == -1 && transform.forward.y < 0.1f))
		{
			rotationVSpeed -= rotationVAcceleration * Time.deltaTime;
			if (rotationVSpeed < -rotationVMaxSpeed)
				rotationVSpeed = -rotationVMaxSpeed;

			movementVPressed = true;
		}


		// If you're about to enter lightspeed, rotate to right position:
		if (tooFar == Vector3.zero && (state == StateTypes.LightSpeed || state == StateTypes.BeforeLightSpeed))
		{
			// Init help vars:
			Vector3 set = Vector3.zero;
			Vector3 targetSpeeds = Vector3.zero;

			targetSpeeds.x = - transform.forward.x * rotationPreLightSpeedMultiplier;
			targetSpeeds.y = transform.forward.y * rotationPreLightSpeedMultiplier;

			// Rotate in H axis:
			// If you're positioned horizontally, mark it:
			if (Mathf.Abs(transform.forward.x) < 0.005f)
			{
				set.x = 1;
				//transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
				rotationHSpeed = 0;
			}
			// If not, try to match your rotation speed to targetSpeeds:
			else if (Mathf.Abs(rotationHSpeed - targetSpeeds.x) < Time.deltaTime * rotationHAcceleration)
			{
				rotationHSpeed = targetSpeeds.x;
			}
			else if (rotationHSpeed < targetSpeeds.x)
			{
				rotationHSpeed += rotationHAcceleration * Time.deltaTime;
			}
			else
			{
				rotationHSpeed -= rotationHAcceleration * Time.deltaTime;
			}

			// Similarily in V axis:
			if (Mathf.Abs(transform.forward.y) < 0.005f)
			{
				set.y = 1;
				//transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
				rotationVSpeed = 0;
			}
			else if (Mathf.Abs(rotationVSpeed - targetSpeeds.y) < Time.deltaTime * rotationVAcceleration)
			{
				rotationVSpeed = targetSpeeds.y;
			}
			else if (rotationVSpeed < targetSpeeds.y)
			{
				rotationVSpeed += rotationVAcceleration * Time.deltaTime;
			}
			else
			{
				rotationVSpeed -= rotationVAcceleration * Time.deltaTime;
			}

			if (set.x == 1 && set.y == 1)
			{
				transform.rotation = Quaternion.AngleAxis(0, Vector3.up);

				if (timer <= 0 && state == StateTypes.BeforeLightSpeed)
					timer = 2;
			}


			movementHPressed = true;
			movementVPressed = true;
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

		shipForward = transform.forward;

		// Restrict the rotations:

		if (rotationHSpeed > (transform.forward.x - shipMaxRotationH) * -shipMaxRotationHMult && rotationHSpeed > 0)
			rotationHSpeed = (transform.forward.x - shipMaxRotationH) * -shipMaxRotationHMult;
		if (rotationHSpeed < (transform.forward.x + shipMaxRotationH) * -shipMaxRotationHMult && rotationHSpeed < 0)
			rotationHSpeed = (transform.forward.x + shipMaxRotationH) * -shipMaxRotationHMult;

		if (rotationVSpeed > (transform.forward.y + shipMaxRotationV) * shipMaxRotationVMult && rotationVSpeed > 0)
			rotationVSpeed = (transform.forward.y + shipMaxRotationV) * shipMaxRotationVMult;
		if (rotationVSpeed < (transform.forward.y - shipMaxRotationV) * shipMaxRotationVMult && rotationVSpeed < 0)
			rotationVSpeed = (transform.forward.y - shipMaxRotationV) * shipMaxRotationVMult;

		/*
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
		if (state == StateTypes.InControl || state == StateTypes.OnLaunch || state == StateTypes.LightSpeed || state == StateTypes.BeforeLightSpeed)
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
