using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIScript : MonoBehaviour {
	
	public Text text;
	public Text autopilotText;
	public Image crosshairImage;
	public Light emergencyLight;
	public Light autopilotLight;
	public GameObject camera;
	public PlayerScript playerScript;
	public float emergencyTimeout = 0.8f;
	public float autopilotTimeout = 0.8f;

	public AudioSource alarmSound;
	public AudioSource targetSound;
	
	private float currentEmergencyTimeout = 0.0f;
	private float currentAutopilotTimeout = 0.0f;

	private Sprite crosshairNormalSprite;
	public Sprite crosshairAimedSprite;

	private bool startClosingCaptions = false;
	private float closingCaptionsTimer = 0f;

	private bool targetChanged = true;
	
	private Color[] colors = {
		new Color(0.9f, 0.2f, 0.185f),
		new Color(0.5f, 0.9f, 0.485f), 
		new Color(0.5f, 0.485f, 0.95f), 
		new Color(0.9f, 0.55f, 0.2f),
		new Color(0.9f, 0.9f, 0.9f),
		new Color(0.9f, 0.1f, 0.085f, 0.5f),
		//new Color(0.5f, 0.9f, 0.485f, 0.33f)
		new Color(1f, 1f, 1f, 0.33f)
	};
	
	// Use this for initialization
	void Start () {
		playerScript = GetComponent<PlayerScript>();

		crosshairNormalSprite = crosshairImage.sprite;
		//crosshairAimedSprite = Resources.Load<Sprite>("crosshair2");
	}
	
	// Update is called once per frame
	void Update () {
		string uiText = "...";
		Color color = colors[1];
		
		switch (playerScript.state) {
		case PlayerScript.StateTypes.BeforeStart:
			uiText = "WCIŚNIJ PRZYCISK JOYSTICKA";
			autopilotLight.enabled = true;
			autopilotText.color = Color.blue;
			break;
		case PlayerScript.StateTypes.OnStart:
			uiText = "STARTUJĘ";
			autopilotLight.enabled = true;
			autopilotText.color = Color.blue;
			break;
		case PlayerScript.StateTypes.OnLaunch:
			uiText = "PRZYGOTUJ SIĘ";
			autopilotLight.enabled = true;
			autopilotText.color = Color.blue;
			color = colors[3];
			break;
		case PlayerScript.StateTypes.InControl:
			uiText = "OMIJAJ LUB ZESTRZELAJ ASTEROIDY";
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
				camera.GetComponent<CameraShake>().shakeAmount = 0.1f;
				emergencyLight.enabled = true;

				alarmSound.Play();
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

			startClosingCaptions = true;
			break;
		default:
			uiText = playerScript.state.ToString();
			break;
		}
		
		//		if (currentAutopilotTimeout <= 0f) {
		//			currentAutopilotTimeout = autopilotTimeout;
		//			autopilotLight.enabled = true;
		//		}

		if (playerScript.tooFar != Vector3.zero) {
			autopilotLight.enabled = true;
			autopilotText.color = Color.blue;
		}
		
		if (startClosingCaptions) {
			closingCaptionsTimer += Time.deltaTime;
			
			autopilotText.color = Color.black;
			autopilotLight.enabled = false;

			if (closingCaptionsTimer > 18f) {
				uiText = " ";
			} else if (closingCaptionsTimer > 14f) {
				uiText = "NASA, freesfx.co.uk, freesound.org";
				color = colors[4];
			} else if (closingCaptionsTimer > 11f) {
				uiText = "Dźwięki dzięki uprzejmości";
				color = colors[4];
			} else if (closingCaptionsTimer > 8f) {
				uiText = " ";
			} else if (closingCaptionsTimer > 5f) {
				uiText = "MISJA ZAKOŃCZONA POWODZENIEM!";
			} else if (closingCaptionsTimer > 2.5f) {
				uiText = "GRATULACJE!";
			}
		}
		
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

		if (playerScript.targetedRock != null) {
			crosshairImage.sprite = crosshairAimedSprite;
			crosshairImage.color = colors[5];
			if (targetChanged) {
				targetSound.Play();
				targetChanged = false;
			}
		} else {
			crosshairImage.sprite = crosshairNormalSprite;
			crosshairImage.color = colors[6];
			targetChanged = true;
		}
	}
}
