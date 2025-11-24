using System;
using InGame.Character;
using InGame.Message;
using MessagePipe;
using SynMotion;
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
        private readonly SynMotionSystem _synMotion;
        private readonly ISubscriber<SpawnCharacterMessage> _spawnSubscriber;
        private readonly ISubscriber<DespawnCharacterMessage> _despawnSubscriber;
        private IDisposable _subscription;

        [Inject]
        public CharacterSpawner(
            CharacterFactory factory,
            CharacterRegistry registry,
            SynMotionSystem synMotion,
            ISubscriber<SpawnCharacterMessage> spawnSubscriber,
            ISubscriber<DespawnCharacterMessage> playerDespawnSubscriber)
        {
            _factory = factory;
            _registry = registry;
            _synMotion = synMotion;
            _spawnSubscriber = spawnSubscriber;
            _despawnSubscriber = playerDespawnSubscriber;
        }

        public void Initialize()
        {
            var bag = DisposableBag.CreateBuilder();
            bag.Add(_spawnSubscriber.Subscribe(OnSpawnRequested));
            bag.Add(_despawnSubscriber.Subscribe(OnDespawnRequested));
            
            _subscription = bag.Build();
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
            var character = _factory.Create<PlayerCore>(message.Position, message.Rotation);
            if (character is not PlayerCore player) return;
            
            player.Initialize(message.CharacterId, _synMotion);
            _registry.Register(player);
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
        
        private void OnDespawnRequested(DespawnCharacterMessage message)
        {
            if (message.CharacterId.Value == -1) return;
            _registry.RemoveAt(message.CharacterId.Value);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
