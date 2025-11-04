using System.Collections.Generic;
using Windows.Kinect;
using UnityEngine;

namespace Utility.DEMAFilter
{
    public class DEMAFilterManager
    {
        private readonly Dictionary<int, Dictionary<JointType, QuaternionDEMAFilter>> _quaternionFilters
            = new Dictionary<int, Dictionary<JointType, QuaternionDEMAFilter>>();
        private readonly Dictionary<int, Vector3DEMAFilter> _vector3Filters
            = new Dictionary<int, Vector3DEMAFilter>();

        private readonly float _quaternionAlpha;
        private readonly float _vectorAlpha;
        
        public DEMAFilterManager(float quaternionAlpha, float vectorAlpha)
        {
            _quaternionAlpha = quaternionAlpha;
            _vectorAlpha = vectorAlpha;
        }
        
        public Quaternion FilterQuaternion(int playerId, JointType jointType, Quaternion input)
        {
            if (!_quaternionFilters.ContainsKey(playerId))
            {
                _quaternionFilters[playerId] = new Dictionary<JointType, QuaternionDEMAFilter>();
            }

            var playerFilters = _quaternionFilters[playerId];

            if (!playerFilters.ContainsKey(jointType))
            {
                playerFilters[jointType] = new QuaternionDEMAFilter(_quaternionAlpha);
            }

            return playerFilters[jointType].Filter(input);
        }
        
        public Vector3 FilterVector3(int playerId, Vector3 input)
        {
            if (!_vector3Filters.ContainsKey(playerId))
            {
                _vector3Filters[playerId] = new Vector3DEMAFilter(_vectorAlpha);
            }

            return _vector3Filters[playerId].Filter(input);
        }
        
        public void ResetFilters(int playerId)
        {
            if (_quaternionFilters.ContainsKey(playerId))
            {
                _quaternionFilters.Remove(playerId);
            }

            if (_vector3Filters.ContainsKey(playerId))
            {
                _vector3Filters.Remove(playerId);
            }
        }
        
        public void ResetJointFilter(int playerId, JointType jointType)
        {
            if (_quaternionFilters.ContainsKey(playerId) &&
                _quaternionFilters[playerId].ContainsKey(jointType))
            {
                _quaternionFilters[playerId].Remove(jointType);
            }
        }
        
        public void ResetAllFilters()
        {
            _quaternionFilters.Clear();
            _vector3Filters.Clear();
        }
    }
}