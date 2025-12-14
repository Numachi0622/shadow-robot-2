using System;
using InGame.Character;
using SynMotion;
using UnityEngine;

namespace ShadowRobotDebug
{
    public class PoseRecordingEntryPoint : MonoBehaviour
    {
        [SerializeField] private PlayerCore _playerCore;
        [SerializeField] private DeviceSettings _deviceSettings;
        private MotionReceiver _receiver;
        private SynMotionSystem _synMotion;
        
        private void Start()
        {
            _synMotion = new SynMotionSystem();
            _receiver = new MotionReceiver(_deviceSettings, _synMotion, null, null, null);
            _receiver.Initialize();
            _playerCore.Initialize(new CharacterId(0), _synMotion);
        }

        private void Update()
        {
            _receiver.Tick();
            _playerCore.OnUpdate();
        }
    }
}