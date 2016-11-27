public var points : int = 0;
function Update () {
}

function OnGUI () {
	GUI.Label (Rect (Screen.width - 170, 1, 170, 20), "Points:"+""+points);
}