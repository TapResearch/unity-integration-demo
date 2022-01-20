using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace TapResearch
{
    public class TapResearchSDK : MonoBehaviour
    {

        private static AndroidJavaClass _unityBridge;
        private const string SdkVersion = "2.3.0";
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

        // DELEGATE DEFINITIONS

        public delegate void PlacementDelegate(TRPlacement placement);
        public static PlacementDelegate OnPlacementReady;

        public delegate void PlacementEventDelegate(TRPlacement placement);
        public static PlacementEventDelegate OnPlacementEventReady;

        public delegate void ExpiredPlacementDelegate(String expiredPlacement);
        public static ExpiredPlacementDelegate OnPlacementEventUnavailable;

        public delegate void TRRewardDelegate(TRReward reward);
        private static TRRewardDelegate _rewardDelegate;
        public static TRRewardDelegate OnReceiveReward
        {
            get => _rewardDelegate;
            set
            {
                _rewardDelegate = value;
            }
        }

        public delegate void TRRewardCollectionDelegate(TRReward[] reward);
        private static TRRewardCollectionDelegate _rewardCollectionDelegate;
        public static TRRewardCollectionDelegate OnReceiveRewardCollection
        {
            get => _rewardCollectionDelegate;
            set
            {
                SetReceiveRewardCollection(true);
                _rewardCollectionDelegate = value;

            }
        }

        public delegate void TRSurveyModalDelegate(TRPlacement placement);
        public static TRSurveyModalDelegate OnSurveyWallOpened;
        public static TRSurveyModalDelegate OnSurveyWallDismissed;



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

        public void OnTapResearchPlacementReady(string args)
        {
            Debug.Log("OnTapResearchPlacementReady");
            if (OnPlacementReady != null)
            {
                Debug.Log("On placement ready called with args - " + args);
                // var placement = new TRPlacement(args, _unityBridge);
                var placement = JsonUtility.FromJson<TRPlacement>(args);
                Debug.Log(placement.PlacementIdentifier);
                OnPlacementReady(placement);
            }
        }

        public void TapResearchOnSurveyWallOpened(string args)
        {
            if (OnSurveyWallOpened != null)
            {
                var placement = JsonUtility.FromJson<TRPlacement>(args);
                OnSurveyWallOpened(placement);
            }
        }

        public void TapResearchOnSurveyWallDismissed(string args)
        {
            if (OnSurveyWallDismissed != null)
            {
                var placement = JsonUtility.FromJson<TRPlacement>(args);
                OnSurveyWallDismissed(placement);
            }
        }

        public void OnTapResearchDidReceiveReward(string args)
        {
            if (OnReceiveReward == null)
                return;

            var reward = JsonUtility.FromJson<TRReward>(args);
            OnReceiveReward(reward);
        }

        public void OnTapResearchDidReceiveRewardCollection(string args)
        {
            if (OnReceiveRewardCollection == null)
                return;

            var wrapper = JsonUtility.FromJson<TRRewardList>("{\"Rewards\":" + args + "}");
            OnReceiveRewardCollection(wrapper.Rewards);
        }

        public void OnTapResearchPlacementEventReady(string args)
        {
            Debug.Log("OnTapResearchPlacementEventReady");
            if (OnPlacementEventReady != null)
            {
                Debug.Log("On placement ready called with args - " + args);
                var placement = JsonUtility.FromJson<TRPlacement>(args);
                Debug.Log(placement.PlacementIdentifier);
                OnPlacementEventReady(placement);
            }
        }

        public void OnTapResearchPlacementEventUnavailable(String args)
        {
            Debug.Log("OnTapResearchPlacementEventUnavailable" + args);
            if (OnPlacementEventUnavailable == null) { return; }
            OnPlacementEventUnavailable(args);
        }

        [Serializable]
        private class TRRewardList
        {
            [SerializeField]
            public TRReward[] Rewards;
        }

#if UNITY_EDITOR || (!UNITY_IPHONE && !UNITY_ANDROID)
        static public void Configure(string apiToken)
        {
            Debug.LogWarning("TapResearch will not work in the Unity editor.");
            InitializeInstance();
        }

         public static void ShowSurveyWall(string placementIdentifier) { }
        public static void ShowSurveyWall(string placementIdentifier, PlacementCustomParameters customParameters) { }
        public static void SetUniqueUserIdentifier(string userIdentifier) { }
        public static void SetDebugMode(bool debugMode) { }
        public static void SetNavigationBarColor(string hexColor) { }
        public static void SetNavigationBarText(string text) { }
        public static void SetNavigationBarTextColor(String hexColor) { }
        public static void SetReceiveRewardCollection(Boolean receiveAsCollection) { }

#elif UNITY_IPHONE && !UNITY_EDITOR
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
      extern public static void ShowSurveyWall(string placementIdentifier);
      [DllImport ("__Internal")]
      extern public static void ShowSurveyWallWithParameters(string placementIdentifier, string customParameters);
      [DllImport ("__Internal")]
      extern public static void SetUniqueUserIdentifier(string userIdentifier);
      [DllImport ("__Internal")]
      extern public static void SetNavigationBarColor(string hexColor);
      [DllImport("__Internal")]
      extern public static void SetNavigationBarText(string text);
      [DllImport("__Internal")]
      extern public static void SetNavigationBarTextColor(String hexColor);
      [DllImport("__Internal")]
      extern public static void SetReceiveRewardCollection(Boolean receiveAsCollection);


#elif UNITY_ANDROID && !UNITY_EDITOR
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
        if (!_pluginInitialized)
          Debug.Log("Please call `Configure (string apiToken)` before making any ");
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

      public static void SetUniqueUserIdentifier(string userIdentifier)
      {
          if (isInitialized())
              _unityBridge.CallStatic("setUniqueUserIdentifier", new object[]{userIdentifier});
      }

      public static void SetDebugMode(bool debugMode)
      {
          if (isInitialized())
              _unityBridge.CallStatic("setDebugMode", new object[]{debugMode});
      }

      public static void SetNavigationBarColor(string hexColor)
      {
          if (isInitialized())
              _unityBridge.CallStatic("setNavigationBarColor", hexColor);
      }

      public static void SetNavigationBarText(string text)
      {
        if (isInitialized())
              _unityBridge.CallStatic("setNavigationBarText", text);
      }

      public static void SetNavigationBarTextColor(string hexColor)
      {
        if (isInitialized())
          _unityBridge.CallStatic("setNavigationBarTextColor", hexColor);
      }

      public static void SetReceiveRewardCollection(Boolean receiveAsCollection){
        if (isInitialized())
          _unityBridge.CallStatic("setReceiveRewardCollection", receiveAsCollection);
      }

#endif
    }
}

