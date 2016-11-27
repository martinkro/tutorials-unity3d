#pragma strict
var Objectman : Transform;
private var timetemp = 0;
var timeSpawn : float = 3;
var enemyCount = 0;
var radiun :int;


function Start () {
if(renderer)
  renderer.enabled = false;
timetemp = Time.time;
}

function Update () {
   var gos : GameObject[];
   gos = GameObject.FindGameObjectsWithTag("Enemy");

   if(gos.length < enemyCount){
  
   if(Time.time > timetemp+timeSpawn){
   	  timetemp = Time.time;
   	  var enemyCreated = Instantiate(Objectman, transform.position+ new Vector3(Random.Range(-radiun,radiun),this.transform.position.y,Random.Range(-radiun,radiun)), Quaternion.identity);
	 	 
   	  }
   }
}
