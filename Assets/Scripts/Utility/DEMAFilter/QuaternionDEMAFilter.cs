using UnityEngine;

namespace Utility.DEMAFilter
{
    public class QuaternionDEMAFilter
    {
        private Quaternion _ema1, _ema2;
        private float _alpha;
        private bool _isInitialized;

        public QuaternionDEMAFilter(float alpha)
        {
            _alpha = alpha;
            _isInitialized = false;
        }

        public Quaternion Filter(Quaternion input)
        {
            _ema1 = _isInitialized ? Quaternion.Slerp(_ema1, input, _alpha) : input;
            _ema2 = _isInitialized ? Quaternion.Slerp(_ema2, _ema1, _alpha) : input;

            if (!_isInitialized)
            {
                _isInitialized = true;
            }

            var dema = Quaternion.SlerpUnclamped(_ema2, _ema1, 2f);
            return Quaternion.Normalize(dema);
        }
    }
}