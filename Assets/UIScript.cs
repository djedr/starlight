using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIScript : MonoBehaviour {

	public Text text;
	public Text autopilotText;
	public Light emergencyLight;
	public Light autopilotLight;
	public GameObject camera;
	public PlayerScript playerScript;
	public float emergencyTimeout = 0.8f;
	public float autopilotTimeout = 0.8f;

	private float currentEmergencyTimeout = 0.0f;
	private float currentAutopilotTimeout = 0.0f;

	private Color[] colors = {
		new Color(0.9f, 0.2f, 0.185f),
		new Color(0.5f, 0.9f, 0.485f), 
		new Color(0.5f, 0.485f, 0.95f), 
		new Color(0.9f, 0.55f, 0.2f) 
	};

	// Use this for initialization
	void Start () {
		playerScript = GetComponent<PlayerScript>();
	}
	
	// Update is called once per frame
	void Update () {
		string uiText = "...";
		Color color = colors[1];

		switch (playerScript.state) {
		case PlayerScript.StateTypes.OnStart:
			uiText = "PRZYGOTUJ SIĘ";
			autopilotLight.enabled = true;
			autopilotText.color = Color.blue;
			break;
		case PlayerScript.StateTypes.OnLaunch:
			uiText = "WYŁĄCZAM AUTOPILOT";
			autopilotLight.enabled = true;
			autopilotText.color = Color.blue;
			color = colors[3];
			break;
		case PlayerScript.StateTypes.InControl:
			uiText = "OMIJAJ ASTEROIDY";
			autopilotLight.enabled = false;
			autopilotText.color = Color.black;
			color = colors[3];
			break;
		case PlayerScript.StateTypes.LightSpeed:
			uiText = "PRĘDKOŚĆ WARP";
			color = colors[2];
			break;
		case PlayerScript.StateTypes.BeforeLightSpeed:
			uiText = "WŁĄCZAM NAPĘD WARP";
			autopilotLight.enabled = true;
			autopilotText.color = Color.blue;
			color = colors[3];
			break;	
		case PlayerScript.StateTypes.Bounced:
			uiText = "UWAGA!";
			//autopilotLight.enabled = true;
			//autopilotText.color = Color.blue;
			color = colors[0];
			if (currentEmergencyTimeout <= 0f) {
				currentEmergencyTimeout = emergencyTimeout;
				camera.GetComponent<CameraShake>().shake = 0.4f;
				emergencyLight.enabled = true;
			}
			break;
		case PlayerScript.StateTypes.Landing:
			uiText = "INICJUJĘ DOKOWANIE";
			autopilotLight.enabled = true;
			autopilotText.color = Color.blue;
			break;
		case PlayerScript.StateTypes.Landed:
			uiText = "STATEK ZADOKOWANY!";
			autopilotLight.enabled = true;
			autopilotText.color = Color.blue;
			autopilotLight.color = colors[1];
			break;
		default:
			uiText = playerScript.state.ToString();
			break;
		}

//		if (currentAutopilotTimeout <= 0f) {
//			currentAutopilotTimeout = autopilotTimeout;
//			autopilotLight.enabled = true;
//		}

		if (autopilotLight.enabled) {
			if (Mathf.Repeat(Time.time, 0.5f) > 0.25f) autopilotText.color = Color.blue;
			else autopilotText.color = Color.black;
			//if (autopilotLight.intensity <= 0f) autopilotLight.intensity = 8f;
		}

		text.text = uiText;
		text.color = color;

		if (currentEmergencyTimeout > 0f) {
			float oldEmergencyTimeout = currentEmergencyTimeout;
			currentEmergencyTimeout -= Time.deltaTime;
			emergencyLight.intensity -= 1f;
			if (emergencyLight.intensity <= 0f) emergencyLight.intensity = 8f;

		} else {
			emergencyLight.enabled = false;
		}
	}
}
