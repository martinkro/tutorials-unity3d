using UnityEngine;
using System.Collections;

public class TouchSphere : MonoBehaviour {

	private int count;
	private Vector3 touchposition;
	private GameObject particle;
	public GameObject prefab;
	private float deltay;
	private int fingerCount; 

	// Use this for initialization
	void Start () {
		count = 0;
		deltay = 0.0f;
		fingerCount = 0;
		touchposition = new Vector3(0,0,0);
		particle = GameObject.Find ("Sphere");
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.touchCount > 0) {
			print (Input.touchCount);
			count += Input.touchCount;
			if (Input.touchCount > 2 && Input.GetTouch(0).phase == TouchPhase.Moved) {
				touchposition = Input.GetTouch(0).deltaPosition;
				deltay = Input.GetTouch(0).deltaPosition.y;
				//touchposition.x*0.01, touchposition.y*0.01, (float)0
				//float fx = touchposition.x*0.01;
				particle.transform.Translate(touchposition.x*0.01f, touchposition.y*0.01f, 0.0f); //移动距离
			}

			int finger = 0;
			int cnt = Input.touches.Length;
			for (int i = 0; i < cnt; i++) {
				Touch touch = Input.touches[i];
				if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
					finger++;
			}
			fingerCount = finger;
		}
	}

	void OnGUI() {
		GUI.Label(new Rect(10, 10, Screen.width, Screen.height), "count:" + count.ToString ());
		GUI.Label(new Rect(10, 30, Screen.width, Screen.height), touchposition.ToString());
		GUI.Label(new Rect(10, 50, Screen.width, Screen.height), "delta y:"+deltay.ToString());
		GUI.Label(new Rect(10, 70, Screen.width, Screen.height), "finger:"+fingerCount.ToString());
		GUI.Label(new Rect(10, 90, Screen.width, Screen.height), "timeScale:"+Time.timeScale.ToString());
	}
}
