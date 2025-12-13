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
        private readonly IPublisher<AreaId, BuildingCountChangeMessage> _buildingCountChangePublisher;
        private readonly IPublisher<AllBuildingsDestroyMessage> _allBuildingsDestroyPublisher;
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
            IPublisher<AreaId, BuildingCountChangeMessage> buildingCountChangePublisher,
            IPublisher<AllBuildingsDestroyMessage> allBuildingsDestroyPublisher)
        {
            _factory = factory;
            _registry = registry;
            _synMotion = synMotion;
            _spawnSubscriber = spawnSubscriber;
            _createBuildingSubscriber = createBuildingSubscriber;
            _despawnSubscriber = playerDespawnSubscriber;
            _buildingDestroyedSubscriber = buildingDestroyedSubscriber;
            _buildingCountChangePublisher = buildingCountChangePublisher;
            _allBuildingsDestroyPublisher = allBuildingsDestroyPublisher;
        }

        public void Initialize()
        {
            var bag = DisposableBag.CreateBuilder();
            bag.Add(_spawnSubscriber.Subscribe(OnSpawnRequested));
            bag.Add(_despawnSubscriber.Subscribe(OnDespawnRequested));
            bag.Add(_createBuildingSubscriber.Subscribe(CreateBuilding));
            bag.Add(_buildingDestroyedSubscriber.Subscribe(OnBuildingDestroyed));

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
                (character as NormalEnemyCore)?.SetAreaId(message.AreaId);
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
                _allBuildingsDestroyPublisher.Publish(new AllBuildingsDestroyMessage());
            }
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
