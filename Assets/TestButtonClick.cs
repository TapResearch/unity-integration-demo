using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestButtonClick : MonoBehaviour {

	public Button surveyButton;
	public TRPlacement myPlacement;

	// Use this for initialization
	void Start ()
	{
		surveyButton.gameObject.SetActive (false);
		TapResearch.Configure (API_TOKEN);
		TapResearch.SetUniqueUserIdentifier (UNIQUE_USER_IDENTIFIER);
		TapResearch.OnPlacementReady = this.OnPlacementReady;
		TapResearch.OnSurveyWallOpened = this.OnSurveyWallOpened;
		TapResearch.OnSurveyWallDismissed = this.OnSurveyWallDismissed;
		TapResearch.OnReceiveReward = this.OnDidReceiveReward;
		TapResearch.InitPlacement(PLACEMENT_IDENTIFIER);
	}

	public void OnButtonClick()
	{
		myPlacement.ShowSurveyWall();
	}

	void OnPlacementReady(TRPlacement placement)
  {
		myPlacement = placement;
		surveyButton.gameObject.SetActive(true);
  }

  void OnDidReceiveReward(TRReward reward)
	{
		Debug.Log ("You've earned " + reward.RewardAmount + " " + reward.CurrencyName + ". " + reward.TransactionIdentifier);
	}

	void OnSurveyWallOpened (TRPlacement placement)
	{
		Debug.Log ("Survey Modal Opened");
	}

	void OnSurveyWallDismissed (TRPlacement placement)
	{
		Debug.Log ("Survey Modal Dismissed");
	}

}
