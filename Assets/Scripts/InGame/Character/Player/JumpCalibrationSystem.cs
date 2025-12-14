using Cysharp.Threading.Tasks;
using InGame.System;
using SynMotion;
using UnityEngine;

namespace InGame.Character
{
    public class JumpCalibrationSystem
    {
        private readonly SynMotionSystem _synMotion;
        private readonly CharacterId _playerId;

        public JumpCalibrationSystem(SynMotionSystem synMotion, CharacterId playerId)
        {
            _synMotion = synMotion;
            _playerId = playerId;
        }

        public async UniTask<float> CalibrateGroundHeight(int attemptCount = 60)
        {
            float totalHeight = 0f;
            int validSamples = 0;

            for (int i = 0; i < attemptCount; i++)
            {
                await UniTask.Yield(); // 1フレーム待つ

                var motionParam = _synMotion.GetMotionParam(_playerId.Value);
                if (motionParam.IsTracked)
                {
                    totalHeight += motionParam.SpineMidPosition.y;
                    validSamples++;
                }
            }

            float averageHeight = validSamples > 0 ? totalHeight / validSamples : 0f;
            Debug.Log($"[JumpCalibration] Player {_playerId.Value}: Calibrated ground height = {averageHeight} ({validSamples}/{attemptCount} valid samples)");
            return averageHeight;
        }
    }
}