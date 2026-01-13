using System;
using Cysharp.Threading.Tasks;
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
        private IDisposable _subscription;
        
        [Inject]
        public void Construct(
            // Inject用
            ISubscriber<AreaId, BuildingCountChangeMessage> buildingCountChangeSubscriber,
            // 購読のみ
            ISubscriber<InitGameMessage> initGameSubscriber)
        {
            _buildingCountChangeSubscriber = buildingCountChangeSubscriber;
            
            var bag = DisposableBag.CreateBuilder();
            bag.Add(initGameSubscriber.Subscribe(OnInitGame));
            
            _subscription = bag.Build();
        }

        private void OnInitGame(InitGameMessage message)
        {
            _buildingPresenter.Initialize(message.PlayerCount, _buildingCountChangeSubscriber);
        }

        #region PoseMatchEvent
        public async UniTask ShowPoseMatchViewAsync()
        {
            await _poseMatchPresenter.ShowAsync();
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