using System;
using InGame.Character;
using InGame.Environment;
using InGame.Message;
using InGame.System.UI;
using MessagePipe;
using SynMotion;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace InGame.System
{
    public class InGameLifetimeScope : LifetimeScope
    {
        [SerializeField] private CharacterPrefabs _characterPrefabs;
        [SerializeField] private StageReferences _stageReferences;
        [SerializeField] private EnvironmentPrefabs _environmentPrefabs;
        [SerializeField] private PlayerSpawnSettings _playerSpawnSettings;
        [SerializeField] private DeviceSettings _deviceSettings;
        [SerializeField] private MainStageManager _mainStageManager;
        [SerializeField] private InGameUIController _inGameUIController;
        [SerializeField] private HitPointViewList _hitPointViewList;
        
        [SerializeField] private DebugCommand _debugCommand;
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<StateChangeMessage>(options);
            builder.RegisterMessageBroker<SpawnCharacterMessage>(options);
            builder.RegisterMessageBroker<DespawnCharacterMessage>(options);
            builder.RegisterMessageBroker<InitGameMessage>(options);
            builder.RegisterMessageBroker<NormalBattleEndMessage>(options);
            builder.RegisterMessageBroker<BuildingDestroyedMessage>(options);
            builder.RegisterMessageBroker<EnemyDestroyedMessage>(options);
            builder.RegisterMessageBroker<AllEnemyDespawnMessage>(options);
            builder.RegisterMessageBroker<AllPlayerDespawnMessage>(options);
            builder.RegisterMessageBroker<BossBattleStartMessage>(options);
            builder.RegisterMessageBroker<PoseMatchEventStartMessage>(options);
            builder.RegisterMessageBroker<PoseMatchEventResultMessage>(options);
            builder.RegisterMessageBroker<PoseMatchEventEndMessage>(options);

            builder.RegisterMessageBroker<CharacterId, GameStartPlayerInitMessage>(options);
            builder.RegisterMessageBroker<AreaId, BuildingCountChangeMessage>(options);

            // インゲーム基盤システム
            builder.RegisterEntryPoint<InGameCore>().AsSelf();
            builder.RegisterEntryPoint<KillRecordManager>().AsSelf();
            
            // キャラクターのPrefabを登録
            builder.RegisterInstance(_characterPrefabs);
            builder.RegisterInstance(_characterPrefabs.PlayerPrefab);
            builder.RegisterInstance(_characterPrefabs.NormalEnemyPrefab);
            builder.RegisterInstance(_characterPrefabs.BossEnemyPrefab);
            
            // ステージ等のPrefabを登録
            builder.RegisterInstance(_stageReferences);
            //builder.RegisterInstance(_environmentPrefabs);
            builder.RegisterInstance(_mainStageManager);
            
            // キャラクター生成位置登録
            builder.RegisterInstance(_playerSpawnSettings);
            
            // プレイヤー生成
            builder.RegisterInstance(_deviceSettings);
            builder.RegisterEntryPoint<MotionReceiver>().AsSelf();
            builder.Register<SynMotionSystem>(Lifetime.Singleton).AsSelf();

            // キャラクター生成
            builder.Register<CharacterFactory>(Lifetime.Singleton).AsSelf();
            builder.RegisterEntryPoint<CharacterRegistry>().AsSelf();
            builder.RegisterEntryPoint<CharacterSpawner>().AsSelf();
            
            // UIコンポーネント
            builder.RegisterComponent(_inGameUIController);
            builder.RegisterInstance(_hitPointViewList);

            builder.RegisterComponent(_debugCommand);
        }
    }

    [Serializable]
    public class HitPointViewList
    {
        [SerializeField] private HitPointView _playerHitPointView;
        [SerializeField] private HitPointView _bossHitPointView;
        public HitPointView PlayerHitPointView => _playerHitPointView;
        public HitPointView BossHitPointView => _bossHitPointView;
    }
}