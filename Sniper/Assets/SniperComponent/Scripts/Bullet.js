public var ParticleHit:GameObject;// particle when bullet hit
public var LookTargetHit:boolean; // if you don't want the action camera set to false
public var Owner:GameObject;// owner is player

private var objcameraLookat:GameObject;
private var hited:boolean;
private var raylength:int = 50;
private var cameraLook:boolean;
public var Damage:int = 100;

function HitTarget(collision:Collider){
	// When hit the target
	if(collision.gameObject.GetComponent(DamageManager)){
		collision.gameObject.GetComponent(DamageManager).ApplyDamage(Damage,this.transform.rigidbody.velocity);	
	}
}

function Start () {
	RayShoot();
}

function Update () {
	if(!hited){
		RayShoot();
		CameraLookAtTarget();
	}
	if(this.rigidbody)
	transform.forward = Vector3.Normalize(this.rigidbody.velocity);
}

function SetUp(owner:GameObject){
	Owner = owner;
}



function RayShoot(){

	var hits : RaycastHit[];
   	hits = Physics.RaycastAll (transform.position, transform.forward, 20);
   	
    for (var i = 0;i < hits.Length; i++) {
        var hit : RaycastHit = hits[i];
        if (hit.collider){

        if (hit.collider.tag!="Player" && hit.collider.tag!="Bullet"){
        	var hitparticle = Instantiate(ParticleHit,hit.point, hit.transform.rotation);
        	hitparticle.transform.rotation = Quaternion.FromToRotation (hitparticle.transform.up, transform.forward) * hitparticle.transform.rotation;
   			GameObject.Destroy(hitparticle,5);
   			
   			HitTarget(hit.collider);
   			if (hit.rigidbody){
				hit.rigidbody.AddForceAtPosition((100 + this.rigidbody.velocity.magnitude) * hit.normal, hit.point);
			}
			hited = true;
        }
        }
	}

   	if(hited){
   		GameObject.Destroy(this.gameObject);
   	}	
}


function CameraLookAtTarget(){
	
	var camerahits : RaycastHit;
    if (Physics.Raycast(transform.position, transform.forward,camerahits, 1000)) {
      	var hitcam : RaycastHit = camerahits;
        if (hitcam.collider){
        	if(hitcam.collider.gameObject.GetComponent(DamageManager)){
        		if(!cameraLook){
        			cameraLook = true;
        			if(GameObject.Find("CamaraBullet") && LookTargetHit){
        				if(Owner){
        					Owner.GetComponent(GunHanddle).SeeBulletRun();
        				}
        				objcameraLookat = hitcam.collider.gameObject;
        				GameObject.Find("CamaraBullet").GetComponent(CameraBullet).SetUp(objcameraLookat,this.transform,hitcam.point);
        			}
        		}
        	}
        }
    }
}
