@script AddComponentMenu ("Camera-Control/Mouse Look")

public var sensitivityX : float = 15;
public var sensitivityY : float = 15;
public var minimumX : float = -360;
public var maximumX : float = 360;
public var minimumY : float = -60;
public var maximumY : float = 60;
public var delayMouse :float = 3;
public var noiseX:float = 0.1f;
public var noiseY:float = 0.1f;
public var breathHolderVal:float = 1;
public var Noise:boolean;

private var rotationX : float = 0;
private var rotationY : float = 0;
private var rotationXtemp : float = 0;
private var rotationYtemp : float = 0;
private var originalRotation : Quaternion;
private var noisedeltaX:float;
private var noisedeltaY:float;
private var stunY:float;


function Update () {
		sensitivityY = sensitivityX;
		Screen.lockCursor = true;
    
    	stunY+= (0-stunY)/20f;
    	
    	if(Noise){
        	noisedeltaX += ((((Mathf.Cos(Time.time) * Random.Range(-10,10)/5f) * noiseX) - noisedeltaX)/100)*breathHolderVal;
        	noisedeltaY += ((((Mathf.Sin(Time.time) * Random.Range(-10,10)/5f) * noiseY) - noisedeltaY)/100)*breathHolderVal;
		}else{
			
			noisedeltaX = 0;
			noisedeltaY = 0;
		}
		
		rotationXtemp += (Input.GetAxis("Mouse X") * sensitivityX) + noisedeltaX;
		rotationYtemp += (Input.GetAxis("Mouse Y") * sensitivityY) + noisedeltaY;
        rotationX += (rotationXtemp - rotationX)/delayMouse;
       
         if(rotationX>=360){
        	rotationX = 0;
        	rotationXtemp = 0;
        }
         if(rotationX<=-360){
        	rotationX = 0;
        	rotationXtemp = 0;
        }

        rotationY += (rotationYtemp - rotationY)/delayMouse;


        rotationX = ClampAngle (rotationX, minimumX, maximumX);
        rotationY = ClampAngle (rotationY, minimumY, maximumY);

        

        var xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
        var yQuaternion = Quaternion.AngleAxis (rotationY+stunY, Vector3.left);

        transform.localRotation = originalRotation * xQuaternion * yQuaternion;

}
function Stun(val:float){
	stunY = val;
}
 

function Start () {
    if (rigidbody)
        rigidbody.freezeRotation = true;
    originalRotation = transform.localRotation;

}

 

static function ClampAngle (angle : float, min : float, max : float) : float {

    if (angle < -360.0)
        angle += 360.0;

    if (angle > 360.0)
        angle -= 360.0;

    return Mathf.Clamp (angle, min, max);

}