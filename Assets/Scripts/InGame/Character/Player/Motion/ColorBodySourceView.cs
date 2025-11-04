using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Serialization;
using Kinect = Windows.Kinect;
using UnityEngine.UI;

public class ColorBodySourceView : MonoBehaviour
{
    [SerializeField] private Material _boneMaterial;
    [SerializeField] private Material _meshMaterial;
    [SerializeField] private Camera _convertCamera;
    [SerializeField] private GameObject _idTextDebugCanvas;
    
    private Dictionary<Kinect.Body, GameObject> _bodies = new Dictionary<Kinect.Body, GameObject>();
    private Kinect.CoordinateMapper _coordinateMapper;
    private const int WIDTH = 1920;
    private const int HEIGHT = 1080;
    private uint _currentDisplayId = 0;

    
    private Dictionary<Kinect.JointType, Kinect.JointType> _boneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    public void Initialize()
    {
        BodySourceManager.Instance.TrackedData
            .ObserveAdd()
            .Where(body => !_bodies.ContainsKey(body.Value))
            .Subscribe(body =>
            {
                _currentDisplayId++;
                _bodies.Add(body.Value, CreateBodyObject(body.Value.TrackingId, _currentDisplayId));
            }) 
            .AddTo(this);

        BodySourceManager.Instance.TrackedData
            .ObserveRemove()
            .Where(body => _bodies.ContainsKey(body.Value))
            .Subscribe(body =>
            {
                _currentDisplayId--;
                Destroy(_bodies[body.Value]);
                _bodies.Remove(body.Value);
            })
            .AddTo(this);
        
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                _coordinateMapper ??= BodySourceManager.Instance.Sensor.CoordinateMapper;
                RefreshTrackedBodyObject();
            })
            .AddTo(this);
    }
    
    private GameObject CreateBodyObject(ulong id, uint displayId)
    {
        GameObject body = new GameObject("Body:" + id);
        body.layer = LayerMask.NameToLayer("Debug");
        
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            jointObj.GetComponent<MeshRenderer>().material = _meshMaterial;
            jointObj.layer = LayerMask.NameToLayer("Debug");
            
            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = _boneMaterial;
            lr.SetWidth(0.05f, 0.05f);
            
            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
        }
        
        // view id text
        var idText = Instantiate(_idTextDebugCanvas, body.transform.GetChild(3).transform);
        idText.transform.GetChild(0).GetComponent<Text>().text = $"id: {displayId}";
        
        return body;
    }

    private void RefreshTrackedBodyObject()
    {
        foreach (var body in _bodies)
        {
            RefreshBodyObject(body.Key, body.Value);
        }
    }
    
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;
            
            if(_boneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_boneMap[jt]];
            }
            
            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);
            
            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if(targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.SetColors(GetColorForState (sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            }
            else
            {
                lr.enabled = false;
            }
        }
    }
    
    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
        case Kinect.TrackingState.Tracked:
            return Color.green;

        case Kinect.TrackingState.Inferred:
            return Color.red;

        default:
            return Color.black;
        }
    }
    
    private Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        var valid = joint.TrackingState != Kinect.TrackingState.NotTracked;
        if (_convertCamera != null || valid ) {
            // KinectのCamera座標系(3次元)をColor座標系(2次元)に変換する
            var point =_coordinateMapper.MapCameraPointToColorSpace( joint.Position );
            var point2 = new Vector3( point.X, point.Y, 0 );
            if ( (0 <= point2.x) && (point2.x < WIDTH) && 
                 (0 <= point2.y) && (point2.y < HEIGHT) ) {

                // スクリーンサイズで調整(Kinect->Unity)
                point2.x = point2.x * Screen.width / WIDTH;
                point2.y = point2.y * Screen.height / HEIGHT;

                // Unityのワールド座標系(3次元)に変換
                var colorPoint3 = _convertCamera.ScreenToWorldPoint( point2 );

                // 座標の調整
                // Y座標は逆、Z座標は-1にする(Xもミラー状態によって逆にする必要あり)
                colorPoint3.y *= -1;
                colorPoint3.z = -1;

                return colorPoint3;
            }
        }

        return new Vector3( joint.Position.X * 10, 
            joint.Position.Y * 10, 
            -1 );
    }
}
