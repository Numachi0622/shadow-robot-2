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
        private readonly IPublisher<CharacterSpawnRequestMessage> _spawnRequestPublisher;
        private readonly IPublisher<CharacterDespawnRequestMessage> _despawnRequestPublisher;
        private readonly IPublisher<ConnectionRecoverMessage> _connectionRecoverPublisher;
        private readonly CompositeDisposable _subscriptions;
        
        public IReadOnlyList<int> ConnectedPlayerIds => _connectedPlayerIds;

        public MotionRegistry(
            PlayerSpawnSettings spawnSettings,
            IPublisher<CharacterSpawnRequestMessage> spawnRequestPublisher,
            IPublisher<CharacterDespawnRequestMessage> despawnRequestPublisher,
            IPublisher<ConnectionRecoverMessage> connectionRecoverPublisher)
        {
            _subscriptions = new CompositeDisposable();
            _spawnSettings = spawnSettings;
            _spawnRequestPublisher = spawnRequestPublisher;
            _despawnRequestPublisher = despawnRequestPublisher;
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
            var spawnMessage = new SpawnCharacterMessage(
                playerId,
                CharacterType.Player,
                pos,
                Quaternion.identity
            );
            _spawnRequestPublisher?.Publish(new CharacterSpawnRequestMessage(spawnMessage));
            
            _connectionRecoverPublisher.Publish(new ConnectionRecoverMessage(playerId));
        }
        
        private void OnRemoved(CollectionRemoveEvent<int> removeEvent)
        {
            var playerId = removeEvent.Value;
            var despawnMessage = new DespawnCharacterMessage(new CharacterId(playerId));
            _despawnRequestPublisher?.Publish(new CharacterDespawnRequestMessage(despawnMessage));
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }
    }
}