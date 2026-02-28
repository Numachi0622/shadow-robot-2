using System;
using System.Collections.Generic;
using System.Linq;
using InGame.Character;
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
        private readonly IPublisher<UpdateKillCountMessage> _updateKillCountPublisher;
        private int _playerCount = -1;
        private int _currentKillCount = 0;
        private int _requiredKillCount;
        private bool _isRecordComplete;
        private IDisposable _subscription;
        private readonly Dictionary<int, int> _killCountByPlayer = new();

        public IReadOnlyDictionary<int, int> KillCountByPlayer => _killCountByPlayer;

        public CharacterId GetTopKillerCharacterId()
        {
            if (_killCountByPlayer.Count == 0) return new CharacterId(0);
            var topId = _killCountByPlayer.Aggregate((a, b) => a.Value >= b.Value ? a : b).Key;
            return new CharacterId(topId);
        }
        
        [Inject]
        public KillRecordManager(
            ISubscriber<EnemyDestroyedMessage> enemyDestroyedSubscriber,
            ISubscriber<InitGameMessage> initGameSubscriber,
            IPublisher<StateChangeMessage> stateChangePublisher,
            IPublisher<UpdateKillCountMessage> updateKillCountPublisher)
        {
            _enemyDestroyedSubscriber = enemyDestroyedSubscriber;
            _initGameSubscriber = initGameSubscriber;
            _stateChangePublisher = stateChangePublisher;
            _updateKillCountPublisher = updateKillCountPublisher;
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
            if (_playerCount == -1 || _isRecordComplete) return;

            var killerId = message.KillerId.Value;
            _killCountByPlayer.TryGetValue(killerId, out var count);
            _killCountByPlayer[killerId] = count + 1;

            _currentKillCount++;
            _updateKillCountPublisher.Publish(new UpdateKillCountMessage(_currentKillCount, _requiredKillCount));

            if (_currentKillCount >= _requiredKillCount)
            {
                _isRecordComplete = true;
                _stateChangePublisher.Publish(new StateChangeMessage(GameStateType.BossBattle));
            }
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}