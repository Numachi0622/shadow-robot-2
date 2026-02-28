using System;
using System.Collections.Generic;
using System.Linq;
using InGame.Character;
using InGame.Message;
using MessagePipe;
using SynMotion;
using UniRx;
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
        private readonly TextureRegistry _textureRegistry;
        private readonly CharacterPrefabs _characterPrefabs;
        private readonly ISubscriber<SpawnCharacterMessage> _spawnSubscriber;
        private readonly ISubscriber<CreateBuildingMessage> _createBuildingSubscriber;
        private readonly ISubscriber<DespawnCharacterMessage> _despawnSubscriber;
        private readonly ISubscriber<BuildingDestroyedMessage> _buildingDestroyedSubscriber;
        private readonly ISubscriber<EnemyDestroyedMessage> _enemyDestroyedSubscriber;
        private readonly IPublisher<AreaId, BuildingCountChangeMessage> _buildingCountChangePublisher;
        private readonly IPublisher<StateChangeMessage> _stateChangePublisher;
        private readonly KillRecordManager _killRecordManager;
        private readonly HashSet<AreaId> _allBuildingDestroyedAreas = new();
        private IDisposable _subscription;

        [Inject]
        public CharacterSpawner(
            CharacterFactory factory,
            CharacterRegistry registry,
            SynMotionSystem synMotion,
            TextureRegistry textureRegistry,
            CharacterPrefabs characterPrefabs,
            ISubscriber<SpawnCharacterMessage> spawnSubscriber,
            ISubscriber<CreateBuildingMessage> createBuildingSubscriber,
            ISubscriber<DespawnCharacterMessage> playerDespawnSubscriber,
            ISubscriber<BuildingDestroyedMessage> buildingDestroyedSubscriber,
            ISubscriber<EnemyDestroyedMessage> enemyDestroyedSubscriber,
            IPublisher<AreaId, BuildingCountChangeMessage> buildingCountChangePublisher,
            IPublisher<StateChangeMessage> stateChangePublisher,
            KillRecordManager killRecordManager)
        {
            _factory = factory;
            _registry = registry;
            _synMotion = synMotion;
            _textureRegistry = textureRegistry;
            _spawnSubscriber = spawnSubscriber;
            _characterPrefabs = characterPrefabs;
            _createBuildingSubscriber = createBuildingSubscriber;
            _despawnSubscriber = playerDespawnSubscriber;
            _buildingDestroyedSubscriber = buildingDestroyedSubscriber;
            _enemyDestroyedSubscriber = enemyDestroyedSubscriber;
            _buildingCountChangePublisher = buildingCountChangePublisher;
            _stateChangePublisher = stateChangePublisher;
            _killRecordManager = killRecordManager;
        }

        public void Initialize()
        {
            var bag = DisposableBag.CreateBuilder();
            bag.Add(_spawnSubscriber.Subscribe(OnSpawnRequested));
            bag.Add(_despawnSubscriber.Subscribe(OnDespawnRequested));
            bag.Add(_createBuildingSubscriber.Subscribe(CreateBuilding));
            bag.Add(_buildingDestroyedSubscriber.Subscribe(OnBuildingDestroyed));
            bag.Add(_enemyDestroyedSubscriber.Subscribe(OnEnemyDestroyed));

            // テクスチャ変更時にPlayerCoreに反映
            bag.Add(_textureRegistry.OnPlayerTextureChanged.Subscribe(x => ApplyTextureToPlayer(x.playerId, x.context)));

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
            CharacterCore character;
            if (message.CharacterType == CharacterType.Player)
            {
                character = _factory.Create<PlayerCore>(message.Position, message.Rotation);
            }
            else
            {
                character = _factory.Create(_characterPrefabs.CombinePlayerPrefab, message.Position, message.Rotation);
            }
            if (character == null || character is not PlayerCore player) return;
            
            player.Initialize(message.CharacterId, _synMotion, message.TotalPlayerCount);

            // テクスチャ反映
            var context = message.CharacterType == CharacterType.CombinePlayer
                ? _textureRegistry.GetCombineTextureContext(_killRecordManager.GetTopKillerCharacterId(), message.TotalPlayerCount)
                : _textureRegistry.GetTextureContext(message.CharacterId);
            player.SetTexture(context);
            
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
                    normalEnemy.SetPlayType(message.TotalPlayerCount == 1 ? PlayType.Solo : PlayType.Multi);
                }
                _registry.Register(character, message.AreaId);
            }
            else if (message.CharacterType == CharacterType.BossEnemy)
            {
                var character = _factory.Create<BossEnemyCore>(message.Position, message.Rotation);
                if (character is BossEnemyCore bossEnemy)
                {
                    bossEnemy.SetTotalPlayerCount(message.TotalPlayerCount);
                }
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
                _allBuildingDestroyedAreas.Add(message.AreaId);
                if (_allBuildingDestroyedAreas.Count >= _registry.Buildings.Count)
                {
                    _stateChangePublisher.Publish(new StateChangeMessage(GameStateType.GameOver));   
                }
            }
        }

        private void OnEnemyDestroyed(EnemyDestroyedMessage message)
        {
            _registry.Remove(message.Enemy, message.AreaId);
        }

        private void ApplyTextureToPlayer(int playerId, ITextureContext context)
        {
            var players = _registry.GetAllPlayers();
            var playerCore = players
                .OfType<PlayerCore>()
                .FirstOrDefault(p => p.PlayerId.Value == playerId);

            if (playerCore == null) return;
            
            playerCore.SetTexture(context);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
