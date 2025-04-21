using System;
using System.Linq;
using Kinect = Windows.Kinect;
using UnityEngine;
using Utility;

public class KinectMotionTest : MonoBehaviour
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

    [SerializeField] private BodySourceManager _bodySourceManager;
    [SerializeField] private DebugParamsPresenter _debugParamsPresenter;
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

    private void Start()
    {
        _debugParamsPresenter.Initialize();
    }
    private void Update()
    {
        if(_bodySourceManager == null) return;

        var data = _bodySourceManager.GetData();
        if(data == null) return;
        
        var trackedData = data.Where(b => b.IsTracked)
            .ToArray();
        if(data.Length == 0) return;
        
        _debugParamsPresenter.SetCount(data.Length);
        
        var leftBody = data[_debugParamsPresenter.Model.LeftTrackedId.Value];
        if (leftBody == null) return;
        var leftJoints = leftBody.JointOrientations;
        
        var rightBody = data[_debugParamsPresenter.Model.RightTrackedId.Value];
        if(rightBody == null) return;
        var rightJoints = rightBody.JointOrientations;
        
        var floorPlane = _bodySourceManager.FloorClipPlane;
        var comp = Quaternion.FromToRotation(new Vector3(floorPlane.X, floorPlane.Y, floorPlane.Z), Vector3.up);

        _spineBase = rightJoints[Kinect.JointType.SpineBase].Orientation.ToQuaternion(comp);
        _spineMid = rightJoints[Kinect.JointType.SpineMid].Orientation.ToQuaternion(comp);
        _spineShoulder = rightJoints[Kinect.JointType.SpineShoulder].Orientation.ToQuaternion(comp);
        _shoulderLeft = leftJoints[Kinect.JointType.ShoulderLeft].Orientation.ToQuaternion(comp);
        _shoulderRight = rightJoints[Kinect.JointType.ShoulderRight].Orientation.ToQuaternion(comp);
        _elbowLeft = leftJoints[Kinect.JointType.ElbowLeft].Orientation.ToQuaternion(comp);
        _wristLeft = leftJoints[Kinect.JointType.WristLeft].Orientation.ToQuaternion(comp);
        _handLeft = leftJoints[Kinect.JointType.HandLeft].Orientation.ToQuaternion(comp);
        _elbowRight = rightJoints[Kinect.JointType.ElbowRight].Orientation.ToQuaternion(comp);
        _wristRight = rightJoints[Kinect.JointType.WristRight].Orientation.ToQuaternion(comp);
        _handRight = rightJoints[Kinect.JointType.HandRight].Orientation.ToQuaternion(comp);
        _kneeLeft = leftJoints[Kinect.JointType.KneeLeft].Orientation.ToQuaternion(comp);
        _ankleLeft = leftJoints[Kinect.JointType.AnkleLeft].Orientation.ToQuaternion(comp);
        _kneeRight = rightJoints[Kinect.JointType.KneeRight].Orientation.ToQuaternion(comp);
        _ankleRight = rightJoints[Kinect.JointType.AnkleRight].Orientation.ToQuaternion(comp);

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
        var pos = leftBody.Joints[Kinect.JointType.SpineMid].Position;
        _ref.position = new Vector3(-pos.X, pos.Y, -pos.Z);

    }
}


