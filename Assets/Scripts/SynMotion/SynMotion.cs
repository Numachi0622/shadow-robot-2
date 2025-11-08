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
                SpineMidRotation = _motionParams[playerId].SpineMidRotation * Quaternion.AngleAxis(-180f, Vector3.up),
                
                ElbowLeftRotation = _motionParams[playerId].ElbowLeftRotation * Quaternion.AngleAxis(-180f, Vector3.up) * Quaternion.AngleAxis(-90f, Vector3.forward),
                WristLeftRotation = _motionParams[playerId].WristLeftRotation * Quaternion.AngleAxis(-90f, Vector3.forward),
                HandLeftRotation = _motionParams[playerId].HandLeftRotation,
                
                ElbowRightRotation = _motionParams[playerId].ElbowRightRotation * Quaternion.AngleAxis(90f, Vector3.forward),
                WristRightRotation = _motionParams[playerId].WristRightRotation * Quaternion.AngleAxis(90f, Vector3.forward),
                HandRightRotation = _motionParams[playerId].HandRightRotation,
                
                KneeLeftRotation = _motionParams[playerId].KneeLeftRotation * Quaternion.AngleAxis(-180f, Vector3.right) * Quaternion.AngleAxis(90f, Vector3.up),
                AnkleLeftRotation = _motionParams[playerId].AnkleLeftRotation * Quaternion.AngleAxis(180f, Vector3.right) * Quaternion.AngleAxis(90f, Vector3.up),
                KneeRightRotation = _motionParams[playerId].KneeRightRotation * Quaternion.AngleAxis(-180f, Vector3.right) * Quaternion.AngleAxis(-90f, Vector3.up),
                AnkleRightRotation = _motionParams[playerId].AnkleRightRotation * Quaternion.AngleAxis(180f, Vector3.right) * Quaternion.AngleAxis(-90f, Vector3.up),
                
                SpineMidPosition = _motionParams[playerId].SpineMidPosition
            };

            return motionParam;
        }
    }
}