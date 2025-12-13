using System;
using InGame.Message;
using MessagePipe;
using Utility;
using VContainer;
using VContainer.Unity;

namespace InGame.System
{
    public class KillRecordManager : IInitializable, IDisposable
    {
        private readonly ISubscriber<EnemyDestroyedMessage> _enemyDestroyedSubscriber;
        private readonly ISubscriber<InitGameMessage> _initGameSubscriber;
        private readonly IPublisher<StateChangeMessage> _stateChangePublisher;
        private int _playerCount = -1;
        private int _currentKillCount = 0;
        private int _requiredKillCount;
        private IDisposable _subscription;
        
        [Inject]
        public KillRecordManager(
            ISubscriber<EnemyDestroyedMessage> enemyDestroyedSubscriber,
            ISubscriber<InitGameMessage> initGameSubscriber,
            IPublisher<StateChangeMessage> stateChangePublisher)
        {
            _enemyDestroyedSubscriber = enemyDestroyedSubscriber;
            _initGameSubscriber = initGameSubscriber;
            _stateChangePublisher = stateChangePublisher;
        }
        
        public void Initialize()
        {
            var builder = DisposableBag.CreateBuilder();
            _initGameSubscriber.Subscribe(OnGameInit).AddTo(builder);
            _enemyDestroyedSubscriber.Subscribe(RecordKillData).AddTo(builder);
            _subscription = builder.Build();
        }
        
        private void OnGameInit(InitGameMessage message)
        {
            _playerCount = message.PlayerCount;
            _requiredKillCount = _playerCount * GameConst.RequiredNormalEnemyKillCount;
        }

        private void RecordKillData(EnemyDestroyedMessage message)
        {
            if (_playerCount == -1) return;

            _currentKillCount++;
            if (_currentKillCount >= _requiredKillCount)
            {
                _stateChangePublisher.Publish(new StateChangeMessage(GameStateType.BossBattle));
            }
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}