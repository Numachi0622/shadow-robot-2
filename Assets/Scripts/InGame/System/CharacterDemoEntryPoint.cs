using System;
using InGame.Character;
using SynMotion;
using UnityEngine;

namespace InGame.System
{
    public class CharacterDemoEntryPoint : MonoBehaviour
    {
        [SerializeField] private DeviceSettings _deviceSettings;
        [SerializeField] private CharacterCore _playerCore;
        [SerializeField] private CharacterCore _testEnemyCore;
        private MotionReceiver _motionReceiver;
        private SynMotionSystem _synMotion;

        private void Start()
        {
            _synMotion = new SynMotionSystem();
            _motionReceiver = new MotionReceiver(_deviceSettings, _synMotion);
            _playerCore.Initialize(0, _synMotion);
            _testEnemyCore.Initialize();
        }
        
        private void Update()
        {
            _motionReceiver.UpdateMotion();
            _playerCore.OnUpdate();
            _testEnemyCore.OnUpdate();
        }
    }
}