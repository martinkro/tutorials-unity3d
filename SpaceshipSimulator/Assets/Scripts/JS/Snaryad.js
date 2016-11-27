//Ставим взрыв
var explosion : GameObject;
var timeOut = 3.0; //Время жизни снаряда
var damage = 5;
var explosionRadius = 1;
var explosionPower = 10.0;


//Киляем если прошло слишком много времени
function Start () {
	Invoke("Destroy_p", timeOut);
	Physics.IgnoreCollision(GameObject.FindWithTag("Player").collider, collider);
}

//Проверяем на столкновение
function OnCollisionEnter (collision : Collision) {
	if (collision.transform.tag != "Player") {
		// Instantiate explosion at the impact point and rotate the explosion
		// so that the y-axis faces along the surface normal
		var contact : ContactPoint = collision.contacts[0];
		var rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
		Instantiate (explosion, contact.point, rotation); //Делаем вид взрыва

		var explosionPosition = transform.position;
		var colliders : Collider[] = Physics.OverlapSphere (explosionPosition, explosionRadius);

		for (var hit in colliders) {
			// Calculate distance from the explosion position to the closest point on the collider
			var closestPoint = hit.ClosestPointOnBounds(explosionPosition);
			var distance = Vector3.Distance(closestPoint, explosionPosition);

			// The hit points we apply fall decrease with distance from the explosion point
			var hitPoints = 1.0 - Mathf.Clamp01(distance / explosionRadius);
			hitPoints *= damage;

			// Tell the rigidbody or any other script attached to the hit object how much damage is to be applied!
			hit.SendMessageUpwards("ApplyDamage", hitPoints, SendMessageOptions.DontRequireReceiver);
		}
		
		Destroy_p ();    
	}
}

function Destroy_p () {
	// Stop emitting particles in any children
	var emitter : ParticleEmitter = GetComponentInChildren(ParticleEmitter);
	if (emitter) {
		emitter.emit = false;
	}

	// Detach children - We do this to detach the trail rendererer which should be set up to auto destruct
	transform.DetachChildren();
	
	Destroy(gameObject);
}


@script RequireComponent (Rigidbody)