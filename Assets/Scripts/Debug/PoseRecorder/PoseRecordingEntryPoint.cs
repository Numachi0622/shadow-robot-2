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
        [SerializeField] private PoseRecordingPresenter _presenter;
        private MotionRegistry _motionRegistry;
        private MotionReceiver _receiver;
        private SynMotionSystem _synMotion;
        
        private void Start()
        {
            _synMotion = new SynMotionSystem();
            _motionRegistry = new MotionRegistry(null, null, null, null);
            _receiver = new MotionReceiver(_deviceSettings, _synMotion, _motionRegistry);
            _receiver.Initialize();
            _playerCore.Initialize(new CharacterId(0), _synMotion);
            _presenter.Initialize(_playerCore.Transforms);
        }

        private void Update()
        {
            _receiver.Tick();
            _playerCore.OnUpdate();
        }
    }
}