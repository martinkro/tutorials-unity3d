public var healthAdd = 10;

function Update () {
}

function OnCollisionEnter (collision : Collision) {
	var explosionPosition = transform.position;
	var colliders : Collider[] = Physics.OverlapSphere (explosionPosition, 3);
	
	for (var hit in colliders) {
		hit.SendMessageUpwards("AddHealth", healthAdd, SendMessageOptions.DontRequireReceiver);
	}
	
	Destroy(gameObject);
}