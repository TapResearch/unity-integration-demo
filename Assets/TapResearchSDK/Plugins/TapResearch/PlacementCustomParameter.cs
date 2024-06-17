using System;
using UnityEngine;

namespace TapResearch
{
    [Serializable]
    public class PlacementCustomParameter
    {
        [SerializeField]
        private string key;

        [SerializeField]
        private string value;

        public string Key { get { return key; } }
        public string Value { get { return value; } }

        public PlacementCustomParameter(PlacementCustomParameterBuilder builder,
            string key, string value)
        {
            if (builder == null)
                throw new ArgumentNullException("The builder parameter can't be a null");
            this.key = key;
            this.value = value;
        }

    }

    public class PlacementCustomParameterBuilder
    {
        private const int MAX_VALUE_LENGTH = 256;
        private string _key;
        private string _value;

        public PlacementCustomParameterBuilder SetKey(string key)
        {
            _key = key;
            return this;
        }

        public PlacementCustomParameterBuilder SetValue(string value)
        {
            _value = value;
            return this;
        }

        public PlacementCustomParameter build()
        {
            ValidatePlacement();
            return new PlacementCustomParameter(this, _key, _value);
        }

        private void ValidatePlacement()
        {
            if (_key == null)
            {
                throw new PlacementCustomParametersException("The parameter key can't be null");
            }
            else if (_key.Length == 0)
            {
                throw new PlacementCustomParametersException("The parameter key size can't be zero");
            }
            else if (_value == null)
            {
                throw new PlacementCustomParametersException("The parameter value can't be null");
            }
            else if (_key.Length > MAX_VALUE_LENGTH)
            {
                throw new PlacementCustomParametersException(String.Format("The prameter value "
                        + "length should be less than %d characters", MAX_VALUE_LENGTH));
            }
        }
    }
}
