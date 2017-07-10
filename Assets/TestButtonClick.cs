using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestButtonClick : MonoBehaviour {

	public Button surveyButton;

	// Use this for initialization
	void Start () {
		Debug.Log("WOOFWOOFWOOF");
//		surveyButton.gameObject.SetActive (false);
		TapResearch.Configure ("2b6026c0a49d10b1c22af0a52345158d");
		TapResearch.OnSurveyAvailable = this.OnSurveyAvailable;
		TapResearch.SetUniqueUserIdentifier ("KevinUnityTest");
		TapResearch.OnDidReceiveReward = this.OnDidReceiveReward;
		TapResearch.OnSurveyModalOpened = this.OnSurveyModalOpened;
		TapResearch.OnSurveyModalDismissed = this.OnSurveyModalDismissed;
	}

	// Update is called once per frame
	void Update () {

	}

	public void OnButtonClick() {
		TapResearch.ShowSurvey ();
	}

	void OnDidReceiveReward(int quantity, string transactionIdentifier, string currencyName, int payoutEvent) {
		Debug.Log ("You've earned " + quantity + " " + currencyName + ". " + transactionIdentifier);
	}

	void OnSurveyModalOpened () {
		Debug.Log ("Survey Modal Opened");
	}

	void OnSurveyModalDismissed () {
		Debug.Log ("Survey Modal Dismissed");
	}

	void OnSurveyAvailable() {
		Debug.Log ("Survey Avaliable");
		surveyButton.gameObject.SetActive (true);		
	}
		
}
