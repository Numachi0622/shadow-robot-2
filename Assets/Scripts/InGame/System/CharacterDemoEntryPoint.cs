using System;
using System.Collections.Generic;
using InGame.Character;
using SynMotion;
using TNRD;
using UnityEngine;
using Utility;

namespace InGame.System
{
    public class CharacterDemoEntryPoint : MonoBehaviour
    {
        [SerializeField] private DeviceSettings _deviceSettings;
        [SerializeField] private CharacterCore _playerCore;
        [SerializeField] private CharacterCore _testEnemyCore;
        [SerializeField] private CharacterCore _normalEnemyCore;
        [SerializeField] private CharacterCore _buildingCore;
        [SerializeField] private CharacterCore _bossEnemyCore;
        private MotionReceiver _motionReceiver;
        private SynMotionSystem _synMotion;
        
        [SerializeField] private List<SerializableInterface<ISingleton>> _singletons;

        private void Awake()
        {
            foreach (var singleton in _singletons)
            {
                singleton.Value.Initialize();
            }
        }

        private void Start()
        {
            _synMotion = new SynMotionSystem();
            _motionReceiver = new MotionReceiver(_deviceSettings, _synMotion);
            _playerCore?.Initialize(0, _synMotion);
            _testEnemyCore?.Initialize();
            _normalEnemyCore?.Initialize();
            _buildingCore?.Initialize();
            _bossEnemyCore?.Initialize();
            
            // CharacterRegistry.Register(_playerCore);
            // CharacterRegistry.Register(_testEnemyCore);
            // CharacterRegistry.Register(_normalEnemyCore);
            // CharacterRegistry.Register(_buildingCore);
            // CharacterRegistry.Register(_bossEnemyCore);
        }
        
        private void Update()
        {
            _motionReceiver?.UpdateMotion();

            if (_playerCore != null) _playerCore.OnUpdate();
            if (_testEnemyCore != null) _testEnemyCore.OnUpdate();
            if (_normalEnemyCore != null) _normalEnemyCore.OnUpdate();
            if (_buildingCore != null) _buildingCore.OnUpdate();
            if (_bossEnemyCore != null) _bossEnemyCore.OnUpdate();
        }
    }
}