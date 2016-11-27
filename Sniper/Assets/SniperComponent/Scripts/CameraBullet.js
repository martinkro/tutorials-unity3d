var Bullet:Transform;
var Target:GameObject;

function SetUp(victim:GameObject,bullet:Transform,hitpoint:Vector3){
	if(victim){
		Bullet = bullet;
		Target = victim;
		transform.position = victim.transform.position + new Vector3(Random.Range(-25,25),Random.Range(2,5),Random.Range(-25,25));
		transform.LookAt(Target.transform.position);
	}
}
function View(victim:GameObject,bullet:Transform,hitpoint:Vector3){
	if(victim){
		Target = victim;
		transform.LookAt(Target.transform.position);
	}
}

function Update(){
	if(Target)
		transform.LookAt(Target.transform.position);
	
	//if(Bullet)
		//transform.LookAt(Bullet.transform.position);
	
}
