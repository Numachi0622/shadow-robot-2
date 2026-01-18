using System;
using System.Linq;
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
        private CharacterRegistry _characterRegistry;
        private IDisposable _subscription;
        
        [Inject]
        public void Construct(
            // Inject用
            ISubscriber<AreaId, BuildingCountChangeMessage> buildingCountChangeSubscriber,
            IPublisher<PoseMatchEventResultMessage> poseMatchEventResultPublisher,
            CharacterRegistry characterRegistry,
            // 購読のみ
            ISubscriber<InitGameMessage> initGameSubscriber)
        {
            _buildingCountChangeSubscriber = buildingCountChangeSubscriber;
            _poseMatchEventResultPublisher = poseMatchEventResultPublisher;
            _characterRegistry = characterRegistry;
            
            var bag = DisposableBag.CreateBuilder();
            bag.Add(initGameSubscriber.Subscribe(OnInitGame));
            
            _subscription = bag.Build();
        }

        public void Initialize()
        {
        }

        private void OnInitGame(InitGameMessage message)
        {
            _buildingPresenter.Initialize(message.PlayerCount, _buildingCountChangeSubscriber);
        }

        #region PoseMatchEvent
        public async UniTask ShowPoseMatchViewAsync(PoseData poseData)
        {
            if (!_poseMatchPresenter.Initialized)
            {
                var player = _characterRegistry.GetAllPlayers().FirstOrDefault() as PlayerCore;
                if (player == null)
                {
                    Debug.LogError("[InGameUIController] No player found for PoseMatchPresenter initialization.");
                    return;   
                }
                var transforms = player.Transforms;
                _poseMatchPresenter.Initialize(transforms, _poseMatchEventResultPublisher);
            }
            
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