using UnityEngine;

namespace SynMotion
{
    public class SynMotionSystem
    {
        private MotionParam[] _motionParams;

        private readonly Quaternion _comp = Quaternion.AngleAxis(90f, Vector3.up) * Quaternion.AngleAxis(-90f, Vector3.forward);
        
        public SynMotionSystem(int playerCount = 3)
        {
            _motionParams = new MotionParam[playerCount];
        }

        public void SetMotionParam(MotionParam[] motionParams)
        {
            for (var i = 0; i < motionParams.Length; i++)
            {
                if (!motionParams[i].IsTracked) continue;
                _motionParams[i] = motionParams[i];
            }
        }

        public MotionParam GetMotionParam(int playerId)
        {
            var motionParam = new MotionParam
            {
                SpineMidRotation = _motionParams[playerId].SpineMidRotation * _comp,
                
                ElbowLeftRotation = _motionParams[playerId].ElbowLeftRotation * _comp,
                WristLeftRotation = _motionParams[playerId].WristLeftRotation * _comp,
                HandLeftRotation = _motionParams[playerId].HandLeftRotation * _comp,
                
                ElbowRightRotation = _motionParams[playerId].ElbowRightRotation * _comp,
                WristRightRotation = _motionParams[playerId].WristRightRotation * _comp,
                HandRightRotation = _motionParams[playerId].HandRightRotation * _comp,
                
                KneeLeftRotation = _motionParams[playerId].KneeLeftRotation * _comp,
                AnkleLeftRotation = _motionParams[playerId].AnkleLeftRotation * _comp,
                KneeRightRotation = _motionParams[playerId].KneeRightRotation * 
                                    Quaternion.AngleAxis(-90f, Vector3.forward),
                AnkleRightRotation = _motionParams[playerId].AnkleRightRotation 
                                     * Quaternion.AngleAxis(-90f, Vector3.forward),
                
                SpineMidPosition = _motionParams[playerId].SpineMidPosition
            };

            return motionParam;
        }
    }
}