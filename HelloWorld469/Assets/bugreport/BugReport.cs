using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

public sealed class BugReport  {

	private static string mAppId = "11111111";
	private static string mUserId = "22222222";
	private static bool mEnableDebugLog = false;

	private static string mVersion = "1.0.2(20160707)";

	public static void start (){
		Debug.Log ("BugReport version:" + mVersion);
		EnableExceptionHandler ();
		BugReportInit ();
	}

	public static void setUserId(string userId){
		if (string.IsNullOrEmpty(userId)) {
			return;
		}

		mUserId = userId;
		BugReportSetUserId (mUserId);
		// Debug.Log ("user id:" + mUserId);
	}

	public static void setAppId(string appId){
		if (string.IsNullOrEmpty(appId)) {
			return;
		}

		mAppId = appId;
		BugReportSetAppId (mAppId);
		// Debug.Log ("app id:" + mAppId);
	}

	public static void setEnableDebugLog(bool enableDebugLog){
		mEnableDebugLog = enableDebugLog;
		BugReportSetEnableDebugLog (mEnableDebugLog);
		// Debug.Log ("Enable Debug Log:" + mEnableDebugLog);
	}

	private static bool _isInitialized = false;
	private static void EnableExceptionHandler(){
		if (_isInitialized){
			return;
		}
		RegisterExceptionHandler();
		_isInitialized = true;
	}
	
	private static void DisableExceptionHandler(){
		if (!_isInitialized){
			return;
		}
		UnregisterExceptionHandler();
		_isInitialized = false;
	}
	
	private static void RegisterExceptionHandler(){
		#if (UNITY_4_6 || UNITY_4_7) 
		Application.RegisterLogCallback (LogCallbackHandler);
		#endif
		
		#if UNITY_5
		Application.logMessageReceived += LogCallbackHandler;
		#endif

		AppDomain.CurrentDomain.UnhandledException += UncaughtExceptionHandler;
	}
	
	private static void UnregisterExceptionHandler(){
		#if (UNITY_4_6 || UNITY_4_7) 
		Application.RegisterLogCallback (null);
		#endif
		
		#if UNITY_5
		Application.logMessageReceived -= LogCallbackHandler;
		#endif

		AppDomain.CurrentDomain.UnhandledException -= UncaughtExceptionHandler;
	}
	
	private static void LogCallbackHandler(string condition, string stack, LogType type){
		if (type == LogType.Exception) {
			// FileLog.Instance.Log ("<LogCallbackHandler>reason:" + condition);
			// FileLog.Instance.Log ("<LogCallbackHandler>stack:" + stack);
			HandleException (condition, stack);
		}
		
	}
	
	private static void UncaughtExceptionHandler(object sender, System.UnhandledExceptionEventArgs args){
		// FileLog.Instance.Log ("UncaughtExceptionHandler");
		Exception e = (Exception)args.ExceptionObject;
		if (e != null){
			HandleException (e.Message, e.StackTrace);
		}
		
	}
	
	#if (UNITY_EDITOR || UNITY_STANDALONE)
	private static void HandleException(string reason, string stack){
		Debug.Log ("reason:" + reason);
		Debug.Log ("stack:" + stack);
	}
	
	
	private static void BugReportInit(){
		Debug.Log ("BugReportInit");
	}
	
	private static void BugReportSetUserId(string userId){
		Debug.Log ("BugReportSetUserId:" + userId);
	}
	
	private static void BugReportSetAppId(string appId){
		Debug.Log ("BugReportSetAppId:" + appId);
	}
	
	private static void BugReportSetEnableDebugLog(bool enableDebugLog){
		Debug.Log ("BugReportSetEnableDebugLog:" + enableDebugLog);
	}
	
	
	#elif UNITY_ANDROID
	private static readonly string CLASS_UNITYAGENT = "com.tencent.tqm.bugreport.BugReportAgent";
	private static AndroidJavaObject _unityAgent;
	public static AndroidJavaObject UnityAgent{
		get{
			if (_unityAgent == null){
				using (AndroidJavaClass clazz = new AndroidJavaClass(CLASS_UNITYAGENT)) {
					_unityAgent = clazz.CallStatic<AndroidJavaObject> ("getInstance");
				}
			}
			return _unityAgent;
		}
	}
	private static void HandleException(string reason, string stack){
		try{
			UnityAgent.Call("reportCSharpException", reason,stack);	
		}catch{
			Debug.LogWarning("Android HandleException catch");
		}
	}

	private static void BugReportInit(){
		try{
			UnityAgent.Call("init");	
		}catch{
			Debug.LogWarning("init fail");
		}
	}

	private static void BugReportSetUserId(string userId){
		try{
			UnityAgent.Call("setUserId", userId);	
		}catch{
			Debug.LogWarning("set user id fail");
		}
	}

	private static void BugReportSetAppId(string appId){
		try{
			UnityAgent.Call("setAppId", appId);	
		}catch{
			Debug.LogWarning("set app id fail");
		}
	}

	private static void BugReportSetEnableDebugLog(bool enableDebugLog){
		try{
			UnityAgent.Call("setEnableDebugLog", enableDebugLog);	
		}catch{
			Debug.LogWarning("setEnableDebugLog fail");
		}
	}
	
	#elif (UNITY_IOS || UNITY_IPHONE)
	[DllImport("__Internal")]
	private static extern void BugReportSendCSharpException (string reason, string stack);
	private static void HandleException(string reason, string stack){
		BugReportSendCSharpException(reason, stack);
	}

	[DllImport("__Internal")]
	private static extern void BugReportInit();

	[DllImport("__Internal")]
	private static extern void BugReportSetUserId(string userId);

	[DllImport("__Internal")]
	private static extern void BugReportSetAppId(string appId);

	[DllImport("__Internal")]
	private static extern void BugReportSetEnableDebugLog(bool enableDebugLog);



	
	#endif
} // end of class BugReport

