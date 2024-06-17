
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace TapResearch
{
	public class TapResearchSDK : MonoBehaviour
	{

		private static AndroidJavaClass _unityBridge;
		public const string SdkVersion = "2.5.17";
		// Make sure there is only one instance of TapResearch
		private static TapResearchSDK _instance;

		// A game object must exist for us to pass messages from native to unity
		private static void InitializeInstance()
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType(typeof(TapResearchSDK)) as TapResearchSDK;

				if (_instance == null)
				{
					_instance = new GameObject("TapResearch").AddComponent<TapResearchSDK>();
				}
			}
		}

		#region DELEGATE_DEFINITIONS

		public delegate void PlacementDelegate(TRPlacement placement);
		public static PlacementDelegate OnPlacementReady;

		public delegate void ExpiredPlacementDelegate(String expiredPlacement);
		public static ExpiredPlacementDelegate OnPlacementUnavailable;

		public delegate void TRRewardCollectionDelegate(TRReward[] reward);
		public static TRRewardCollectionDelegate OnReceiveRewardCollection;


		public delegate void TRSurveyModalDelegate(TRPlacement placement);
		public static TRSurveyModalDelegate OnSurveyWallOpened;
		public static TRSurveyModalDelegate OnSurveyWallDismissed;

		public delegate void TREventModalDelegate(TRPlacement placement);
		public static TREventModalDelegate OnEventOpened;
		public static TREventModalDelegate OnEventDismissed;

		#endregion

		#region UNITY

		void Awake()
		{
			// Set name to allow unity to find this object when passing messages.
			name = "TapResearch";

			// Make sure this object persists across scenes
			DontDestroyOnLoad(transform.gameObject);
		}

		void Start()
		{
			DontDestroyOnLoad(transform.gameObject);
		}

		#endregion

		#region TAPRESEARCH

		public void TapResearchOnEventOpened(string args)
		{
			//Debug.Log("Bridge Unity C#: TapResearchOnEventOpened");
			if (OnPlacementReady != null)
			{
				var placement = JsonUtility.FromJson<TRPlacement>(args);
				Debug.Log(placement.PlacementIdentifier);
				OnEventOpened(placement);
			}
			else
			{
				Debug.LogError("Bridge Unity C#: TapResearchOnEventOpened CALLBACK IS NULL");
			}
		}

		public void TapResearchOnEventDismissed(string args)
		{
			//Debug.Log("Bridge Unity C#: TapResearchOnEventDismissed");
			if (OnPlacementReady != null)
			{
				var placement = JsonUtility.FromJson<TRPlacement>(args);
				Debug.Log(placement.PlacementIdentifier);
				OnEventDismissed(placement);
			}
			else
			{
				Debug.LogError("Bridge Unity C#: TapResearchOnEventDismissed CALLBACK IS NULL");
			}
		}

		public void TapResearchOnSurveyWallOpened(string args)
		{
			//Debug.Log("Bridge Unity C#: TapResearchOnSurveyWallOpened");
			if (OnSurveyWallOpened != null)
			{
				var placement = JsonUtility.FromJson<TRPlacement>(args);
				OnSurveyWallOpened(placement);
			}
			else
			{
				Debug.LogError("Bridge Unity C#: TapResearchOnSurveyWallOpened CALLBACK IS NULL");
			}
		}

		public void TapResearchOnSurveyWallDismissed(string args)
		{
			//Debug.Log("Bridge Unity C#: TapResearchOnSurveyWallDismissed");
			if (OnSurveyWallDismissed != null)
			{
				var placement = JsonUtility.FromJson<TRPlacement>(args);
				OnSurveyWallDismissed(placement);
			}
			else
			{
				Debug.LogError("Bridge Unity C#: TapResearchOnSurveyWallDismissed CALLBACK IS NULL");
			}
		}

		public void OnTapResearchDidReceiveRewardCollection(string args)
		{
			//Debug.Log("Bridge Unity C#: OnTapResearchDidReceiveRewardCollection");
			if (OnReceiveRewardCollection == null)
			{
				Debug.LogError("Bridge Unity C#: OnTapResearchDidReceiveRewardCollection CALLBACK IS NULL");
				return;
			}
			
			var wrapper = JsonUtility.FromJson<TRRewardList>("{\"Rewards\":" + args + "}");
			OnReceiveRewardCollection(wrapper.Rewards);
		}

		public void OnTapResearchPlacementReady(string args)
		{
			//Debug.Log("Bridge Unity C#: OnTapResearchPlacementReady");
			if (OnPlacementReady != null)
			{
				var placement = JsonUtility.FromJson<TRPlacement>(args);
				Debug.Log(placement.PlacementIdentifier);
				OnPlacementReady(placement);
			}
			else
			{
				Debug.LogError("Bridge Unity C#: OnTapResearchPlacementReady CALLBACK IS NULL");
			}
		}

		public void OnTapResearchPlacementUnavailable(String args)
		{
			if (OnPlacementUnavailable == null)
			{
				Debug.LogError("Bridge Unity C#: OnTapResearchPlacementUnavailable CALLBACK IS NULL");
				return;
			}
			OnPlacementUnavailable(args);
		}

		[Serializable]
		private class TRRewardList
		{
			[SerializeField]
			public TRReward[] Rewards;
		}

		#endregion

#if UNITY_EDITOR || (!UNITY_IPHONE && !UNITY_ANDROID)

		#region EDITOR

		static public void Configure(string apiToken)
		{
			Debug.LogWarning("TapResearch will not work in the Unity editor.");
			InitializeInstance();
		}

		public static void ShowSurveyWall(string placementIdentifier) { }
		public static void ShowSurveyWall(string placementIdentifier, PlacementCustomParameters customParameters) { }
		public static void SetUniqueUserIdentifier(string userIdentifier) { }
		public static void SetDebugMode(bool debugMode) { }
		public static void DisplayEvent(string placementIdentifier) { }
		public static void DisplayEvent(string placementIdentifier, PlacementCustomParameters customParameters) { }
		public static bool IsEventAvailable(string placementIdentifier) { return false; }
#if UNITY_ANDROID || (!UNITY_EDITOR && !UNITY_IPHONE)
		public static void SetReceiveRewardCollection(Boolean receiveAsCollection) { }
#endif

		#endregion

#elif UNITY_IPHONE && !UNITY_EDITOR
	
		#region IPHONE
	
	public static void Configure (string apiToken)
	{
		InitializeInstance ();
		TRIOSConfigure (apiToken, SdkVersion);
	}
	
	public static void ShowSurveyWall(String placementIdentifier, PlacementCustomParameters customParameters)
	{
		ShowSurveyWallWithParameters(placementIdentifier, JsonUtility.ToJson(customParameters));
	}
	
	[DllImport ("__Internal")]
	extern public static void TRIOSConfigure(string apiToken, string SdkVersion);
	[DllImport ("__Internal")]
	extern public static void SetUniqueUserIdentifier(string userIdentifier);
	[DllImport ("__Internal")]
	extern public static void ShowSurveyWall(string placementIdentifier);
	[DllImport ("__Internal")]
	extern public static void ShowSurveyWallWithParameters(string placementIdentifier, string customParameters);
	[DllImport ("__Internal")]
	extern public static void DisplayEvent(string placementIdentifier);
	[DllImport ("__Internal")]
	extern public static void DisplayEvent(string placementIdentifier, PlacementCustomParameters customParameters);
    [DllImport ("__Internal")]
	extern public static bool IsEventAvailable(string placementIdentifier);

		#endregion
	
#elif UNITY_ANDROID && !UNITY_EDITOR
	
		#region ANDROID
	
	private static bool _pluginInitialized = false;
	private static AndroidJavaClass _unityPlayer;
	
	public static void Configure (string apiToken)  {
		InitializeInstance ();
		AndroidConfigure (apiToken, SdkVersion);
	}
	
	private static void InitializeAndroidPlugin()
	{
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
		
		_unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		_pluginInitialized = true;
	}
	
	private static void AndroidConfigure(string apiToken, string SdkVersion)
	{
		if (!_pluginInitialized)
		{
			InitializeAndroidPlugin ();
			
			var javaActivity = _unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
			_unityBridge.CallStatic("configure", new object[]{apiToken, javaActivity, SdkVersion});
		}
	}
	
	private static bool isInitialized()
	{
		if (!_pluginInitialized) {
			Debug.Log("Please call `Configure (string apiToken)` before making any ");
		}
		return _pluginInitialized;
	}
	
	public static void ShowSurveyWall(string placementIdentifier)
	{
		if (isInitialized()) {
			_unityBridge.CallStatic("showSurveyWall", placementIdentifier);
		}
	}
	
	public static void ShowSurveyWall(string placementIdentifier, PlacementCustomParameters customParameters)
	{
		if (isInitialized()) {
			_unityBridge.CallStatic("showSurveyWall", placementIdentifier, JsonUtility.ToJson(customParameters));
		}
	}
	
	public static void DisplayEvent(string placementIdentifier) {
		if (isInitialized()) {
			_unityBridge.CallStatic("displayEvent", placementIdentifier);
		}
	}
	
	public static void DisplayEvent(string placementIdentifier, PlacementCustomParameters customParameters) {
		if (isInitialized()) {
			_unityBridge.CallStatic("displayEvent", placementIdentifier, JsonUtility.ToJson(customParameters));
		}
	}
	
	public static void SetUniqueUserIdentifier(string userIdentifier)
	{
		if (isInitialized()) {
			_unityBridge.CallStatic("setUniqueUserIdentifier", new object[]{userIdentifier});
		}
	}
	
	public static void SetDebugMode(bool debugMode)
	{
		if (isInitialized()) {
			_unityBridge.CallStatic("setDebugMode", new object[]{debugMode});
		}
	}
	
	public static bool IsEventAvailable(string placementIdentifier) {
		 var _isEventAvailable = false;
		if (isInitialized()) {
		    _isEventAvailable = _unityBridge.CallStatic<bool>("isEventAvailable", placementIdentifier);
		}
		 return _isEventAvailable;
	}

	public static void SetReceiveRewardCollection(Boolean receiveAsCollection){
        if (isInitialized())
          _unityBridge.CallStatic("setReceiveRewardCollection", receiveAsCollection);
      }

		#endregion
	
#endif

	}
}
