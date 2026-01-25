using System;
using System.Collections.Generic;
using InGame.Message;
using MessagePipe;
using UniRx;
using UnityEngine;
using Utility;
using VContainer.Unity;

namespace InGame.Character
{
    public class MotionRegistry : IInitializable, IDisposable
    {
        private readonly ReactiveCollection<int> _connectedPlayerIds = new();
        private readonly PlayerSpawnSettings _spawnSettings;
        private readonly IPublisher<SpawnCharacterMessage> _spawnPublisher;
        private readonly IPublisher<DespawnCharacterMessage> _despawnPublisher;
        private readonly IPublisher<ConnectionRecoverMessage> _connectionRecoverPublisher;
        private readonly CompositeDisposable _subscriptions;
        
        public IReadOnlyList<int> ConnectedPlayerIds => _connectedPlayerIds;

        public MotionRegistry(
            PlayerSpawnSettings spawnSettings,
            IPublisher<SpawnCharacterMessage> spawnPublisher,
            IPublisher<DespawnCharacterMessage> despawnPublisher,
            IPublisher<ConnectionRecoverMessage> connectionRecoverPublisher)
        {
            _subscriptions = new CompositeDisposable();
            _spawnSettings = spawnSettings;
            _spawnPublisher = spawnPublisher;
            _despawnPublisher = despawnPublisher;
            _connectionRecoverPublisher = connectionRecoverPublisher;
        }
        
        public void Initialize()
        {
            _connectedPlayerIds.ObserveAdd().Subscribe(OnAdded).AddTo(_subscriptions);
            _connectedPlayerIds.ObserveRemove().Subscribe(OnRemoved).AddTo(_subscriptions);
        }

        public void TryAdd(int id)
        {
            if (_connectedPlayerIds.Count >= GameConst.MaxPlayerCount) return;
            _connectedPlayerIds.Add(id);
        }

        public void Remove(int id) => _connectedPlayerIds.Remove(id);

        private void OnAdded(CollectionAddEvent<int> addEvent)
        {
            if (_spawnSettings == null)
            {
                Debug.LogError("[MotionRegistry] PlayerSpawnSettings is not set.");
                return;
            };
            
            var playerId = new CharacterId(addEvent.Value);
            var pos = _spawnSettings.SpawnPositions[playerId.Value];
            _spawnPublisher?.Publish(new SpawnCharacterMessage(
                playerId,
                CharacterType.Player,
                pos,
                Quaternion.identity
            ));
            
            _connectionRecoverPublisher.Publish(new ConnectionRecoverMessage(playerId));
        }
        
        private void OnRemoved(CollectionRemoveEvent<int> removeEvent)
        {
            var playerId = removeEvent.Value;
            _despawnPublisher?.Publish(new DespawnCharacterMessage(new CharacterId(playerId)));
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }
    }
}