using System;
using InGame.Character;
using InGame.Message;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace InGame.System
{
    /// <summary>
    /// キャラクター生成を管理
    /// SpawnCharacterMessageを購読してCharacterFactoryを呼び出す
    /// </summary>
    public class CharacterSpawner : IInitializable, IDisposable
    {
        private readonly CharacterFactory _factory;
        private readonly CharacterRegistry _registry;
        private readonly ISubscriber<SpawnCharacterMessage> _subscriber;
        private IDisposable _subscription;

        [Inject]
        public CharacterSpawner(
            CharacterFactory factory,
            CharacterRegistry registry,
            ISubscriber<SpawnCharacterMessage> subscriber)
        {
            _factory = factory;
            _registry = registry;
            _subscriber = subscriber;
            Debug.Log("[CharacterSpawner] Constructor called");
        }

        public void Initialize()
        {
            Debug.Log("[CharacterSpawner] Initialize called - Subscribing to SpawnCharacterMessage");
            _subscription = _subscriber.Subscribe(OnSpawnRequested);
            Debug.Log("[CharacterSpawner] Subscribed successfully");
        }

        /// <summary>
        /// キャラクター生成リクエストを受け取る
        /// </summary>
        private void OnSpawnRequested(SpawnCharacterMessage message)
        {
            if (message.CharacterType == CharacterType.Player) SpawnPlayer(message);
            else SpawnEnemy(message);
        }

        private void SpawnPlayer(SpawnCharacterMessage message)
        {
            
        }

        private void SpawnEnemy(SpawnCharacterMessage message)
        {
            if (message.CharacterType == CharacterType.NormalEnemy)
            {
                var character = _factory.Create<NormalEnemyCore>(message.Position, message.Rotation);
                character.Initialize();
                _registry.Register(character);
            }
            else if (message.CharacterType == CharacterType.BossEnemy)
            {
                var character = _factory.Create<BossEnemyCore>(message.Position, message.Rotation);
                character.Initialize();
                _registry.Register(character);
            }
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
