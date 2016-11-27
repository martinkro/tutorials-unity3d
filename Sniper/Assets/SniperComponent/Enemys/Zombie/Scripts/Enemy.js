var Myself : Transform;
var Speed = 3;
var footstepSound: AudioClip[];
var targetPosition:Vector3;
var timethink:int = 0;

function Start () {
	yield WaitForSeconds(Random.Range(0, 3.0));
	Myself.animation.CrossFade("Run", 0.3f);
}


function Update (){

	Myself.animation.CrossFade("Run", 0.3f);
	
	if(timethink<=0){
   		targetPosition = new Vector3(Random.Range(-200,200),0,Random.Range(-200,200));
   		timethink = Random.Range(100,500);
   	}else{
   		timethink-=1;
   	}
   	
   	targetPosition.y = transform.position.y;
   	transform.LookAt(targetPosition);
   	transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * Speed);

	
}

