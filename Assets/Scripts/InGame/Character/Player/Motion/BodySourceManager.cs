using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using DEMAFilter;
using InGame.Character;
using SynMotion;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utility;
using ZLinq;
using JointType = Windows.Kinect.JointType;

public class BodySourceManager : Utility.Singleton<BodySourceManager>
{
    [SerializeField] private int _kinectId;
    [FormerlySerializedAs("_oscSettings")] [SerializeField] private DeviceSettings _deviceSettings;
    [SerializeField] private TrackingDebugView _debugView;
    [SerializeField] private ColorBodySourceView _bodySourceView;
    
    private KinectSensor _sensor;
    private BodyFrameReader _reader;
    private Body[] _data = null;
    private readonly ReactiveCollection<Body> _trackedData = new ReactiveCollection<Body>();
    private IMotionSender _motionSender;
    
    public KinectSensor Sensor => _sensor;
    public Body[] GetData() => _data;
    public IReadOnlyReactiveCollection<Body> TrackedData => _trackedData;
    public Windows.Kinect.Vector4 FloorClipPlane { get; private set; }
    
    public Quaternion Comp => Quaternion.FromToRotation(new Vector3(FloorClipPlane.X, FloorClipPlane.Y, FloorClipPlane.Z), Vector3.up);
    
    public IMotionSender MotionSender => _motionSender;

    private void Awake()
    {
        Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();
        _motionSender = new MotionSender(_deviceSettings.IpAddress, _deviceSettings.Port);
        _sensor = KinectSensor.GetDefault();
        _bodySourceView.Initialize();

        if (_sensor != null)
        {
            _reader = _sensor.BodyFrameSource.OpenReader();
            
            if (!_sensor.IsOpen)
            {
                _sensor.Open();
            }
        }
        
        // _trackedData.ObserveCountChanged()
        //     .Where(_ => _debugView != null)
        //     .Subscribe(_debugView.UpdateTrackedCountView)
        //     .AddTo(this);
    }

    private void Update()
    {
        if(_reader == null) return;

        var frame = _reader.AcquireLatestFrame();
        if (frame == null) return;
        
        if (_data == null)
        {
            _data = new Body[_sensor.BodyFrameSource.BodyCount];
        }
                
        frame.GetAndRefreshBodyData(_data);
        FloorClipPlane = frame.FloorClipPlane;
        ObserveBodies();
                
        frame.Dispose();
        frame = null;
        
        SendMotion();
    }

    private void ObserveBodies()
    {
        if (_data == null) return;
        
        for(var i = 0; i < _data.Length; i++)
        {
            var body = _data[i];
            if (body == null) continue;

            if (body.IsTracked)
            {
                if(_trackedData.Contains(body)) continue; 
                _trackedData.Add(body);
                continue;
            }
            
            if(!_trackedData.Contains(body)) continue;
            _trackedData.Remove(body);
        }

        // if (_trackedData.Count >= 2)
        // {
        //     var firstBody = _trackedData
        //         .AsValueEnumerable()
        //         .MinBy(body => body.Joints[JointType.SpineMid].Position.X);
        //     
        //     if (_trackedData[0] != firstBody)
        //     {
        //         var tmp = _trackedData[0];
        //         _trackedData[0] = firstBody;
        //         _trackedData[1] = tmp;
        //     }
        // }
        
        _debugView?.UpdateTrackingView(_data);
    }

    private void SendMotion()
    {
        for (var i = 0; i < GameConst.MAX_TRACKING_COUNT; i++)
        {
            _motionSender.SendFlag(
                OscAddress.GetFlagAddress(_kinectId, i),
                i == 0 ? (i < _trackedData.Count ? 1 : 0) : 1
            );
        }
        
        var comp = Quaternion.FromToRotation(new Vector3(FloorClipPlane.X, FloorClipPlane.Y, FloorClipPlane.Z), Vector3.up);
        
        for (var i = 0; i < _trackedData.Count; i++)
        {
            // 上半身
            _motionSender.SendMotion(
                OscAddress.GetRotationAddress(_kinectId, i, JointType.SpineMid),
                GetJointRotation(i, JointType.SpineMid, comp)
            );
            //Debug.Log($"{OscAddress.GetRotationAddress(_kinectId, i, JointType.SpineMid)}, {GetJointRotation(i, JointType.SpineMid, comp)}");
            
            _motionSender.SendMotion(
                OscAddress.GetRotationAddress(_kinectId, i, JointType.SpineShoulder),
                GetJointRotation(i, JointType.SpineShoulder, comp)
            );
            _motionSender.SendMotion(
                OscAddress.GetRotationAddress(_kinectId, i, JointType.ShoulderLeft),
                GetJointRotation(i, JointType.ShoulderLeft, comp)
            );
            _motionSender.SendMotion(
                OscAddress.GetRotationAddress(_kinectId, i, JointType.ElbowLeft),
                GetJointRotation(i, JointType.ElbowLeft, comp)
            );
            _motionSender.SendMotion(
                OscAddress.GetRotationAddress(_kinectId, i, JointType.WristLeft),
                GetJointRotation(i, JointType.WristLeft, comp)
            );
            _motionSender.SendMotion(
                OscAddress.GetRotationAddress(_kinectId, i, JointType.HandLeft),
                GetJointRotation(i, JointType.HandLeft, comp)
            );
            _motionSender.SendMotion(
                OscAddress.GetRotationAddress(_kinectId, i, JointType.ShoulderRight),
                GetJointRotation(i, JointType.ShoulderRight, comp)
            );
            _motionSender.SendMotion(
                OscAddress.GetRotationAddress(_kinectId, i, JointType.ElbowRight),
                GetJointRotation(i, JointType.ElbowRight, comp)
            );
            _motionSender.SendMotion(
                OscAddress.GetRotationAddress(_kinectId, i, JointType.WristRight),
                GetJointRotation(i, JointType.WristRight, comp)
            );
            _motionSender.SendMotion(
                OscAddress.GetRotationAddress(_kinectId, i, JointType.HandRight),
                GetJointRotation(i, JointType.HandRight, comp)
            ); 
            
            // 下半身
            _motionSender.SendMotion(
                OscAddress.GetRotationAddress(_kinectId, i, JointType.SpineBase),
                GetJointRotation(i, JointType.SpineBase, comp)
            );
            _motionSender.SendMotion(
                OscAddress.GetRotationAddress(_kinectId, i, JointType.KneeLeft),
                GetJointRotation(i, JointType.KneeLeft, comp)
            );
            _motionSender.SendMotion(
                OscAddress.GetRotationAddress(_kinectId, i, JointType.AnkleLeft),
                GetJointRotation(i, JointType.AnkleLeft, comp)
            );
            _motionSender.SendMotion(
                OscAddress.GetRotationAddress(_kinectId, i, JointType.KneeRight),
                GetJointRotation(i, JointType.KneeRight, comp)
            );
            _motionSender.SendMotion(
                OscAddress.GetRotationAddress(_kinectId, i, JointType.AnkleRight),
                GetJointRotation(i, JointType.AnkleRight, comp)
            );
            
            // 移動座標
            _motionSender.SendPosition(
                OscAddress.GetPositionAddress(_kinectId, i, JointType.SpineMid),
                GetJointPosition(i, JointType.SpineMid)
            );
        }
    }

    private Quaternion GetJointRotation(int index, JointType jointType, Quaternion comp)
    {
        return _trackedData[index].JointOrientations[jointType].Orientation
            .ToQuaternion(comp)
            .DEMAFilter(jointType);
    }

    private Vector3 GetJointPosition(int index, JointType jointType)
    {
        var pos = _trackedData[index].Joints[jointType].Position;
        var filteredPos = new Vector3(pos.X, pos.Y, pos.Z).DEMAFilter();
        return filteredPos;
    }
    
    void OnDestroy()
    {
        if (_reader != null)
        {
            _reader.Dispose();
            _reader = null;
        }
        
        if (_sensor != null)
        {
            if (_sensor.IsOpen)
            {
                _sensor.Close();
            }
            
            _sensor = null;
        }
    }
}

internal class BodyStatus
{
}
