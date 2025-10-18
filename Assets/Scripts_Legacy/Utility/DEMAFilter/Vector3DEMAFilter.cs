using UnityEngine;

namespace DEMAFilter
{
    public class Vector3DEMAFilter
    {
        private Vector3 _ema1, _ema2;
        private float _alpha;
        private bool _isInitialized;

        public Vector3DEMAFilter(float alpha)
        {
            _alpha = alpha;
            _isInitialized = false;
        }

        public Vector3 Filter(Vector3 input)
        {
            _ema1 = _isInitialized ? 
                _alpha * input + (1 - _alpha) * _ema1 : input;
            _ema2 = _isInitialized ?
                _alpha * _ema1 + (1 - _alpha) * _ema2 : input;

            if (!_isInitialized)
            {
                _isInitialized = true;
            }

            return 2.0f * _ema1 - _ema2;
        }
    }
}