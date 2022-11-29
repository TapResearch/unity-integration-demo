using System;
using System.Collections.Generic;
using UnityEngine;

namespace TapResearch
{
    public class PlacementCustomParameters
    {
        private const int MAX_PASS_VALUES = 5;

        [SerializeField]
        private List<PlacementCustomParameter> ParameterList =
            new List<PlacementCustomParameter>();

        public List<PlacementCustomParameter> List {get{ return ParameterList; } }

        public void AddParameter(PlacementCustomParameter customParameter) 
        {
            if (ParameterList.Count < MAX_PASS_VALUES)
            {
                ParameterList.Add(customParameter);
            }
            else
            {
                throw new PlacementCustomParametersException(String.Format("The maximum number of "
                    + "parameters is %d", MAX_PASS_VALUES));
            }
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }

}


