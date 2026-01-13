using System;
using Cysharp.Threading.Tasks;
using InGame.Character;
using InGame.Message;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace InGame.System.UI
{
    public class InGameUIController : MonoBehaviour
    {
        [SerializeField] private BuildingPresenter _buildingPresenter;
        [SerializeField] private PoseMatchPresenter _poseMatchPresenter;
        
        private ISubscriber<AreaId, BuildingCountChangeMessage> _buildingCountChangeSubscriber;
        private IPublisher<PoseMatchEventResultMessage> _poseMatchEventResultPublisher;
        private IDisposable _subscription;
        
        [Inject]
        public void Construct(
            // Inject用
            ISubscriber<AreaId, BuildingCountChangeMessage> buildingCountChangeSubscriber,
            IPublisher<PoseMatchEventResultMessage> poseMatchEventResultPublisher,
            // 購読のみ
            ISubscriber<InitGameMessage> initGameSubscriber)
        {
            _buildingCountChangeSubscriber = buildingCountChangeSubscriber;
            _poseMatchEventResultPublisher = poseMatchEventResultPublisher;
            
            var bag = DisposableBag.CreateBuilder();
            bag.Add(initGameSubscriber.Subscribe(OnInitGame));
            
            _subscription = bag.Build();
        }

        public void Initialize()
        {
            _poseMatchPresenter.Initialize(_poseMatchEventResultPublisher);
        }

        private void OnInitGame(InitGameMessage message)
        {
            _buildingPresenter.Initialize(message.PlayerCount, _buildingCountChangeSubscriber);
        }

        #region PoseMatchEvent
        public async UniTask ShowPoseMatchViewAsync(PoseData poseData)
        {
            await _poseMatchPresenter.ShowAsync(poseData);
        } 
        
        public async UniTask HidePoseMatchViewAsync()
        {
            await _poseMatchPresenter.HideAsync();
        }
        
        public async UniTask PoseMatchSuccessAnimationAsync()
        {
            await _poseMatchPresenter.PoseMatchSuccessAnimationAsync();
        }
        #endregion

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }
}