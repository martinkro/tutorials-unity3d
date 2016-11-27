using UnityEngine;
using System.Collections;

public class Reticle : MonoBehaviour {
	
	public Texture reticle;
	public float width;
	public float height;
	
	void OnGUI () {
		if (Time.time != 0 && Time.timeScale != 0)
			GUI.DrawTexture(new Rect(Screen.width/2, Screen.height/2, width, height), reticle);

	}
}
