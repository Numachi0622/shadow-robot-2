using System;
using UnityEngine;
using Utility;

namespace SynMotion
{
    public class SynMotionSystem
    {
        private readonly MotionParam[] _motionParams = new MotionParam[3];

        private readonly Quaternion _comp = Quaternion.AngleAxis(90f, Vector3.up) * Quaternion.AngleAxis(-90f, Vector3.forward);

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
                IsTracked = _motionParams[playerId].IsTracked,

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

                SpineMidPosition = new Vector3(
                    _motionParams[playerId].SpineMidPosition.x,
                    _motionParams[playerId].SpineMidPosition.y,
                    -_motionParams[playerId].SpineMidPosition.z)
            };

            return motionParam;
        }

        public MotionParam GetCombineMotionParam(int playerCount)
        {
            if (playerCount <= 0 || playerCount > GameConst.MaxPlayerCount) return default;

            int leftPlayerId = 0;
            int rightPlayerId = playerCount == 1 ? 0 : 1;
            int lowerPlayerId = playerCount switch
            {
                1 => 0, 
                2 => 1,
                3 => 2,
                _ => throw new ArgumentOutOfRangeException(nameof(playerCount), playerCount, null)
            };

            return new MotionParam()
            {
                IsTracked = _motionParams[lowerPlayerId].IsTracked,

                ElbowLeftRotation = _motionParams[leftPlayerId].ElbowLeftRotation *
                                    Quaternion.AngleAxis(-180f, Vector3.up) *
                                    Quaternion.AngleAxis(-90f, Vector3.forward),
                WristLeftRotation = _motionParams[leftPlayerId].WristLeftRotation *
                                    Quaternion.AngleAxis(-90f, Vector3.forward),
                HandLeftRotation = _motionParams[leftPlayerId].HandLeftRotation,

                ElbowRightRotation = _motionParams[rightPlayerId].ElbowRightRotation *
                                     Quaternion.AngleAxis(90f, Vector3.forward),
                WristRightRotation = _motionParams[rightPlayerId].WristRightRotation *
                                     Quaternion.AngleAxis(90f, Vector3.forward),
                HandRightRotation = _motionParams[rightPlayerId].HandRightRotation,

                SpineMidRotation = _motionParams[lowerPlayerId].SpineMidRotation *
                                   Quaternion.AngleAxis(-180f, Vector3.up),
                KneeLeftRotation = _motionParams[lowerPlayerId].KneeLeftRotation *
                                   Quaternion.AngleAxis(-180f, Vector3.right) * Quaternion.AngleAxis(90f, Vector3.up),
                AnkleLeftRotation = _motionParams[lowerPlayerId].AnkleLeftRotation *
                                    Quaternion.AngleAxis(180f, Vector3.right) * Quaternion.AngleAxis(90f, Vector3.up),
                KneeRightRotation = _motionParams[lowerPlayerId].KneeRightRotation *
                                    Quaternion.AngleAxis(-180f, Vector3.right) * Quaternion.AngleAxis(-90f, Vector3.up),
                AnkleRightRotation = _motionParams[lowerPlayerId].AnkleRightRotation *
                                     Quaternion.AngleAxis(180f, Vector3.right) * Quaternion.AngleAxis(-90f, Vector3.up),

                SpineMidPosition = new Vector3(
                    _motionParams[lowerPlayerId].SpineMidPosition.x,
                    _motionParams[lowerPlayerId].SpineMidPosition.y,
                    -_motionParams[lowerPlayerId].SpineMidPosition.z)
            };
        }
    }
}