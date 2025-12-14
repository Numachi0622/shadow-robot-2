using System;
using Windows.Kinect;
using InGame.Message;
using MessagePipe;
using OscCore;
using SynMotion;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace InGame.Character
{
    public class MotionReceiver : ITickable, IInitializable, IDisposable
    {
        private readonly OscServer _receiver;
        private readonly MotionParam[] _motionParam;
        private readonly SynMotionSystem _synMotion;
        private readonly PlayerSpawnSettings _playerSpawnSettings;
        private readonly bool[] _connectedFlags;
        private readonly int _maxDeviceCount;
        private readonly int _maxTrackingCount;
        private readonly IPublisher<SpawnCharacterMessage> _spawnPublisher;
        private readonly IPublisher<DespawnCharacterMessage> _despawnPublisher;

        [Inject]
        public MotionReceiver(
            DeviceSettings settings, 
            SynMotionSystem synMotion,
            PlayerSpawnSettings playerSpawnSettings,
            IPublisher<SpawnCharacterMessage> spawnPublisher,
            IPublisher<DespawnCharacterMessage> despawnPublisher)
        {
            _receiver = new OscServer(settings.Port);
            _synMotion = synMotion;
            _maxDeviceCount = settings.MaxDeviceCount;
            _maxTrackingCount = settings.MaxTrackingCountPerDevice;
            _playerSpawnSettings = playerSpawnSettings;
            
            _motionParam = new MotionParam[_maxDeviceCount * _maxTrackingCount];
            _connectedFlags = new bool[_maxDeviceCount * _maxTrackingCount];
            
            _spawnPublisher = spawnPublisher;
            _despawnPublisher = despawnPublisher;
        }
        
        public void Initialize()
        {
            Bind();
        }
        
        public void Dispose()
        {
            _receiver.Dispose();
        }

        private void Bind()
        {
            for (var deviceId = 0; deviceId < _maxDeviceCount; deviceId++)
            for (var trackedId = 0; trackedId < _maxTrackingCount; trackedId++)
            {
                var index = trackedId + (_maxDeviceCount * deviceId);

                _receiver.TryAddMethod(
                    OscAddress.GetFlagAddress(deviceId, trackedId),
                    values =>
                    {
                        _motionParam[index].IsTracked = values.ReadIntElement(0) == 1;
                        //Log(index, _motionParam[index].IsTracked);
                    });

                _receiver.TryAddMethod(
                    OscAddress.GetRotationAddress(deviceId, trackedId, JointType.SpineMid),
                    values =>
                    {
                        ReadValue(values, out _motionParam[index].SpineMidRotation);
                        //Log(index, JointType.SpineMid, _motionParam[index].SpineMidRotation);
                    });

                _receiver.TryAddMethod(
                    OscAddress.GetRotationAddress(deviceId, trackedId, JointType.SpineShoulder),
                    values => ReadValue(values, out _motionParam[index].SpineShoulderRotation)
                );

                _receiver.TryAddMethod(
                    OscAddress.GetRotationAddress(deviceId, trackedId, JointType.ShoulderLeft),
                    values =>
                    {
                        ReadValue(values, out _motionParam[index].ShoulderLeftRotation);
                        //Log(index, JointType.ShoulderLeft, _motionParam[index].ShoulderLeftRotation);
                    });

                _receiver.TryAddMethod(
                    OscAddress.GetRotationAddress(deviceId, trackedId, JointType.ElbowLeft),
                    values =>
                    {
                        ReadValue(values, out _motionParam[index].ElbowLeftRotation);
                        //Log(index, JointType.ElbowLeft, _motionParam[index].ElbowLeftRotation);
                    });

                _receiver.TryAddMethod(
                    OscAddress.GetRotationAddress(deviceId, trackedId, JointType.WristLeft),
                    values => ReadValue(values, out _motionParam[index].WristLeftRotation)
                );

                _receiver.TryAddMethod(
                    OscAddress.GetRotationAddress(deviceId, trackedId, JointType.HandLeft),
                    values => ReadValue(values, out _motionParam[index].HandLeftRotation)
                );

                _receiver.TryAddMethod(
                    OscAddress.GetRotationAddress(deviceId, trackedId, JointType.ShoulderRight),
                    values => ReadValue(values, out _motionParam[index].ShoulderRightRotation)
                );

                _receiver.TryAddMethod(
                    OscAddress.GetRotationAddress(deviceId, trackedId, JointType.ElbowRight),
                    values => ReadValue(values, out _motionParam[index].ElbowRightRotation)
                );

                _receiver.TryAddMethod(
                    OscAddress.GetRotationAddress(deviceId, trackedId, JointType.WristRight),
                    values => ReadValue(values, out _motionParam[index].WristRightRotation)
                );

                _receiver.TryAddMethod(
                    OscAddress.GetRotationAddress(deviceId, trackedId, JointType.HandRight),
                    values => ReadValue(values, out _motionParam[index].HandRightRotation)
                );

                _receiver.TryAddMethod(
                    OscAddress.GetRotationAddress(deviceId, trackedId, JointType.SpineBase),
                    values => ReadValue(values, out _motionParam[index].SpineBaseRotation)
                );

                _receiver.TryAddMethod(
                    OscAddress.GetRotationAddress(deviceId, trackedId, JointType.KneeLeft),
                    values => ReadValue(values, out _motionParam[index].KneeLeftRotation)
                );

                _receiver.TryAddMethod(
                    OscAddress.GetRotationAddress(deviceId, trackedId, JointType.AnkleLeft),
                    values => ReadValue(values, out _motionParam[index].AnkleLeftRotation)
                );

                _receiver.TryAddMethod(
                    OscAddress.GetRotationAddress(deviceId, trackedId, JointType.KneeRight),
                    values => ReadValue(values, out _motionParam[index].KneeRightRotation)
                );

                _receiver.TryAddMethod(
                    OscAddress.GetRotationAddress(deviceId, trackedId, JointType.AnkleRight),
                    values => ReadValue(values, out _motionParam[index].AnkleRightRotation)
                );

                _receiver.TryAddMethod(
                    OscAddress.GetPositionAddress(deviceId, trackedId, JointType.SpineMid),
                    values => ReadValue(values, out _motionParam[index].SpineMidPosition)
                );
            }
        }
        
        public void Tick()
        {
            _synMotion.SetMotionParam(_motionParam);

            for (var i = 0; i < _motionParam.Length; i++)
            {
                if (!_connectedFlags[i] && _motionParam[i].IsTracked)
                {
                    // 未接続 -> 接続
                    _connectedFlags[i] = _motionParam[i].IsTracked;

                    if (_playerSpawnSettings == null) continue;
                    var pos = _playerSpawnSettings.SpawnPositions[i];
                    _spawnPublisher?.Publish(new SpawnCharacterMessage(
                        new CharacterId(i),
                        CharacterType.Player,
                        pos,
                        Quaternion.identity
                    ));
                }
                else if (_connectedFlags[i] && !_motionParam[i].IsTracked)
                {
                    // 接続 -> 未接続
                    _connectedFlags[i] = _motionParam[i].IsTracked;
                    _despawnPublisher?.Publish(new DespawnCharacterMessage(new CharacterId(i)));
                }
            }
        }

        private void ReadValue(OscMessageValues values, out Quaternion rot)
        {
            var x = values.ReadFloatElement(0);
            var y = values.ReadFloatElement(1);
            var z = values.ReadFloatElement(2);
            var w = values.ReadFloatElement(3);
            rot = new Quaternion(x, y, z, w);
        }

        private void ReadValue(OscMessageValues values, out Vector3 pos)
        {
            var x = values.ReadFloatElement(0);
            var y = values.ReadFloatElement(1);
            var z = values.ReadFloatElement(2);
            pos = new Vector3(x, y, z);
        }

        private void Log(int playerId, JointType jointType, Quaternion rot)
        {
            var col = string.Empty;
            switch (playerId)
            {
                case 0:
                    col = "<color=red>";
                    break;
                case 1:
                    col = "<color=blue>";
                    break;
                case 2:
                    col = "<color=green>";
                    break;
            }

            Debug.Log($"{col}" +
                      $"PlayerId: {playerId}, " +
                      $"Joint: {jointType}, " +
                      $"Rotation: {rot} </color>");
        }

        private void Log(int playerId, bool isTracked)
        {
            var col = string.Empty;
            switch (playerId)
            {
                case 0:
                    col = "<color=red>";
                    break;
                case 1:
                    col = "<color=blue>";
                    break;
                case 2:
                    col = "<color=green>";
                    break;
            }

            Debug.Log($"{col}" +
                      $"PlayerId: {playerId}, " +
                      $"tracked: {isTracked} </color>");
        }
    }
}
