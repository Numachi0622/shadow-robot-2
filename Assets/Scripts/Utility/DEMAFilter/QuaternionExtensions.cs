using System.Collections.Generic;
using UnityEngine;
using Kinect = Windows.Kinect;

namespace DEMAFilter
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

    public static class QuaternionExtensions
    {
        private static readonly Dictionary<Kinect.JointType, QuaternionDEMAFilter> _filters =
            new Dictionary<Kinect.JointType, QuaternionDEMAFilter>();

        public static Quaternion DEMAFilter(this Quaternion quaternion, Kinect.JointType jointType)
        {
            if (!_filters.ContainsKey(jointType))
            {
                _filters[jointType] = new QuaternionDEMAFilter(GameConst.DEMA_FILTER_ALPHA);
            }

            return _filters[jointType].Filter(quaternion);
        }

        public static void ResetDEMAFilter(Kinect.JointType joinType)
        {
            if(!_filters.ContainsKey(joinType)) return;
            _filters.Remove(joinType);
        }
    }
}