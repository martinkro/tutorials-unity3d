
private var vRandomRotation;
var rotationSpeed = 3;

public var rand : boolean = true;
private var randMultiplier : int;
public var minMultiplier : int = 1;
public var maxMultiplier : int = 3;
private var health : int;
public var explosion : GameObject;
public var makeChildren : boolean = true;
public var clone : GameObject;

public var points : int = 100;


function Start () {
	vRandomRotation = Vector3 (Random.Range(-rotationSpeed, rotationSpeed), Random.Range(-rotationSpeed, rotationSpeed), Random.Range(-rotationSpeed, rotationSpeed));
	gameObject.tag = "Respawn";
	if (rand) {
		randMultiplier = Random.Range(minMultiplier, maxMultiplier);
	}
	transform.localScale = Vector3(randMultiplier, randMultiplier, randMultiplier);
	
	health = 10 * randMultiplier;
}

function Update () {
	transform.Rotate(vRandomRotation * Time.deltaTime);
	
	if (health <= 0) {
		Die ();
	}
}

function ApplyDamage (damage) {
	health -= damage;
}

function Die () {
	if (explosion) {
		Instantiate (explosion, transform.position, transform.rotation);
	}
	
	if (makeChildren) {
		var clones : int;
		if(randMultiplier == maxMultiplier) {
			clones = 3;
		} else if (randMultiplier == ((minMultiplier + maxMultiplier) / 2)) {
			clones = 2;
		} else if (randMultiplier == minMultiplier) {
			clones = 1;
		}

		while (clones > 0) {
			clones -= 1;
			
			var randPosPlus = Vector3(Random.Range(-2, 2),Random.Range(-2, 2),Random.Range(-2, 2));
			
			var clone = Instantiate (clone, transform.position + randPosPlus, transform.rotation);
			clone.name = "asteroid clone";
			var cloneAstScr = clone.GetComponent("asteroid");
			cloneAstScr.rand = false;
			cloneAstScr.randMultiplier = 1;
			cloneAstScr.makeChildren = false;
			
			var colliders : Collider[] = Physics.OverlapSphere(transform.position, 3);
			for (var hit in colliders)
			{
				if (hit.rigidbody)
				{
					hit.rigidbody.AddExplosionForce(100, transform.position, 5);
				}
			}
			print("Создание клона астероида");
		}

	}
	if (clones <= 0) {
		var gameManagerObj : GameObject = GameObject.Find("GameManager");
		var localValues : LocalValues = gameManagerObj.GetComponent("LocalValues");
		localValues.points += points;
		Destroy(gameObject);
	}
}