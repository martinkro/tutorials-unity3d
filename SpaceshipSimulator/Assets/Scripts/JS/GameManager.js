public var asteroidsCount : int = 100;
private var leftAsteroids : int;
public var maxX : float = 200;
public var maxY : float = 200;
public var maxZ : float = 200;
public var asteroid : GameObject;
private var instComplete : boolean = false;

public var energyCluster : GameObject;
public var clustersCount : int = 15;
private var leftClusters : int;

public var dead : boolean = false;
private var windowDeadRect : Rect = Rect (50, 50, 190, 100);

public var localValues : LocalValues;


//Списки
public var astListPrefab : GameObject;
public var clustListPrefab : GameObject;
private var asteroids : GameObject;
private var clusters : GameObject;
private var instSt : boolean = false;


function Awake() {
	dead = false;
	leftAsteroids = asteroidsCount;
	leftClusters = clustersCount;
	instSt = false;
	instComplete = false;
}

function Update () {
	if(!instSt) {
		asteroids = Instantiate(astListPrefab,Vector3(0,0,0),transform.rotation);
		clusters = Instantiate(clustListPrefab,Vector3(0,0,0),transform.rotation);
		asteroids.name = "Asteroids";
		clusters.name = "Clusters";
		instSt = true;
	}
	if (!instComplete && instSt) {
		InstantiateObjects ();
	}
	
	if(dead) {
		GetComponent("Radar").enabled = false;
	}
}

function InstantiateObjects () {
	while (leftAsteroids > 0) {
		leftAsteroids -= 1;
		var asteroid_i = Instantiate(asteroid,Vector3(Random.Range(-maxX, maxX), Random.Range(-maxY, maxY), Random.Range(-maxZ, maxZ)),transform.rotation);
		asteroid_i.name = "asteroid"+leftAsteroids;
		var astTr = asteroid_i.transform; //Если этого не будет, то возникает ошибка, хз почему, лишний код только...
		astTr.parent = asteroids.transform;
	}
	
	while (leftClusters > 0) {
		leftClusters -= 1;
		var cluster_i : GameObject = Instantiate(energyCluster,Vector3(Random.Range(-maxX, maxX), Random.Range(-maxY, maxY), Random.Range(-maxZ, maxZ)),transform.rotation);
		cluster_i.name = "EnergyCluster"+leftClusters;
		var clustTr = cluster_i.transform; //Если этого не будет, то возникает ошибка, хз почему, лишний код только...
		clustTr.parent = clusters.transform;
	}
	
	if (leftAsteroids == 0 && leftClusters == 0) {
		instComplete = true;
	}
}

function OnGUI () {
	if (dead) {
		windowDeadRect = GUI.Window (0, windowDeadRect, DeadWindow, "Dead!");
	}
}	

function DeadWindow() {
	GUI.Label (Rect (10, 20, 100, 20), "Your points:" + localValues.points);
	if (GUI.Button (Rect (10,45,170,20), "Reload")) {
		Application.LoadLevel("myScene");
	}
}
