using UnityEngine;
using UnityEngine.UI;
using TapResearch;

public class TapResearchTestButtonClick : MonoBehaviour
{
    public Button surveyButton;
    public TRPlacement mainPlacement;

    public TRScreenFader screenFader;
    public TROrientationChanger orientationChanger;
    public TRWaiter waiter;

    void Start() 
    {
        Debug.Log("Unity C# TestButton: Start");
	    DontDestroyOnLoad(transform.gameObject);
    }

    void Awake()
    {
        Debug.Log("Unity C# TestButton: Awake");
	    DontDestroyOnLoad(transform.gameObject);
        Screen.orientation = ScreenOrientation.LandscapeLeft;

#if UNITY_ANDROID
        TapResearchSDK.SetDebugMode(true);
#endif
        TapResearchSDK.OnSurveyWallOpened = this.OnSurveyModalOpened;
        TapResearchSDK.OnSurveyWallDismissed = this.OnSurveyModalDismissed;
        TapResearchSDK.OnReceiveRewardCollection = this.OnReceiveRewardCollection;
        TapResearchSDK.OnPlacementUnavailable = this.OnPlacementUnavailable;
        TapResearchSDK.OnPlacementReady = this.OnPlacementReady;
        TapResearchSDK.OnEventOpened = this.OnEventModalOpened;
        TapResearchSDK.OnEventDismissed = this.OnEventModalDismissed;
#if UNITY_IPHONE
		TapResearchSDK.Configure("YOUR_IOS_API_TOKEN");
#elif UNITY_ANDROID        
        TapResearchSDK.Configure("YOUR_ANDROID_API_TOKEN");
#endif
        TapResearchSDK.SetUniqueUserIdentifier("TestUser");

        Button button = this.GetComponent<Button>();
        button.GetComponentInChildren<Text>().text = "Take Survey (v" + TapResearchSDK.SdkVersion + ")";
        screenFader.SetAlpha(0.0f);
  }

    private void OnPlacementReady(TRPlacement placement)
    {
        Debug.Log("Unity C# TestButton: Placement Ready " + placement.PlacementIdentifier);
#if UNITY_IPHONE
        if (placement.PlacementIdentifier.Equals("YOUR_IOS_PLACEMENT_ID"))
#elif UNITY_ANDROID        
        if (placement.PlacementIdentifier.Equals("YOUR_ANDROID_PLACEMENT_ID"))
#endif
        {
            mainPlacement = placement;
        }
     }

    private void OnPlacementUnavailable(string expiredPlacement)
    {
        Debug.Log("Unity C# TestButton: Placement Unavailable: " + expiredPlacement);
    }

    public void OnButtonClick()
    {
        if (mainPlacement.IsSurveyWallAvailable) 
        {
            Debug.Log("Unity C# TestButton: OnButtonClick called, fading to black");
            screenFader.FadeToBlack(OnFadeToBlackComplete);
        }
        else 
        {
            Debug.Log("Unity C# TestButton: OnButtonClick called, IsSurveyWallAvailable is false!");
        }
    }
    
    void OnReceiveRewardCollection(TRReward[] rewards)
    {
        Debug.Log("Unity C# TestButton: OnReceiveRewardCollection called");
        foreach (TRReward reward in rewards)
        {
            Debug.Log("Unity C# TestButton: Collection You've earned " + reward.RewardAmount + " " + reward.CurrencyName + ". " + reward.TransactionIdentifier);
        }
    }
    
    void OnSurveyModalOpened(TRPlacement placement)
    {
        Debug.Log("Unity C# TestButton: Survey Modal Opened");
    }

    void OnSurveyModalDismissed(TRPlacement placement)
    {
        Debug.Log("Unity C# TestButton: Survey Modal Dismissed, set orientation to landscape left");
        orientationChanger.SetLandscapeLeft(OnOrientationChangedToLandscapeLeft);
    }

    void OnEventModalOpened(TRPlacement placement)
    {
        Debug.Log("Unity C# TestButton: Event Modal Opened");
    }

    void OnEventModalDismissed(TRPlacement placement)
    {
        Debug.Log("Unity C# TestButton: Event Modal Dismissed");
    }

    // Fader and orientation callbacks

    private void OnFadeToBlackComplete()
    {
        Debug.Log("Unity C# TestButton: Fade to black complete!");
        orientationChanger.SetPortrait(OnOrientationChangedToPortrait);
    }

    private void OnOrientationChangedToPortrait()
    {
        Debug.Log("Unity C# TestButton: OnOrientationChangedToPortrait complete, showing survey modal!!");
        mainPlacement.ShowSurveyWall();
    }
    
    private void OnOrientationChangedToLandscapeLeft()
    {
        Debug.Log("Unity C# TestButton: OnOrientationChangedToLandscapeLeft complete, fading from black!!");
        waiter.Wait(1.0f, () => {
            Debug.Log("Unity C# TestButton: Waiter done"); 
            screenFader.FadeFromBlack(() => { Debug.Log("Unity C# TestButton: fade from black complete!"); });
        });
    }

}
