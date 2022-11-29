using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TapResearch;

public class TestButtonClick : MonoBehaviour
{
    public Button surveyButton;
    private TRPlacement mainPlacement;

    // Use this for initialization
    void Awake()
    {
        surveyButton.enabled = false;
        TapResearchSDK.Configure("<api_token>");
        TapResearchSDK.SetUniqueUserIdentifier("<user_identifier>");
        TapResearchSDK.OnSurveyWallOpened = this.OnSurveyModalOpened;
        TapResearchSDK.OnSurveyWallDismissed = this.OnSurveyModalDismissed;
        TapResearchSDK.OnReceiveRewardCollection = this.OnReceiveRewardCollection;
        TapResearchSDK.OnReceiveReward = this.OnDidReceiveReward;
        TapResearchSDK.OnPlacementEventUnavailable = this.OnPlacementEventUnavailable;
        TapResearchSDK.OnPlacementEventReady = this.OnPlacementEventReady;
    }

    private void OnPlacementEventReady(TRPlacement placement)
    {
        surveyButton.enabled = true;
        Debug.Log("Placement Received " + placement.PlacementIdentifier);
        if (placement.PlacementCode != TRPlacement.PLACEMENT_CODE_SDK_NOT_READY)
        {
            mainPlacement = placement;
        }
    }

    private void OnPlacementEventUnavailable(string expiredPlacement)
    {
        Debug.Log("Placement expired: " + expiredPlacement);
    }

    public void OnButtonClick()
    {
        Debug.Log("Button pressed: attmpting to show: " + mainPlacement.IsSurveyWallAvailable + " " + mainPlacement.PlacementIdentifier);
        if (mainPlacement.IsSurveyWallAvailable)
            mainPlacement.ShowSurveyWall();
    }

    void OnDidReceiveReward(TRReward reward)
    {
        Debug.Log("You've earned " + reward.RewardAmount + " " + reward.CurrencyName + ". " + reward.TransactionIdentifier);
    }

    void OnReceiveRewardCollection(TRReward[] rewards)
    {
        foreach (TRReward reward in rewards)
        {
            Debug.Log("Collection You've earned " + reward.RewardAmount + " " + reward.CurrencyName + ". " + reward.TransactionIdentifier);
        }
    }

    void OnSurveyModalOpened(TRPlacement placement)
    {
        Debug.Log("Survey Modal Opened");
    }

    void OnSurveyModalDismissed(TRPlacement placement)
    {
        Debug.Log("Survey Modal Dismissed");
    }

}
