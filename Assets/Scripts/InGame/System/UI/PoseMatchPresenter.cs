using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using InGame.Character;
using InGame.Message;
using MessagePipe;

namespace InGame.System.UI
{
    public class PoseMatchPresenter : MonoBehaviour
    {
        [SerializeField] private PoseMatchView _view;
        [SerializeField] private float _limitTime = 10f;
        private PoseMatchModel _model;
        private IPublisher<PoseMatchEventResultMessage> _poseMatchEventResultPublisher;
        private bool _initialized = false;
        
        public bool Initialized => _initialized;

        public void Initialize(
            PlayerCore.MovementTransforms transforms,
            IPublisher<PoseMatchEventResultMessage> poseMatchEventResultPublisher)
        {
            _model = new PoseMatchModel(transforms, _limitTime);
            _poseMatchEventResultPublisher = poseMatchEventResultPublisher;
            Bind();
            
            _initialized = true;
        }

        private void Bind()
        {
            _model.PoseMatchRate.Subscribe(_view.UpdateMatchGauge).AddTo(this);
            _model.PoseMatchLimitTime.Subscribe(_view.UpdateTimeGauge).AddTo(this);
            _model.OnMatchSuccess.Subscribe(OnMatchSuccess).AddTo(this);
        }

        public async UniTask ShowAsync(PoseData poseData)
        {
            _model.SetPoseData(poseData);
            await _view.ShowAsync();
        }
        
        public async UniTask HideAsync()
        {
            await _view.HideAsync();
        }
        
        public async UniTask PoseMatchSuccessAnimationAsync()
        {
            await _view.PoseMatchSuccessAnimationAsync();
        }
        
        private void OnMatchSuccess(bool isSuccess)
        {
            _poseMatchEventResultPublisher.Publish(new PoseMatchEventResultMessage(isSuccess));
        }
    }
}