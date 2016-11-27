

public var Bullets:GameObject; // Bullet prefeb
public var Shell:GameObject; // Shell prefeb
public var BulletSpawn:Transform; // position of bullet spawn
public var ShellSpawn:Transform; // position of shell spawn
public var cameras : GameObject[];// camera list using for swith between normal view and zooming view
public var fireRate:float = 0.2f; // rate of fire 

public var SoundGunFire:AudioClip;
public var SoundBoltEnd:AudioClip;
public var SoundBoltStart:AudioClip;
public var Force:int = 10000;

private var boltout:boolean;
private var indexcamera:int;
private var timefire:float = 0;
private var gunState:int = 0;
private var ammoin:int = 1;


function DisableCamera () {
    for (var i : int=0 ;i<cameras.length; i++) {
    	if(cameras[i].camera)
        cameras[i].camera.gameObject.SetActive(false);
    }
}


function SelectCamera (index : int) {
    for (var i : int=0 ;i<cameras.length; i++) {
        if (i == index){
            cameras[i].camera.gameObject.SetActive(true);
       		 }else{
        	cameras[i].camera.gameObject.SetActive(false);
    	}
    }
}


function Start () {
	SelectCamera(0);
}

function Update () {

	if(ammoin<=0){
		if(indexcamera == 1){
			GameObject.Find("Howto2").guiText.text = "Right Click to Reload";
		}
	}else{
		GameObject.Find("Howto2").guiText.text = "";
	}
	switch(gunState){
		case 0:
			if(ammoin<=0){
			if(indexcamera != 1){
				animation.CrossFade("Bolt");
				gunState = 2;
				if(SoundBoltStart){
					AudioSource.PlayClipAtPoint(SoundBoltStart,this.transform.position);
				}
			}
			}
		
		break;
		case 1:
			if(animation["Shoot"].time >= animation["Shoot"].length*0.8f){
				gunState = 0;
			}
		break;
		case 2:
		
			if(animation["Bolt"].time >= animation["Bolt"].length*0.3f){
				if(Shell && ShellSpawn){
					if(!boltout){
    				var shell:GameObject = Instantiate(Shell,ShellSpawn.position, ShellSpawn.rotation);
    				shell.rigidbody.AddForce(transform.right*400);
    				shell.rigidbody.AddRelativeTorque(Vector3.up * 1000);
    				GameObject.Destroy(shell,5);
    				boltout = true;
    				}
    			}
    		}
    		
			if(animation["Bolt"].time >= animation["Bolt"].length*0.8f){
				gunState = 0;
				ammoin = 1;
				
				animation.CrossFade("Idle");
				if(SoundBoltEnd){
					AudioSource.PlayClipAtPoint(SoundBoltEnd,this.transform.position);
				}
			}
		break;
	}
	
	if(transform.parent.GetComponent(MouseLooking)){
	if(indexcamera == 1){
		transform.parent.GetComponent(MouseLooking).sensitivityX = 0.5;
		transform.parent.GetComponent(MouseLooking).Noise = true;
	}else{
		transform.parent.GetComponent(MouseLooking).sensitivityX = 3;
		transform.parent.GetComponent(MouseLooking).Noise = false;
	}
	}
	
}


function Zoom(){
	if(indexcamera!=1){
		indexcamera = 1;
	}else{
		indexcamera = 0;
		gunState = 0;
	}
	SelectCamera(indexcamera);
}


function Shoot(){
	if(timefire + fireRate < Time.time){
		if(gunState==0){
			if(ammoin>0){
			
				if(transform.parent.GetComponent(MouseLooking))
				transform.parent.GetComponent(MouseLooking).Stun(10);
				if(SoundGunFire){
					AudioSource.PlayClipAtPoint(SoundGunFire,this.transform.position);
				}
				
				if(Bullets && BulletSpawn){
					var bullet:GameObject = Instantiate(Bullets,BulletSpawn.position, BulletSpawn.rotation);
					bullet.GetComponent(Bullet).SetUp(this.transform.parent.gameObject);
    				bullet.rigidbody.AddForce(transform.forward*Force);
    				
    			}
    			
    			boltout = false;
				animation.Play("Shoot");
				timefire = Time.time;
				gunState = 1;
				ammoin-=1;
				
			}
		}
	}
}