using Cysharp.Threading.Tasks;
using InGame.Character;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace ShadowRobotDebug
{
    public class PoseRecordingPresenter : MonoBehaviour
    {
        [SerializeField] private PoseRecordingView _view;
        private PoseRecorder _recorder;
        private PoseData _prevPoseData;

        public void Initialize(PlayerCore.MovementTransforms transforms)
        {
            _recorder = new PoseRecorder(transforms);
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
    }
}