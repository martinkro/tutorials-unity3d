using UnityEngine;
using System.Collections;

public class HealActive : MonoBehaviour {

	public int HPFill = 100;
	
	void Start () {
	
	}
	
	void OnTriggerEnter(Collider other) {
        var air = other.collider.gameObject.GetComponent<DamageManager>();
		if(air){
			air.HP += HPFill;	
		}
    }
}
