var hitsound: AudioClip[];
var body:GameObject;
var DamageMult:int = 1;
function Start(){

}

function ApplyDamage (damage : float, velosity:Vector3) {
	if(hitsound.Length>0){
	 	AudioSource.PlayClipAtPoint(hitsound[Random.Range(0,hitsound.length)], transform.position);
	}
	if(body){
		if(body.GetComponent(Status)){
			body.GetComponent(Status).ApplyDamage(damage*DamageMult,velosity*DamageMult);
		}
	}
}



