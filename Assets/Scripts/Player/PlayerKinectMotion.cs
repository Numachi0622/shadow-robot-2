using System;
using System.Collections.Generic;
using System.Linq;
using DEMAFilter;
using UniRx;
using UniRx.Triggers;
using UnityEditor.Build.Reporting;
using Kinect = Windows.Kinect;
using UnityEngine;
using Utility;

public class PlayerKinectMotion : MonoBehaviour
{
    [SerializeField] private Transform _ref;
    [SerializeField] private Transform _hips;
    [SerializeField] private Transform _leftUpLeg;
    [SerializeField] private Transform _leftLeg;
    [SerializeField] private Transform _rightUpLeg;
    [SerializeField] private Transform _rightLeg;
    [SerializeField] private Transform _firstSpine;
    [SerializeField] private Transform _secondSpine;
    [SerializeField] private Transform _leftShoulder;
    [SerializeField] private Transform _leftArm;
    [SerializeField] private Transform _leftForeArm;
    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightShoulder;
    [SerializeField] private Transform _rightArm;
    [SerializeField] private Transform _rightForeArm;
    [SerializeField] private Transform _rightHand;
    [SerializeField] private Transform _neck;
    [SerializeField] private Transform _head;

    [SerializeField] private float _moveMagnification = 5f;
    [SerializeField] private float _jumpThreshold = 0.25f;
    [SerializeField] private float _jumpMagnification = 2f;
    
    private Quaternion _spineBase;
    private Quaternion _spineMid;
    private Quaternion _spineShoulder;
    private Quaternion _shoulderLeft;
    private Quaternion _shoulderRight;
    private Quaternion _elbowLeft;
    private Quaternion _wristLeft;
    private Quaternion _handLeft;
    private Quaternion _elbowRight;
    private Quaternion _wristRight;
    private Quaternion _handRight;
    private Quaternion _kneeLeft;
    private Quaternion _ankleLeft;
    private Quaternion _kneeRight;
    private Quaternion _ankleRight;

    private Kinect.Body _handData;
    private Kinect.Body _footData;

    private bool _isJumping = false;
    public bool IsJumping => _isJumping;
    
    public bool IsMovable => GameStatePresenter.Instance.CurrentGameState == GameState.InGame;
    
    public void Initialize()
    {
        this.UpdateAsObservable()
            .Subscribe(_ => UpdateMotion())
            .AddTo(this);

        BodySourceManager.Instance.TrackedData
            .ObserveAdd()
            .Subscribe(_ => SetTrackingData(BodySourceManager.Instance.TrackedData))
            .AddTo(this);

        BodySourceManager.Instance.TrackedData
            .ObserveRemove()
            .Subscribe(_ => SetTrackingData(BodySourceManager.Instance.TrackedData))
            .AddTo(this);

        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.C))
            .Subscribe(_ => ReverseTrackingData())
            .AddTo(this);
    }

    private void SetTrackingData(IReadOnlyReactiveCollection<Kinect.Body> bodies)
    {
        if(bodies == null) return;

        switch (bodies.Count)
        {
            case 0:
                _handData = null;
                _footData = null;
                break;
            case 1: 
                _handData = bodies[0];
                _footData = bodies[0];
                break;
            default:
                _handData = bodies[0];
                _footData = bodies[1];
                break;
        }
    }

    private void ReverseTrackingData()
    {
        var trackedData = BodySourceManager.Instance.TrackedData;
        if(trackedData == null) return;
        if(trackedData.Count < 2) return;

        (_handData, _footData) = (_footData, _handData);
    }
    
    private void UpdateMotion()
    {
        var trackedData = BodySourceManager.Instance.TrackedData;
        if(trackedData == null) return;
        if (trackedData.Count == 0) return;
        
        if(_handData == null || _footData == null) return;
        
        var handJoints = _handData.JointOrientations;
        var footJoints = _footData.JointOrientations;
        
        var floorPlane = BodySourceManager.Instance.FloorClipPlane;
        var comp = Quaternion.FromToRotation(new Vector3(floorPlane.X, floorPlane.Y, floorPlane.Z), Vector3.up);
        var comp2 = Quaternion.AngleAxis(90f, new Vector3(0, 1, 0)) * Quaternion.AngleAxis(-90f, new Vector3(0, 0, 1));

        // 上半身
        _spineMid = handJoints[Kinect.JointType.SpineMid].Orientation.ToQuaternion(comp).DEMAFilter(Kinect.JointType.SpineMid);
        _spineShoulder = handJoints[Kinect.JointType.SpineShoulder].Orientation.ToQuaternion(comp).DEMAFilter(Kinect.JointType.SpineShoulder);
        _shoulderLeft = handJoints[Kinect.JointType.ShoulderLeft].Orientation.ToQuaternion(comp).DEMAFilter(Kinect.JointType.ShoulderLeft);
        _shoulderRight = handJoints[Kinect.JointType.ShoulderRight].Orientation.ToQuaternion(comp).DEMAFilter(Kinect.JointType.ShoulderRight);
        _elbowLeft = handJoints[Kinect.JointType.ElbowLeft].Orientation.ToQuaternion(comp).DEMAFilter(Kinect.JointType.ElbowLeft);
        _wristLeft = handJoints[Kinect.JointType.WristLeft].Orientation.ToQuaternion(comp).DEMAFilter(Kinect.JointType.WristLeft);
        _handLeft = handJoints[Kinect.JointType.HandLeft].Orientation.ToQuaternion(comp).DEMAFilter(Kinect.JointType.HandLeft);
        _elbowRight = handJoints[Kinect.JointType.ElbowRight].Orientation.ToQuaternion(comp).DEMAFilter(Kinect.JointType.ElbowRight);
        _wristRight = handJoints[Kinect.JointType.WristRight].Orientation.ToQuaternion(comp).DEMAFilter(Kinect.JointType.WristRight);
        _handRight = handJoints[Kinect.JointType.HandRight].Orientation.ToQuaternion(comp).DEMAFilter(Kinect.JointType.HandRight);
        
        _firstSpine.rotation = _spineMid * comp2;
        
        _rightArm.rotation = _elbowRight * comp2;
        _rightForeArm.rotation = _wristRight * comp2;
        _rightHand.rotation = _handRight * comp2;
        
        _leftArm.rotation = _elbowLeft * comp2;
        _leftForeArm.rotation = _wristLeft * comp2;
        _leftHand.rotation = _handLeft * comp2;
        
        // 下半身
        _spineBase = footJoints[Kinect.JointType.SpineBase].Orientation.ToQuaternion(comp).DEMAFilter(Kinect.JointType.SpineBase);
        _kneeLeft = footJoints[Kinect.JointType.KneeLeft].Orientation.ToQuaternion(comp).DEMAFilter(Kinect.JointType.KneeLeft);
        _ankleLeft = footJoints[Kinect.JointType.AnkleLeft].Orientation.ToQuaternion(comp).DEMAFilter(Kinect.JointType.AnkleLeft);
        _kneeRight = footJoints[Kinect.JointType.KneeRight].Orientation.ToQuaternion(comp).DEMAFilter(Kinect.JointType.KneeRight);
        _ankleRight = footJoints[Kinect.JointType.AnkleRight].Orientation.ToQuaternion(comp).DEMAFilter(Kinect.JointType.AnkleRight);

        var q = _ref.rotation;
        _ref.rotation = Quaternion.identity;

        _rightUpLeg.rotation = _kneeRight * Quaternion.AngleAxis(-90f, new Vector3(0, 0, 1));
        _rightLeg.rotation = _ankleRight * Quaternion.AngleAxis(-90f, new Vector3(0, 0, 1));

        _leftUpLeg.rotation = _kneeLeft * Quaternion.AngleAxis(-90f, new Vector3(0, 0, 1));
        _leftLeg.rotation = _ankleLeft * Quaternion.AngleAxis(-90f, new Vector3(0, 0, 1));

        _ref.rotation = q;
        
        // 移動
        var kinectPos = _footData.Joints[Kinect.JointType.SpineMid].Position;
        var filteredPos = new Vector3(kinectPos.X, kinectPos.Y, -kinectPos.Z).DEMAFilter();
        
        _isJumping = filteredPos.y > _jumpThreshold;
        
        var x = !IsMovable ? 0f : filteredPos.x * _moveMagnification;
        var z = !IsMovable ? 0f : filteredPos.z * _moveMagnification;
        var y =  !_isJumping ? 0f : (filteredPos.y - _jumpThreshold) * _jumpMagnification;
        
        var movedPos = new Vector3(x, y, z);
        _ref.position = movedPos;
    }
}


