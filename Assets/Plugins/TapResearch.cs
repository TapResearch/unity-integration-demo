using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class TapResearch : MonoBehaviour {
	// Make sure there is only one instance of TapResearch
	private static TapResearch instance;

	// A game object must exist for us to pass messages from native to unity
	private static void InitializeInstance () {
		if (instance == null) {
			instance = FindObjectOfType (typeof(TapResearch)) as TapResearch;

			if (instance == null) {
				instance = new GameObject ("TapResearch").AddComponent<TapResearch> ();
			}

		}
	}

	// DELEGATE DEFINITIONS
	public delegate void TRRewardDelegate (int quantity, string transactionIdentifier,
		string currencyName, int payoutEvent, string offerIdentifier);
	public static TRRewardDelegate OnDidReceiveReward;

	public delegate void TRSurveyModalDelegate ();
	public static TRSurveyModalDelegate OnSurveyModalOpened;
	public static TRSurveyModalDelegate OnSurveyModalDismissed;
	public static TRSurveyModalDelegate OnSurveyAvailable;
	public static TRSurveyModalDelegate OnSurveyNotAvailable;


	void Awake() {
		// Set name to allow unity to find this object when passing messages.
		name = "TapResearch";

		// Make sure this object persists across scenes
		DontDestroyOnLoad (transform.gameObject);
	}

	public void OnTapResearchDidReceiveReward (string args) {
		if (OnDidReceiveReward == null)
			return;

		string[] argsArray = args.Split ('|');
		int quantity = int.Parse (argsArray [0]);
		string tid = argsArray [1];
		string currencyName = argsArray [2];
		int payoutEvent = int.Parse (argsArray [3]);
		string offerIdentifier = argsArray [4];
		OnDidReceiveReward (quantity, tid, currencyName, payoutEvent, offerIdentifier);
	}

	public void OnTapResearchSurveyModalOpened(string args) {
		if (OnSurveyModalOpened != null)
			OnSurveyModalOpened ();
	}

	public void OnTapResearchSurveyModalDismissed (string args) {
		if (OnSurveyModalDismissed != null)
			OnSurveyModalDismissed ();
	}

	public void OnTapResearchSurveyAvailable() {
		if (OnSurveyAvailable != null)
			OnSurveyAvailable ();
	}

	public void OnTapResearchSurveyNotAvailable() {
		if (OnSurveyNotAvailable != null)
			OnSurveyNotAvailable ();
	}


#if UNITY_EDITOR || (!UNITY_IPHONE && !UNITY_ANDROID)
	static public void Configure (string apiToken) {
		Debug.LogWarning ("TapResearch will not work in the Unity editor.");
		InitializeInstance ();
	}

	public static bool IsSurveyAvailable () { return true; }
	public static void ShowSurvey () { }
	public static void ShowSurveyWithIdentifier (string surveyIdentifier) { }
	public static void SetUniqueUserIdentifier (string userIdentifier) { }
	public static void SetDebugMode(bool debugMode) { }

#elif UNITY_IPHONE && !UNITY_EDITOR
	public static void Configure (string apiToken) {
		InitializeInstance ();
		TRIOSConfigure (apiToken);
	}

	[DllImport ("__Internal")]
	extern public static void TRIOSConfigure (string apiToken);
	[DllImport ("__Internal")]
	extern public static bool IsSurveyAvailable ();
	[DllImport ("__Internal")]
	extern public static void ShowSurveyWithIdentifier (string offerIdentifier);
	[DllImport ("__Internal")]
	extern public static void ShowSurvey ();
	[DllImport ("__Internal")]
	extern public static void SetUniqueUserIdentifier (string userIdentifier);


#elif UNITY_ANDROID && !UNITY_EDITOR
	private static bool _pluginInitialized = false;
	private static AndroidJavaClass _unityPlayer;
	private static AndroidJavaClass _unityBridge;

	public static void Configure (string apiToken) {
		InitializeInstance ();
		AndroidConfigure (apiToken);
	}

	private static void InitializeAndroidPlugin() {
		// Check of existence of unitybridge and tapresearch libraries.
		_unityBridge = new AndroidJavaClass("com.tapr.unitybridge.TRUnityBridge");
		if (_unityBridge == null)
		{
			Debug.LogError("********************* Can't create AndroidJavaClass ***************************");
			return;
		}


		var localTapResearch = AndroidJNI.FindClass ("com/tapr/sdk/TapResearch");
		if (localTapResearch != IntPtr.Zero)
		{
			AndroidJNI.DeleteLocalRef (localTapResearch);
		}
		else
		{
			Debug.LogError ("TapResearch android config error. Make sure you've included both tapresearch.jar and unitybridge.jar in your Unity project's Assets/Plugins/Android folder.");
			return;
		}

		_unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");

		_pluginInitialized = true;
	}

	private static void AndroidConfigure (string apiToken) {
		if (!_pluginInitialized)
			InitializeAndroidPlugin ();

		var javaActivity = _unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
		_unityBridge.CallStatic("configure", new object[]{apiToken, javaActivity});
	}

	private static bool isInitialized() {
	  if (!_pluginInitialized)
			Debug.Log("Please call `Configure (string apiToken)` before making any ");
		return _pluginInitialized;
	}

	public static bool IsSurveyAvailable () {
		if (isInitialized())
		{
			return _unityBridge.CallStatic<bool>("isSurveyAvailable");
		}
		else
		{
			return false;
		}
	}

	public static void ShowSurvey () {
		if (isInitialized())
			_unityBridge.CallStatic("showSurvey");
	}

	public static void ShowSurveyWithIdentifier (string offerIdentifier) {
		if (isInitialized())
			_unityBridge.CallStatic("showSurvey", offerIdentifier);
	}

	public static void SetUniqueUserIdentifier (string userIdentifier) {
		if (isInitialized())
			_unityBridge.CallStatic("setUniqueUserIdentifier", new object[]{userIdentifier});
	}

	public static void SetDebugMode (bool debugMode) {
		if (isInitialized())
			_unityBridge.CallStatic("setDebugMode", new object[]{debugMode});
	}


#endif

}
