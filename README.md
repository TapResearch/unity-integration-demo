# TapResearch Unity Integration demo app

This simple app demonstrates how to integrate the TapResearch SDK in your app.

* Clone the repo

~~~~~~bash
$ git clone git@github.com:TapResearch/unity-integration-demo.git
~~~~~~

* Start unity and open the folder

* If you want to see things in action make sure you add your Unity api token and a user identifier in `TestButtonClick.cs`

~~~~csharp

  @Override
  void Start ()
  {
   surveyButton.gameObject.SetActive (false);
   TapResearch.Configure ("<api_token>");
   TapResearch.OnSurveyAvailable = this.OnSurveyAvailable;
   TapResearch.SetUniqueUserIdentifier ("<user_identifier>");
   TapResearch.OnDidReceiveReward = this.OnDidReceiveReward;
   TapResearch.OnSurveyModalOpened = this.OnSurveyModalOpened;
   TapResearch.OnSurveyModalDismissed = this.OnSurveyModalDismissed;
 }


~~~~
