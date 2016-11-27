var target : Transform;
function Update () {
		transform.rotation = Quaternion.Lerp (target.rotation, transform.rotation,Time.deltaTime * 70);
}