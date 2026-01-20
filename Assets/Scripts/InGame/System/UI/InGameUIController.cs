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
        [SerializeField] private NormalBattleView _normalBattleView;
        [SerializeField] private BuildingPresenter _buildingPresenter;
        [SerializeField] private PoseMatchPresenter _poseMatchPresenter;
        private HitPointPresenter _playerHpPresenter, _bossHpPresenter;
        
        private ISubscriber<AreaId, BuildingCountChangeMessage> _buildingCountChangeSubscriber;
        private IPublisher<PoseMatchEventResultMessage> _poseMatchEventResultPublisher;
        private CharacterRegistry _characterRegistry;
        private IDisposable _subscription;

        private const int HitPointViewHidePositionX = 1000; 
        
        [Inject]
        public void Construct(
            // Inject用
            ISubscriber<AreaId, BuildingCountChangeMessage> buildingCountChangeSubscriber,
            IPublisher<PoseMatchEventResultMessage> poseMatchEventResultPublisher,
            CharacterRegistry characterRegistry,
            // 購読のみ
            ISubscriber<InitGameMessage> initGameSubscriber,
            ISubscriber<NormalBattleEndMessage> normalBattleEndSubscriber,
            ISubscriber<BossBattleStartMessage> bossBattleStartSubscriber)
        {
            _buildingCountChangeSubscriber = buildingCountChangeSubscriber;
            _poseMatchEventResultPublisher = poseMatchEventResultPublisher;
            _characterRegistry = characterRegistry;
            
            var bag = DisposableBag.CreateBuilder();
            initGameSubscriber.Subscribe(OnInitGame).AddTo(bag);
            normalBattleEndSubscriber.Subscribe(_ => OnNormalBattleEnd()).AddTo(bag);
            bossBattleStartSubscriber.Subscribe(_ => OnBossBattleStart()).AddTo(bag);
            
            _subscription = bag.Build();
        }

        public void Initialize()
        {
            _normalBattleView.Initialize();
        }

        #region NormalBattleEvent
        private void OnInitGame(InitGameMessage message)
        {
            _buildingPresenter.InitializeAndShow(message.PlayerCount, _buildingCountChangeSubscriber);
        }
        
        public async UniTask ShowAndHideBattleStartViewAsync()
        {
            await _normalBattleView.ShowAndHideBattleStartViewAsync();
        }
        
        private void OnNormalBattleEnd()
        {
            _buildingPresenter.Hide();
        }
        #endregion

        #region BossBattleEvent
        private void OnBossBattleStart()
        {
            var player = _characterRegistry.GetAllPlayers().FirstOrDefault() as PlayerCore;
            var boss = _characterRegistry.GetAllEnemies().FirstOrDefault() as BossEnemyCore;
            if (!player || !boss)
            {
                Debug.LogError("[InGameUIController] Player or Boss not found for HitPointPresenter initialization.");
                return;
            }

            _playerHpPresenter = player.HpPresenter;
            _playerHpPresenter.Show();
            _bossHpPresenter = boss.HpPresenter;
            _bossHpPresenter.Show();
        }

        public async UniTask ShowHitPointViewAsync()
        {
            var playerTask = _playerHpPresenter.ShowAsync(new HitPointVisibilityContext(0));
            var bossTask = _bossHpPresenter.ShowAsync(new HitPointVisibilityContext(0));
            await UniTask.WhenAll(playerTask, bossTask);
        }
        
        public async UniTask HideHitPointViewAsync()
        {
            var playerTask = _playerHpPresenter.HideAsync(new HitPointVisibilityContext(-HitPointViewHidePositionX));
            var bossTask = _bossHpPresenter.HideAsync(new HitPointVisibilityContext(HitPointViewHidePositionX));
            await UniTask.WhenAll(playerTask, bossTask);
        }

        #endregion

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