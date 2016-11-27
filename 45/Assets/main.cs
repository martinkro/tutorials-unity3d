using UnityEngine;
using System.Collections;

public class main : MonoBehaviour {

	void Awake () {
		// 在游戏启动的第一时间调用tp2_sdk_init
		// 参数gameid是安全中心为每个游戏分配的唯一标识号
		Tp2Sdk.Tp2SdkInit(8888);
	}

	// Use this for initialization
	void Start () {
		// 第一次调用时机在获取用户信息并登录游戏时,如[微信登录].[QQ登录]或[开始游戏]按钮事件触发
		// 在每次获取用户授权信息之后都要调用Tp2UserLogin,如断线重连
		int account_type = 0;									/*与运营平台相关的帐号类型*/
		int world_id = 1;										/*用户游戏角色的大区信息*/
		string open_id = "B73B36366565F9E02C7525516DCF31C2";	/*与登录平台相关的帐号id*/
		string role_id = "032475";								/*区分用户创建的不同角色的标识*/
		//Tp2Sdk.Tp2UserLogin(account_type, world_id, open_id, role_id);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		GUI.skin.label.fontSize = 100;
		GUI.Label(new Rect(10, 10, Screen.width, Screen.height), "GameClientDemo");
	}
}
