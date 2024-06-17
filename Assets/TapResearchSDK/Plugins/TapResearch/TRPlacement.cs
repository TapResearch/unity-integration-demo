using UnityEngine;
using System;

namespace TapResearch
{
#pragma warning disable 649

    [Serializable]
    public struct TREvent
    {
        [SerializeField]
        private string event_tag;

        [SerializeField]
        private string event_type;

        [SerializeField]
        private string placement_type;

        [SerializeField]
        private string start_time;

        [SerializeField]
        private string end_time;

        [SerializeField]
        private string identifier;
    }

    [Serializable]
    public struct TRPlacement
    {
        [SerializeField]
        private string placementIdentifier;

        [SerializeField]
        private string currencyName;

        [SerializeField]
        private string placementErrorMessage;

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

        [SerializeField]
        private TREvent[] events;

        public const int PLACEMENT_CODE_SDK_NOT_READY = -1;

        public string PlacementIdentifier { get { return placementIdentifier; } }
        public string CurrencyName { get { return currencyName; } }
        public string ErrorMessage { get { return placementErrorMessage; } }
        public bool IsSurveyWallAvailable { get { return isSurveyWallAvailable; } }
        public bool HasHotSurvey { get { return hasHotSurvey; } }
        public int PlacementCode { get { return placementCode; } }
        public int MaxPayoutInCurrency { get { return maxPayoutInCurrency; } }
        public int MinPayoutInCurrency { get { return minPayoutInCurrency; } }
        public int MaxSurveyLength { get { return maxSurveyLength; } }
        public int MinSurveyLength { get { return minSurveyLength; } }
        public TREvent[] Events { get { return events; } }

        public void ShowSurveyWall()
        {
            isSurveyWallAvailable = false;
            TapResearchSDK.ShowSurveyWall(PlacementIdentifier);
        }

        public void ShowSurveyWall(PlacementCustomParameters customParameters)
        {
            isSurveyWallAvailable = false;
            TapResearchSDK.ShowSurveyWall(PlacementIdentifier, customParameters);
        }

        public void DisplayEvent()
        {
            TapResearchSDK.DisplayEvent(PlacementIdentifier);
        }

        public void DisplayEvent(PlacementCustomParameters customParameters)
        {
            TapResearchSDK.DisplayEvent(PlacementIdentifier, customParameters);
        }

        public bool IsEventAvailable()
        {
            return TapResearchSDK.IsEventAvailable(PlacementIdentifier);
        }
    }
}

