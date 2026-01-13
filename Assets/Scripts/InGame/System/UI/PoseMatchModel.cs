using System;
using InGame.Character;
using UniRx;
using UnityEngine;

namespace InGame.System.UI
{
    public class PoseMatchModel
    {
        private readonly ReactiveProperty<float> _poseMatchRate = new(0f);
        private readonly ReactiveProperty<float> _poseMatchLimitTimeRate = new(1f);
        private readonly Subject<bool> _onMatchSuccess = new();
        private readonly PlayerCore.MovementTransforms _transforms;
        private readonly float _limitTime;
        private float _currentTime;
        private bool _isMatched;
        private PoseData _poseData;

        public IReadOnlyReactiveProperty<float> PoseMatchRate => _poseMatchRate;
        public IReadOnlyReactiveProperty<float> PoseMatchLimitTime => _poseMatchLimitTimeRate;
        public IObservable<bool> OnMatchSuccess => _onMatchSuccess;

        public PoseMatchModel(PlayerCore.MovementTransforms transforms, float limitTime)
        {
            _transforms = transforms;
            _limitTime = limitTime;
        }

        /// <summary>
        /// マッチング対象のPoseDataを設定
        /// </summary>
        public void SetPoseData(PoseData poseData)
        {
            _poseData = poseData;
        }

        public void UpdatePoseMatchRate()
        {
            var poseMatchCount = PoseMatchSystem.MatchPose(_poseData, _transforms);
            var rate = (float)poseMatchCount / PoseMatchSystem.MaxMatchValue;
            _poseMatchRate.Value = rate;

            if (rate >= 1f)
            {
                _isMatched = true;
                _onMatchSuccess.OnNext(true);
            }
        }
        
        public void UpdateLimitTime(float deltaTime)
        {
            if (_isMatched || _currentTime <= 0) return;

            _currentTime -= deltaTime;
            var timeRate = Mathf.Clamp01(_currentTime / _limitTime);
            _poseMatchLimitTimeRate.Value = timeRate;

            if (timeRate <= 0f)
            {
                _onMatchSuccess.OnNext(false);
            }
        }

        public void Reset()
        {
            _currentTime = _limitTime;
            _isMatched = false;
        }
    }
}