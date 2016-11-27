
function Update () {
}

function OnGUI() {
	if (GUI.Button(Rect(10,10,120,30),"Start")) {
		Application.LoadLevel("myScene");
	}
}