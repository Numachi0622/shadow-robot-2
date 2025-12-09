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
        private readonly ISubscriber<CreateBuildingMessage> _createBuildingSubscriber;
        private readonly ISubscriber<DespawnCharacterMessage> _despawnSubscriber;
        private IDisposable _subscription;

        [Inject]
        public CharacterSpawner(
            CharacterFactory factory,
            CharacterRegistry registry,
            SynMotionSystem synMotion,
            ISubscriber<SpawnCharacterMessage> spawnSubscriber,
            ISubscriber<CreateBuildingMessage> createBuildingSubscriber,
            ISubscriber<DespawnCharacterMessage> playerDespawnSubscriber)
        {
            _factory = factory;
            _registry = registry;
            _synMotion = synMotion;
            _spawnSubscriber = spawnSubscriber;
            _createBuildingSubscriber = createBuildingSubscriber;
            _despawnSubscriber = playerDespawnSubscriber;
        }

        public void Initialize()
        {
            var bag = DisposableBag.CreateBuilder();
            bag.Add(_spawnSubscriber.Subscribe(OnSpawnRequested));
            bag.Add(_despawnSubscriber.Subscribe(OnDespawnRequested));
            bag.Add(_createBuildingSubscriber.Subscribe(CreateBuilding));
            
            _subscription = bag.Build();
        }

        /// <summary>
        /// キャラクター生成リクエストを受け取る
        /// </summary>
        private void OnSpawnRequested(SpawnCharacterMessage message)
        {
            switch (message.CharacterType)
            {
                case CharacterType.Player:
                    SpawnPlayer(message);
                    break;
                case CharacterType.NormalEnemy:
                case CharacterType.BossEnemy:
                    SpawnEnemy(message);
                    break;
                case CharacterType.Building:
                    break;
            }
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

        private void CreateBuilding(CreateBuildingMessage message)
        {
            var building = _factory.Create(
                message.BuildingCore,
                message.Position,
                message.Rotation,
                message.Parent
            );
            building.Initialize();
            _registry.Register(building);
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
