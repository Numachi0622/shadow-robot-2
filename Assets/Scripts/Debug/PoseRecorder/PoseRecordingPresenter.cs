using System.IO;
using Cysharp.Threading.Tasks;
using InGame.Character;
using UniRx;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace ShadowRobotDebug
{
    public class PoseRecordingPresenter : MonoBehaviour
    {
        [SerializeField] private PoseRecordingView _view;
        private PoseRecorder _recorder;

        public void Initialize(PlayerCore.MovementTransforms transforms)
        {
            _recorder = new PoseRecorder(transforms);
            
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
            await _recorder.StartRecording(recordInfo.fileName, recordInfo.recordTime);
            _view.ResetButtonView();
        }
    }
}