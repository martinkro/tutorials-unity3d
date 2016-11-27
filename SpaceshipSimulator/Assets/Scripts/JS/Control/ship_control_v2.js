/////////////////////////////////////////////////////////////////////
//Настройки корабля
@HideInInspector
public var curShipSpeed : float = 0.0; //Текущая скорость (задаваемая скриптом)
public var step = 1; //Шаг скорости
@HideInInspector
public var current_speed = 0; //Текущая скорость (задаваемая игроком)
public var maxSpeed : int = 10; //Максимальное количество скоростей (задаваемые игроком)
//////////////////////////////////////////////////////////////////////
var particles : GameObject;

function Awake() {
	gameObject.tag = "Player";
}

function Update () {
	if ((Input.GetKeyDown("up") || Input.GetKeyDown("w")) && (current_speed < maxSpeed)) {
		current_speed += step; //Увеличиваем тягу
	} else if ((Input.GetKeyDown("down") || Input.GetKeyDown("s"))  && (current_speed > 0)) {
		current_speed -= step; //Уменьшаем тягу
	}

	curShipSpeed = Mathf.Lerp(curShipSpeed, current_speed, Time.deltaTime * step);
	transform.position += transform.TransformDirection(Vector3.forward) * curShipSpeed * Time.deltaTime;
	
}

function OnCollisionEnter (collision : Collision) {
	if (collision.transform.tag != "GameController") {
		// Instantiate explosion at the impact point and rotate the explosion
		// so that the y-axis faces along the surface normal
		var contact : ContactPoint = collision.contacts[0];
		var rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
		Instantiate (particles, contact.point, rotation); //Делаем вид взрыва

		var explosionPosition = transform.position;
		var colliders : Collider[] = Physics.OverlapSphere (explosionPosition, 1);

		for (var hit in colliders) {
			// Calculate distance from the explosion position to the closest point on the collider
			var closestPoint = hit.ClosestPointOnBounds(explosionPosition);
			var distance = Vector3.Distance(closestPoint, explosionPosition);

			// The hit points we apply fall decrease with distance from the explosion point
			var hitPoints = 1.0 - Mathf.Clamp01(distance / 1);
			hitPoints *= 10;

			// Tell the rigidbody or any other script attached to the hit object how much damage is to be applied!
			if(collision.transform.tag != "Finish") {
				hit.SendMessageUpwards("ApplyDamage", hitPoints, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}