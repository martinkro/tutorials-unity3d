	var o_control : Transform;
	var o_weapon : Transform;
	public var enabled1 = false;



	function Start() {
		enabled1 = false;
	}

	function Update () {
		ship_ctrl = o_control.GetComponent(ship_control_camera_v2);
		weapon_ctrl = o_weapon.GetComponent(WeaponController_v2);
			if (enabled1) {
				weapon_ctrl.enabled = false;
				ship_ctrl.enabled = false;
			} else {
				weapon_ctrl.enabled = true;
				ship_ctrl.enabled = true;
			}
	}
