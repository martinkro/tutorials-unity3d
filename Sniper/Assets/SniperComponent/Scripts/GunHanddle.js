var Guns:GameObject;
function Start () {

}

function Update () {
	if(Input.GetMouseButtonDown(1)){
		Guns.gameObject.GetComponent(Gun).Zoom();
	}
	
	if(Input.GetMouseButtonDown(0)){
		Guns.gameObject.GetComponent(Gun).Shoot();
	}
	

}

function SeeBulletRun(){
	Guns.gameObject.GetComponent(Gun).DisableCamera();
	
}