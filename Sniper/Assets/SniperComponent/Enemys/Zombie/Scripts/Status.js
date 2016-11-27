var deadbody:GameObject;
var hitsound: AudioClip[];
var hp:int = 100;
private var velositydamage:Vector3;


function ApplyDamage (damage : float, velosity:Vector3) {
	if(hp<=0){
		return;
	}
	hp -= damage;
	
	velositydamage = velosity;
 	if(hp<=0){
 		Dead();
 	}
}


function Dead(){
	if(hitsound.Length>0){
	 	AudioSource.PlayClipAtPoint(hitsound[Random.Range(0,hitsound.length)], transform.position);
	}
	if(deadbody){
		var obj:GameObject = Instantiate(deadbody,this.transform.position,this.transform.rotation);
		CopyTransformsRecurse(this.transform, obj);
		Destroy(obj,5);
	}
	Destroy(this.gameObject);
}

function CopyTransformsRecurse (src : Transform,  dst : GameObject) {
	dst.transform.position = src.position;
	dst.transform.rotation = src.rotation;
	dst.transform.localScale = src.localScale;
	if(dst.rigidbody)
	dst.rigidbody.velocity = velositydamage / 20f;
	for (var child : Transform in dst.transform) {
		var curSrc = src.Find(child.name);
		if (curSrc){
			CopyTransformsRecurse(curSrc, child.gameObject);
		}
	}
}
