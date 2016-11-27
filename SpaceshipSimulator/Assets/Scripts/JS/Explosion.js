var explosionTime = 1;
var explosionRadius = 5;
var explosionPower = 2000;

function Start () {
	Destroy(gameObject, explosionTime);
	
	var colliders : Collider[] = Physics.OverlapSphere(transform.position, explosionRadius);
	for (var hit in colliders)
	{
		if (hit.rigidbody)
		{
			hit.rigidbody.AddExplosionForce(explosionPower, transform.position, explosionRadius);
		}
	}
}