using System;
using System.Linq;
using DEMAFilter;
using UniRx;
using UniRx.Triggers;
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

    [SerializeField] private DebugParamsPresenter _debugParamsPresenter;
    [SerializeField] private float _moveMagnification = 5f;
    [SerializeField] private bool _isMovable = false;
    
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

    public void Initialize()
    {
        this.UpdateAsObservable()
            .Subscribe(_ => UpdateMotion())
            .AddTo(this);
    }
    
    private void UpdateMotion()
    {
        var data = BodySourceManager.Instance.GetData();
        if(data == null) return;
        
        var trackedData = data.Where(b => b.IsTracked)
            .ToArray();
        if(data.Length == 0) return;
        
        _debugParamsPresenter.SetCount(data.Length);
        
        var handData = data[_debugParamsPresenter.Model.LeftTrackedId.Value];
        if (handData == null) return;
        var handJoints = handData.JointOrientations;
        
        var footData = data[_debugParamsPresenter.Model.RightTrackedId.Value];
        if(footData == null) return;
        var footJoints = footData.JointOrientations;
        
        var floorPlane = BodySourceManager.Instance.FloorClipPlane;
        var comp = Quaternion.FromToRotation(new Vector3(floorPlane.X, floorPlane.Y, floorPlane.Z), Vector3.up);

        _spineBase = footJoints[Kinect.JointType.SpineBase].Orientation.ToQuaternion(comp).DEMAFilter(Kinect.JointType.SpineBase);
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
        _kneeLeft = footJoints[Kinect.JointType.KneeLeft].Orientation.ToQuaternion(comp).DEMAFilter(Kinect.JointType.KneeLeft);
        _ankleLeft = footJoints[Kinect.JointType.AnkleLeft].Orientation.ToQuaternion(comp).DEMAFilter(Kinect.JointType.AnkleLeft);
        _kneeRight = footJoints[Kinect.JointType.KneeRight].Orientation.ToQuaternion(comp).DEMAFilter(Kinect.JointType.KneeRight);
        _ankleRight = footJoints[Kinect.JointType.AnkleRight].Orientation.ToQuaternion(comp).DEMAFilter(Kinect.JointType.AnkleRight);

        var q = _ref.rotation;
        _ref.rotation = Quaternion.identity;

        var comp2 = Quaternion.AngleAxis(90f, new Vector3(0, 1, 0)) * Quaternion.AngleAxis(-90f, new Vector3(0, 0, 1));

        _firstSpine.rotation = _spineMid * comp2;
        
        _rightArm.rotation = _elbowRight * comp2;
        _rightForeArm.rotation = _wristRight * comp2;
        _rightHand.rotation = _handRight * comp2;
        
        _leftArm.rotation = _elbowLeft * comp2;
        _leftForeArm.rotation = _wristLeft * comp2;
        _leftHand.rotation = _handLeft * comp2;

        _rightUpLeg.rotation = _kneeRight * Quaternion.AngleAxis(-90f, new Vector3(0, 0, 1));
        _rightLeg.rotation = _ankleRight * Quaternion.AngleAxis(-90f, new Vector3(0, 0, 1));

        _leftUpLeg.rotation = _kneeLeft * Quaternion.AngleAxis(-90f, new Vector3(0, 0, 1));
        _leftLeg.rotation = _ankleLeft * Quaternion.AngleAxis(-90f, new Vector3(0, 0, 1));

        _ref.rotation = q;

        if(!_isMovable) return;
        var kinectPos = handData.Joints[Kinect.JointType.SpineMid].Position;
        var movedPos = new Vector3(kinectPos.X, 0f, -kinectPos.Z) * _moveMagnification;

        _ref.position = movedPos;
    }
}


