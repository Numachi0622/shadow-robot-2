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
            _playerCore.Initialize(0, _synMotion);
            _testEnemyCore.Initialize();
            _normalEnemyCore.Initialize();
            _buildingCore.Initialize();
            
            CharacterRegistry.Register(_playerCore);
            CharacterRegistry.Register(_testEnemyCore);
            CharacterRegistry.Register(_normalEnemyCore);
            CharacterRegistry.Register(_buildingCore);
        }
        
        private void Update()
        {
            _motionReceiver.UpdateMotion();
            _playerCore.OnUpdate();
            _testEnemyCore.OnUpdate();
            _normalEnemyCore.OnUpdate();
            _buildingCore.OnUpdate();
        }
    }
}