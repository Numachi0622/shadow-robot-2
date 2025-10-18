using Windows.Kinect;
using OscCore;
using SynMotion;
using Unity.VisualScripting;
using UnityEngine;

public class MotionReceiver
{
    private readonly OscServer _receiver;
    private MotionParam[] _motionParam;
    private SynMotionSystem _synMotion; 

    public MotionReceiver(int port, SynMotionSystem synMotion)
    {
        _receiver = new OscServer(port);
        _synMotion = synMotion;
        _motionParam = new MotionParam[]
        {
            new MotionParam() { IsTracked = false },
            new MotionParam() { IsTracked = false },
            new MotionParam() { IsTracked = false },
            new MotionParam() { IsTracked = false }
        };

        Bind();
    }

    private void Bind()
    {
        for (var deviceId = 0; deviceId < 2; deviceId++)
        for (var trackedId = 0; trackedId < 2; trackedId++)
        {
            var index = trackedId + (2 * deviceId);
            
            _receiver.TryAddMethod(
                OscAddress.GetFlagAddress(deviceId, trackedId),
                values => _motionParam[index].IsTracked = values.ReadIntElement(0) == 1
            );
            
            _receiver.TryAddMethod(
                OscAddress.GetRotationAddress(deviceId, trackedId, JointType.SpineMid),
                values =>
                {
                    ReadValue(values, out _motionParam[index].SpineMidRotation);
                    Log(index, JointType.SpineMid, _motionParam[index].SpineMidRotation);
                });
            
            _receiver.TryAddMethod(
                OscAddress.GetRotationAddress(deviceId, trackedId, JointType.SpineShoulder),
                values => ReadValue(values, out _motionParam[index].SpineShoulderRotation)
            );
            
            _receiver.TryAddMethod(
                OscAddress.GetRotationAddress(deviceId, trackedId, JointType.ShoulderLeft),
                values => ReadValue(values, out _motionParam[index].ShoulderLeftRotation)
            );
            
            _receiver.TryAddMethod(
                OscAddress.GetRotationAddress(deviceId, trackedId, JointType.ElbowLeft),
                values => ReadValue(values, out _motionParam[index].ElbowLeftRotation)
            );
            
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

    public void UpdateMotion()
    {
        _synMotion.SetMotionParam(_motionParam);
    }

    private void ReadValue(OscMessageValues values, out Quaternion rot)
    {
        var x = values.ReadFloatElement(0);
        var y = values.ReadFloatElement(1);
        var z = values.ReadFloatElement(2);
        var w = values.ReadFloatElement(3);
        rot = new Quaternion(x, y, z, w);
    }

    private void Log(int playerId, JointType jointType, Quaternion rot)
    {
        var col = string.Empty;
        switch (playerId)
        {
            case 0: col = "<color=red>"; break;
            case 1: col = "<color=blue>"; break;
            case 2: col = "<color=green>"; break;
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
            case 0: col = "<color=red>"; break;
            case 1: col = "<color=blue>"; break;
            case 2: col = "<color=green>"; break;
        }
        
        Debug.Log($"{col}" +
                  $"PlayerId: {playerId}, " +
                  $"tracked: {isTracked} </color>");
    }
}