/// <summary>
/// Flash light. this script will make a flash of point light
/// </summary> 
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]

public class FlashLight : MonoBehaviour {
	
	public float LightMult = 2;
	void Update () {
		if(!this.light)
			return;
		
		this.light.intensity -= LightMult*Time.deltaTime;
	}
}
