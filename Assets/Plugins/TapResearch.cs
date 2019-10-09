using UnityEngine;
using System;
using System.Runtime.InteropServices;

    public class TapResearch : MonoBehaviour {

        private static AndroidJavaClass _unityBridge;
        private const string version = "2.0.7";
        // Make sure there is only one instance of TapResearch
        private static TapResearch _instance;

        // A game object must exist for us to pass messages from native to unity
        private static void InitializeInstance()
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType (typeof(TapResearch)) as TapResearch;

                if (_instance == null)
                {
                    _instance = new GameObject ("TapResearch").AddComponent<TapResearch> ();
                }
            }
        }

        // DELEGATE DEFINITIONS

        public delegate void PlacementDelegate(TRPlacement placement);
        public static PlacementDelegate OnPlacementReady;

        public delegate void TRRewardDelegate (TRReward reward);
        public static TRRewardDelegate OnReceiveReward;

        public delegate void TRSurveyModalDelegate (TRPlacement placement);
        public static TRSurveyModalDelegate OnSurveyWallOpened;
        public static TRSurveyModalDelegate OnSurveyWallDismissed;


        void Awake()
        {
            // Set name to allow unity to find this object when passing messages.
            name = "TapResearch";

            // Make sure this object persists across scenes
            DontDestroyOnLoad (transform.gameObject);
        }

        void Start()
        {
            DontDestroyOnLoad (transform.gameObject);
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
                OnSurveyWallOpened (placement);
            }
        }

        public void TapResearchOnSurveyWallDismissed(string args)
        {
            if (OnSurveyWallDismissed != null)
            {
              // var placement = new TRPlacement(args, _unityBridge);
              var placement = JsonUtility.FromJson<TRPlacement>(args);
              OnSurveyWallDismissed (placement);
            }
        }

        public void OnTapResearchDidReceiveReward(string args)
        {
            if (OnReceiveReward == null)
                return;

            var reward = JsonUtility.FromJson<TRReward>(args);
            OnReceiveReward (reward);
        }

    #if UNITY_EDITOR || (!UNITY_IPHONE && !UNITY_ANDROID)
      static public void Configure(string apiToken)
      {
          Debug.LogWarning ("TapResearch will not work in the Unity editor.");
          InitializeInstance ();
      }

      public static void InitPlacement(string placementIdentifier) { }
      public static void ShowSurveyWall(string placementIdentifier) { }
      public static void SetUniqueUserIdentifier (string userIdentifier) { }
      public static void SetDebugMode(bool debugMode) { }
      public static void SetNavigationBarColor(string hexColor) { }
      public static void SetNavigationBarText(string text) { }
      public static void SetNavigationBarTextColor(String hexColor) { }

    #elif UNITY_IPHONE && !UNITY_EDITOR
      public static void Configure (string apiToken)
      {
          InitializeInstance ();
          TRIOSConfigure (apiToken, version);
      }

      [DllImport ("__Internal")]
      extern public static void TRIOSConfigure(string apiToken, string version);
      [DllImport ("__Internal")]
      extern public static void InitPlacement(string placementIdentifier);
      [DllImport ("__Internal")]
      extern public static void ShowSurveyWall(string placementIdentifier);
      [DllImport ("__Internal")]
      extern public static void SetUniqueUserIdentifier(string userIdentifier);
      [DllImport ("__Internal")]
      extern public static void SetNavigationBarColor(string hexColor);
      [DllImport("__Internal")]
      extern public static void SetNavigationBarText(string text);
      [DllImport("__Internal")]
      extern public static void SetNavigationBarTextColor(String hexColor);


    #elif UNITY_ANDROID && !UNITY_EDITOR
      private static bool _pluginInitialized = false;
      private static AndroidJavaClass _unityPlayer;

      public static void Configure (string apiToken)  {
          InitializeInstance ();
          AndroidConfigure (apiToken, version);
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

      private static void AndroidConfigure(string apiToken, string version)
      {
          if (!_pluginInitialized)
          {
              InitializeAndroidPlugin ();

              var javaActivity = _unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
              _unityBridge.CallStatic("configure", new object[]{apiToken, javaActivity, version});
          }
      }

      private static bool isInitialized()
      {
        if (!_pluginInitialized)
          Debug.Log("Please call `Configure (string apiToken)` before making any ");
          return _pluginInitialized;
      }

      public static void InitPlacement(string placementIdentifier)
      {
          if (isInitialized())
              _unityBridge.CallStatic("initPlacement", new object[]{placementIdentifier});
      }

      public static void ShowSurveyWall(string placementIdentifier)
      {
        if (isInitialized()) {
          _unityBridge.CallStatic("showSurveyWall", placementIdentifier);
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


    #endif
  }

[Serializable]
public class TRPlacement
{
      [SerializeField]
      private string placementIdentifier;

      [SerializeField]
      private string currencyName;

      [SerializeField]
      private string errorMessage;

      [SerializeField]
      private bool isSurveyWallAvailable;

      [SerializeField]
      private bool hasHotSurvey;

      [SerializeField]
      private int placementCode;

      [SerializeField]
      private int maxPayoutInCurrency;

      [SerializeField]
      private int minPayoutInCurrency;

      [SerializeField]
      private int maxSurveyLength;

      [SerializeField]
      private int minSurveyLength;

      public const int PLACEMENT_CODE_SDK_NOT_READY = -1;

      public string PlacementIdentifier { get { return placementIdentifier; } }
      public string CurrencyName { get{ return currencyName; } }
      public string ErrorMessage { get {return errorMessage;} }
      public bool IsSurveyWallAvailable { get{ return isSurveyWallAvailable; } }
      public bool HasHotSurvey { get{ return hasHotSurvey; } }
      public int PlacementCode { get {return placementCode; } }
      public int MaxPayoutInCurrency { get { return maxPayoutInCurrency; } }
      public int MinPayoutInCurrency { get { return minPayoutInCurrency; } }
      public int MaxSurveyLength { get { return maxSurveyLength; } }
      public int MinSurveyLength { get { return minSurveyLength; } }

      public void ShowSurveyWall()
      {
          isSurveyWallAvailable = false;
          TapResearch.ShowSurveyWall(PlacementIdentifier);
      }
}

[Serializable]
public class TRReward
{
    [SerializeField]
    private string transactionIdentifier;

    [SerializeField]
    private string currencyName;

    [SerializeField]
    private string placementIdentifier;

    [SerializeField]
    private int rewardAmount;

    [SerializeField]
    private int payoutEvent;

    public string TransactionIdentifier{ get {return transactionIdentifier;} }

    public string CurrencyName { get {return currencyName;} }

    public string PlacementIdentifier { get {return placementIdentifier;} }

    public int RewardAmount { get {return rewardAmount;} }

    public int PayoutEvent { get {return payoutEvent;} }
}
