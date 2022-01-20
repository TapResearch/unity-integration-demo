# TapResearch Unity Integration demo app

This simple app demonstrates how to integrate the [TapResearch Unity SDK](https://www.tapresearch.com/docs/unity-integration-guide) in your app.

* Clone the repo

~~~~~~bash
$ git clone git@github.com:TapResearch/unity-integration-demo.git
~~~~~~

* Start unity and open the folder

* If you want to see things in action make sure you add your Unity api token and a user identifier in `TestButtonClick.cs`

~~~~csharp
    void Awake()
    {
        surveyButton.enabled = false;
        TapResearchSDK.Configure("<api_token>");
        TapResearchSDK.SetUniqueUserIdentifier("<user_identifier>");
        TapResearchSDK.OnSurveyWallOpened = this.OnSurveyModalOpened;
        TapResearchSDK.OnSurveyWallDismissed = this.OnSurveyModalDismissed;
        TapResearchSDK.OnReceiveRewardCollection = this.OnReceiveRewardCollection;
        TapResearchSDK.OnPlacementEventUnavailable = this.OnPlacementEventUnavailable;
        TapResearchSDK.OnPlacementEventReady = this.OnPlacementEventReady;
    }

~~~~
* Please note that the SDK only works for iOS or Android and not for the default player
