using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using InGame.Character;
using InGame.Message;
using MessagePipe;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;
using VContainer;

namespace InGame.System.UI
{
    public class InGameUIController : MonoBehaviour
    {
        [SerializeField] private NormalBattlePresenter _normalBattlePresenter;
        [SerializeField] private BuildingPresenter _buildingPresenter;
        [SerializeField] private BossBattlePresenter _bossBattlePresenter;
        [SerializeField] private PoseMatchPresenter _poseMatchPresenter;
        [SerializeField] private GameOverPresenter _gameOverPresenter;
        [SerializeField] private ResultPresenter _resultPresenter;
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
            ISubscriber<BossBattleStartMessage> bossBattleStartSubscriber,
            ISubscriber<ShowWarningMessage> showWarningSubscriber,
            ISubscriber<UpdateKillCountMessage> updateKillCountSubscriber)
        {
            _buildingCountChangeSubscriber = buildingCountChangeSubscriber;
            _poseMatchEventResultPublisher = poseMatchEventResultPublisher;
            _characterRegistry = characterRegistry;

            var bag = DisposableBag.CreateBuilder();
            initGameSubscriber.Subscribe(OnInitGame).AddTo(bag);
            normalBattleEndSubscriber.Subscribe(_ => OnNormalBattleEnd()).AddTo(bag);
            bossBattleStartSubscriber.Subscribe(_ => OnBossBattleStart()).AddTo(bag);
            showWarningSubscriber.Subscribe(OnShowWarning).AddTo(bag);
            updateKillCountSubscriber.Subscribe(OnUpdateKillCount).AddTo(bag);

            _subscription = bag.Build();
        }

        public void Initialize()
        {
            _normalBattlePresenter.Initialize();
            _gameOverPresenter.Initialize();
            _resultPresenter.Initialize();
        }

        #region NormalBattleEvent
        private async void OnInitGame(InitGameMessage message)
        {
            _buildingPresenter.InitializeAndShow(
                message.PlayerCount,
                message.BuildingCoutPerArea,
                _buildingCountChangeSubscriber
            );

            await _normalBattlePresenter.ShowAndHideRemainingEnemiesCountViewAsync(message.PlayerCount * GameConst.RequiredNormalEnemyKillCount);
        }
        
        public async UniTask ShowAndHideBattleStartViewAsync()
        {
            await _normalBattlePresenter.ShowAndHideBattleStartViewAsync();
        }
        
        private void OnNormalBattleEnd()
        {
            _buildingPresenter.Hide();
            _normalBattlePresenter.EndNormalBattle();
        }

        private void OnUpdateKillCount(UpdateKillCountMessage message)
        {
            _normalBattlePresenter.SetRemainingEnemiesCount(message);
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

        public async UniTask ShowWarningAsync(WarningType warningType, int fadeCount = 3, float fadeDuration = 0.5f)
        {
            await _bossBattlePresenter.ShowWarningAsync(warningType, fadeCount, fadeDuration);
        }
        
        private void OnShowWarning(ShowWarningMessage message)
        {
            ShowWarningAsync(message.WarningType, message.FadeCount, message.FadeDuration).Forget();
        }
        
        public async UniTask ShowAndHideMissionPopupAsync()
        {
            await _bossBattlePresenter.ShowAndHideMissionPopupAsync();
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
        
        public void ShowGameOverView()
        {
            _gameOverPresenter.ShowGameOverView();
        }
        
        public async UniTask PlayResultSequenceAsync(int buildingCount)
        {
            await _resultPresenter.PlayResultSequenceAsync(buildingCount);
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }
}