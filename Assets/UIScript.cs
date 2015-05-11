using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIScript : MonoBehaviour {

	public Text text;
	public PlayerScript playerScript;

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
			break;
		case PlayerScript.StateTypes.OnLaunch:
			uiText = "WYŁĄCZAM AUTOPILOT";
			color = colors[3];
			break;
		case PlayerScript.StateTypes.InControl:
			uiText = "OMIJAJ ASTEROIDY";
			color = colors[3];
			break;
		case PlayerScript.StateTypes.LightSpeed:
			uiText = "PRĘDKOŚĆ WARP";
			color = colors[2];
			break;
		case PlayerScript.StateTypes.BeforeLightSpeed:
			uiText = "WŁĄCZAM NAPĘD WARP";
			color = colors[3];
			break;	
		case PlayerScript.StateTypes.Bounced:
			uiText = "UWAGA!";
			color = colors[0];
			break;
		default:
			uiText = playerScript.state.ToString();
			break;
		}

		text.text = uiText;
		text.color = color;
	}
}
