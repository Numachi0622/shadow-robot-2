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
        private readonly ISubscriber<BuildingDestroyedMessage> _buildingDestroyedSubscriber;
        private readonly ISubscriber<EnemyDestroyedMessage> _enemyDestroyedSubscriber;
        private readonly IPublisher<AreaId, BuildingCountChangeMessage> _buildingCountChangePublisher;
        private readonly IPublisher<StateChangeMessage> _stateChangePublisher;
        private IDisposable _subscription;

        [Inject]
        public CharacterSpawner(
            CharacterFactory factory,
            CharacterRegistry registry,
            SynMotionSystem synMotion,
            ISubscriber<SpawnCharacterMessage> spawnSubscriber,
            ISubscriber<CreateBuildingMessage> createBuildingSubscriber,
            ISubscriber<DespawnCharacterMessage> playerDespawnSubscriber,
            ISubscriber<BuildingDestroyedMessage> buildingDestroyedSubscriber,
            ISubscriber<EnemyDestroyedMessage> enemyDestroyedSubscriber,
            IPublisher<AreaId, BuildingCountChangeMessage> buildingCountChangePublisher,
            IPublisher<StateChangeMessage> stateChangePublisher)
        {
            _factory = factory;
            _registry = registry;
            _synMotion = synMotion;
            _spawnSubscriber = spawnSubscriber;
            _createBuildingSubscriber = createBuildingSubscriber;
            _despawnSubscriber = playerDespawnSubscriber;
            _buildingDestroyedSubscriber = buildingDestroyedSubscriber;
            _enemyDestroyedSubscriber = enemyDestroyedSubscriber;
            _buildingCountChangePublisher = buildingCountChangePublisher;
            _stateChangePublisher = stateChangePublisher;
        }

        public void Initialize()
        {
            var bag = DisposableBag.CreateBuilder();
            bag.Add(_spawnSubscriber.Subscribe(OnSpawnRequested));
            bag.Add(_despawnSubscriber.Subscribe(OnDespawnRequested));
            bag.Add(_createBuildingSubscriber.Subscribe(CreateBuilding));
            bag.Add(_buildingDestroyedSubscriber.Subscribe(OnBuildingDestroyed));
            bag.Add(_enemyDestroyedSubscriber.Subscribe(OnEnemyDestroyed));

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
                case CharacterType.CombinePlayer:
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
            
            player.Initialize(message.CharacterId, _synMotion, message.TotalPlayerCount);
            _registry.Register(player);
        }

        private void SpawnEnemy(SpawnCharacterMessage message)
        {
            if (message.CharacterType == CharacterType.NormalEnemy)
            {
                var character = _factory.Create<NormalEnemyCore>(message.Position, message.Rotation);
                character.Initialize();
                if (character is NormalEnemyCore normalEnemy)
                {
                    normalEnemy.SetAreaId(message.AreaId);
                    normalEnemy.SetTarget(message.TargetType);
                }
                _registry.Register(character, message.AreaId);
            }
            else if (message.CharacterType == CharacterType.BossEnemy)
            {
                var character = _factory.Create<BossEnemyCore>(message.Position, message.Rotation);
                character.Initialize();
                _registry.Register(character, message.AreaId);
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
            (building as BuildingCore)?.SetAreaId(message.AreaId);
            _registry.Register(building, message.AreaId);
        }
        
        private void OnDespawnRequested(DespawnCharacterMessage message)
        {
            if (message.CharacterId.Value == -1) return;
            _registry.RemoveAt(message.CharacterId.Value);
        }

        private void OnBuildingDestroyed(BuildingDestroyedMessage message)
        {
            _registry.Remove(message.Building, message.AreaId);

            var remainingCount = _registry.GetBuildings(message.AreaId)?.Count ?? 0;
            _buildingCountChangePublisher.Publish(message.AreaId, new BuildingCountChangeMessage(remainingCount));

            if (remainingCount == 0)
            {
                _stateChangePublisher.Publish(new StateChangeMessage(GameStateType.GameOver));
            }
        }

        private void OnEnemyDestroyed(EnemyDestroyedMessage message)
        {
            _registry.Remove(message.Enemy, message.AreaId);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
