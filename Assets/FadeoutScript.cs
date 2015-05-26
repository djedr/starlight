using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeoutScript : MonoBehaviour {

	public Image me;

	// Use this for initialization
	void Start () {
		me.color = new Color(0, 0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (me.color.a < 1.0f) {
			Color color = me.color;
			color.a += 0.5f * Time.deltaTime;
			me.color = color;
		}
	}
}
