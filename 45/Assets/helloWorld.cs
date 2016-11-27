#define DEBUG //

using UnityEngine;
using System.Collections;

public class helloWorld : MonoBehaviour {

	// Use this for initialization
	void Start () {
#if TEST
		print("");
#elif DEBUG
		print("print DEBUG");
#else
		print("ELSE");
#endif

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		//GUI.skin.label.fontSize = 100;
		//GUI.Label (new Rect(10, 10, Screen.width, Screen.height), "EDG");
	}
}
