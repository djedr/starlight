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
	private PlayerScript playerScript;
	public GameObject levelGenerator;
	private LevelGeneratorScript levelScript;
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

	public Text warning;
	
	private Color[] colors = {
		new Color(0.9f, 0.2f, 0.185f),
		new Color(0.5f, 0.9f, 0.485f), 
		new Color(0.8f, 0.785f, 0.95f), 
		new Color(0.9f, 0.55f, 0.2f),
		new Color(0.9f, 0.9f, 0.9f),
		new Color(0.85f, 0.05f, 0.035f, 0.4f),
		new Color(0.9f, 0.1f, 0.085f, 0.6f),
		//new Color(0.5f, 0.9f, 0.485f, 0.33f),
		//new Color(1f, 1f, 1f, 0.33f),
		new Color(0.3f, 1f, 0.4f)
	};
	
	// Use this for initialization
	void Start () {
		playerScript = GetComponent<PlayerScript>();
		levelScript = levelGenerator.GetComponent<LevelGeneratorScript> ();

		crosshairNormalSprite = crosshairImage.sprite;
		//crosshairAimedSprite = Resources.Load<Sprite>("crosshair2");
	}
	
	// Update is called once per frame
	void Update () {
		string uiText = "...";
		Color color = colors[1];

		
		warning.text = " ";
		
		switch (playerScript.state) {
		case PlayerScript.StateTypes.BeforeStart:
			uiText = "WCIŚNIJ PRZYCISK JOYSTICKA";
			autopilotLight.enabled = true;
			autopilotText.text = "AUTOPILOT";
			break;
		case PlayerScript.StateTypes.OnStart:
			uiText = "STARTUJĘ";
			autopilotLight.enabled = true;
			autopilotText.text = "AUTOPILOT";
			break;
		case PlayerScript.StateTypes.OnLaunch:
			uiText = "PRZYGOTUJ SIĘ NA POWRÓT DO WAHADŁOWCA";
			autopilotLight.enabled = true;
			autopilotText.text = "AUTOPILOT";
			color = colors[3];
			break;
		case PlayerScript.StateTypes.InControl:
			if (levelScript.generatedSecondPart == 3) {
				uiText = "LEĆ W STRONĘ MIGAJĄCEGO ŚWIATŁA";
			} else {
				uiText = "OMIJAJ ASTEROIDY LUB STRZELAJ W NIE";
			}
			autopilotLight.enabled = false;
			autopilotText.text = " ";
			color = colors[3];
			break;
		case PlayerScript.StateTypes.LightSpeed:
			uiText = "PRĘDKOŚĆ WARP";
			color = colors[2];
			break;
		case PlayerScript.StateTypes.BeforeLightSpeed:
			uiText = "WŁĄCZAM NAPĘD WARP";
			autopilotLight.enabled = true;
			autopilotText.text = "AUTOPILOT";
			color = colors[3];
			break;	
		case PlayerScript.StateTypes.Bounced:
			uiText = "UWAGA!";
			warning.text = "UWAGA!";
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
			uiText = "PROCEDURA DOKOWANIA";
			autopilotLight.enabled = true;
			autopilotText.text = "AUTOPILOT";
			break;
		case PlayerScript.StateTypes.Landed:
			uiText = "STATEK ZADOKOWANY!";
			autopilotLight.enabled = true;
			autopilotText.text = "AUTOPILOT";
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
			warning.text = "UWAGA!";
			autopilotLight.enabled = true;
			autopilotText.text = "AUTOPILOT";
		}
		
		if (startClosingCaptions) {
			closingCaptionsTimer += Time.deltaTime;
			
			autopilotText.text = " ";
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
			if (Mathf.Repeat(Time.time, 0.5f) > 0.25f) autopilotText.text = "AUTOPILOT";
			else autopilotText.text = " ";
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
			crosshairImage.color = colors[6];
			if (targetChanged) {
				targetSound.Play();
				targetChanged = false;
			}
		} else {
			crosshairImage.sprite = crosshairNormalSprite;
			crosshairImage.color = colors[5];
			targetChanged = true;
		}
	}
}
