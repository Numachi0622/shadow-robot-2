using System;
using System.Collections.Generic;
using InGame.Character;
using InGame.Message;
using MessagePipe;
using SynMotion;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace InGame.System
{
    public class InGameLifetimeScope : LifetimeScope
    {
        [Serializable]
        private class CharacterPrefabs
        {
            public PlayerCore Player;
            public NormalEnemyCore NormalEnemyCore;
            public BossEnemyCore BossEnemyCore;
        }
        
        [SerializeField] private CharacterPrefabs _characterPrefabs;
        [SerializeField] private PlayerSpawnSettings _playerSpawnSettings;
        [SerializeField] private DeviceSettings _deviceSettings;
        
        [SerializeField] private DebugCommand _debugCommand;
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<StateChangeMessage>(options);
            builder.RegisterMessageBroker<SpawnCharacterMessage>(options);

            // インゲーム基盤システム
            builder.RegisterEntryPoint<InGameCore>().AsSelf();
            
            // キャラクターのPrefabを登録
            builder.RegisterInstance(_characterPrefabs.Player);
            builder.RegisterInstance(_characterPrefabs.NormalEnemyCore);
            builder.RegisterInstance(_characterPrefabs.BossEnemyCore);
            
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

            builder.RegisterComponent(_debugCommand);
        }
    }
}