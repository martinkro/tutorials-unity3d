using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public static class Tp2Sdk{

	public static void Tp2SdkInit (int gameId) {
		tp2_sdk_init(gameId);
	}

	public static void Tp2UserLogin (int account_type, int world_id, string open_id, string role_id)
	{
		tp2_setuserinfo(account_type, world_id, open_id, role_id);
	}

	#if UNITY_IOS
	[DllImport("__Internal")]
	#else
	[DllImport("tersafe2")]
	#endif
	private static extern int tp2_sdk_init(int gameId);

	#if UNITY_IOS
	[DllImport("__Internal")]
	#else	
	[DllImport("tersafe2")]
	#endif
	private static extern int tp2_setuserinfo(int account_type, int world_id, string open_id, string role_id);
}

public class UserInfo
{
	public int account_type;
	public int world_id;
	public string open_id;
	public string role_id;
}

public class TssSdkVersion 
{
	private const string  cs_sdk_version = "C# SDK ver: 1.0(2015/05/13)";
	public static string GetSdkVersion()
	{
		return cs_sdk_version;	
	}
}
