using System;
using Cysharp.Threading.Tasks;
using InGame.Character;
using InGame.System;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace ShadowRobotDebug
{
    public class PoseRecordingPresenter : MonoBehaviour
    {
        [SerializeField] private PoseRecordingView _view;
        private PoseRecorder _recorder;
        [SerializeField] private PoseData _prevPoseData;
        private PlayerCore.MovementTransforms _transforms;

        public void Initialize(PlayerCore.MovementTransforms transforms)
        {
            _transforms = transforms;
            _recorder = new PoseRecorder(_transforms);
            _view.Initialize();
            
            _view.RecordStartButton.OnClickAsObservable()
                .Select(_ => _view.RecordInfo())
                .Subscribe(OnRecordStartButtonClicked)
                .AddTo(this);
        }
        
        private async void OnRecordStartButtonClicked((string fileName, float recordTime) recordInfo)
        {
            int currentCount = 3;
            while (currentCount > 0)
            {
                _view.SetButtonView(currentCount.ToString(), Color.white);
                currentCount--;
                await UniTask.Delay(1000);
            }
            
            _view.SetButtonView("Recording...", Color.red);
            _prevPoseData = await _recorder.StartRecording(recordInfo.fileName, recordInfo.recordTime);
            _view.ResetButtonView();

            if (_prevPoseData == null) return;
            var poseDataPath = AssetDatabase.GetAssetPath(_prevPoseData);
            _view.SetPoseDataView(poseDataPath);
        }

        private void Update()
        {
            if (_prevPoseData == null) return;
            var poseMatch = PoseMatchSystem.MatchPose(_prevPoseData, _transforms); 
            _view.SetMatchResultView(poseMatch.ToString());
        }
    }
}