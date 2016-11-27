public var maxHealth : int = 100;
var curHealth : int;
public var dischargeSpeed : float = 1;
public var hitParticles : GameObject;
public var hitSound : AudioClip;
public var explosion : GameObject;
public var timeOut : float;
public var deathHandler : GameObject;

private var gameManagerObj : GameObject;
private var gameManager : GameManager;

var mainCamera : GameObject;
var camera_control : ship_control_camera_v2;
function Awake() {
	curHealth = maxHealth;
	gameManagerObj = GameObject.Find("GameManager");
	gameManager = gameManagerObj.GetComponent("GameManager");
	mainCamera = GameObject.FindWithTag("MainCamera");
	camera_control = mainCamera.GetComponent("ship_control_camera_v2");
}

function Update () {
	if(curHealth > 0) {
		if(curHealth > maxHealth) {
			curHealth = maxHealth;
		}
	} else { 
		Die();
		camera_control.enabled = false;
	}
}

function ApplyDamage (damage) {
	curHealth -= damage;
}

function AddHealth (heal) {
	curHealth += heal;
}

function Die() {
	if (explosion) {
		Instantiate(explosion, transform.position, transform.rotation);
	}
	
	if (deathHandler) {
		Instantiate(deathHandler, transform.position, transform.rotation);
	}

	
	gameManager.dead = true;
	Destroy(gameObject);
}

function OnGUI() {
	GUI.Label (Rect (10, 1, 170, 20), "Heatlh:"+""+curHealth+"/"+maxHealth);
}