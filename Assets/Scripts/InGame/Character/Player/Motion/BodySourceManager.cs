using UnityEngine;
using Windows.Kinect;
using Utility.DEMAFilter;
using SynMotion;
using UniRx;
using Utility.Extensions;
using JointType = Windows.Kinect.JointType;
using Utility;
using Cysharp.Threading.Tasks;

namespace InGame.Character
{
    public class BodySourceManager : Singleton<BodySourceManager>
    {
        [SerializeField] private int _kinectId;
        [SerializeField] private string _ipAddress = "10.94.11.12";
        [SerializeField] private DeviceSettings _deviceSettings;
        [SerializeField] private TrackingDebugView _debugView;
        [SerializeField] private ColorBodySourceView _bodySourceView;
        [SerializeField] private bool _useInputView = true;

        private KinectSensor _sensor;
        private BodyFrameReader _reader;
        private Body[] _data = null;
        private readonly ReactiveCollection<Body> _trackedData = new ReactiveCollection<Body>();
        private IMotionSender _motionSender;
        private DEMAFilterManager _demaFilterManager;
        private bool _isInitialized = false;

        // GUI用の変数
        private string _kinectIdInput = "0";
        private string _statusMessage = "";

        public KinectSensor Sensor => _sensor;
        public Body[] GetData() => _data;
        public IReadOnlyReactiveCollection<Body> TrackedData => _trackedData;
        public Windows.Kinect.Vector4 FloorClipPlane { get; private set; }

        public Quaternion Comp =>
            Quaternion.FromToRotation(new Vector3(FloorClipPlane.X, FloorClipPlane.Y, FloorClipPlane.Z), Vector3.up);

        public IMotionSender MotionSender => _motionSender;
        public bool UseInputView => _useInputView;

        private async void Awake()
        {
            base.Initialize();
            await InitializeAsync();
        }

        private async UniTask InitializeAsync()
        {
            await UniTask.WaitUntil(() => _isInitialized || !_useInputView);

            _motionSender = new MotionSender(_ipAddress, _deviceSettings.Port);
            _sensor = KinectSensor.GetDefault();
            _bodySourceView.Initialize();
            _demaFilterManager = new DEMAFilterManager(GameConst.QUATERNION_DEMA_ALPHA, GameConst.VECTOR3_DEMA_ALPHA);

            if (_sensor != null)
            {
                _reader = _sensor.BodyFrameSource.OpenReader();

                if (!_sensor.IsOpen)
                {
                    _sensor.Open();
                }
            }
        }

        private void Update()
        {
            if (!_isInitialized && _useInputView) return;
            if (_reader == null) return;

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

            for (var i = 0; i < _data.Length; i++)
            {
                if (_trackedData.Count >= GameConst.MAX_TRACKING_COUNT) continue;
                var body = _data[i];
                if (body == null) continue;

                if (body.IsTracked)
                {
                    if (_trackedData.Contains(body)) continue;
                    _trackedData.Add(body);
                    continue;
                }

                if (!_trackedData.Contains(body)) continue;
                _trackedData.Remove(body);
            }

            // X座標で順序を維持（_trackedData[0]が左、_trackedData[1]が右）
            if (_trackedData.Count >= GameConst.MAX_TRACKING_COUNT)
            {
                var pos0X = _trackedData[0].Joints[JointType.SpineMid].Position.X;
                var pos1X = _trackedData[1].Joints[JointType.SpineMid].Position.X;

                if (pos0X > pos1X)
                {
                    (_trackedData[0], _trackedData[1]) = (_trackedData[1], _trackedData[0]);
                }
            }

            _debugView?.UpdateTrackingView(_data);
        }

        private void SendMotion()
        {
            for (var i = 0; i < GameConst.MAX_TRACKING_COUNT; i++)
            {
                _motionSender.SendFlag(
                    OscAddress.GetFlagAddress(_kinectId, i),
                    i < _trackedData.Count ? 1 : 0
                );
            }

            var comp = Quaternion.FromToRotation(new Vector3(FloorClipPlane.X, FloorClipPlane.Y, FloorClipPlane.Z),
                Vector3.up);

            for (var i = 0; i < _trackedData.Count; i++)
            {
                // 上半身
                _motionSender.SendMotion(
                    OscAddress.GetRotationAddress(_kinectId, i, JointType.SpineMid),
                    GetJointRotation(i, JointType.SpineMid, comp)
                );

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
            var rot = _trackedData[index].JointOrientations[jointType].Orientation
                .ToQuaternion(comp);
            return _demaFilterManager.FilterQuaternion(index, jointType, rot);
        }

        private Vector3 GetJointPosition(int index, JointType jointType)
        {
            var pos = _trackedData[index].Joints[jointType].Position.ToVector3();
            return _demaFilterManager.FilterVector3(index, pos);
        }

        private void OnGUI()
        {
            if (!_useInputView) return;
            
            // 初期化済みの場合は何も表示しない
            if (_isInitialized) return;

            // 画面中央にGUIを配置
            float windowWidth = 400f;
            float windowHeight = 300f;
            float windowX = (Screen.width - windowWidth) / 2f;
            float windowY = (Screen.height - windowHeight) / 2f;

            GUI.Box(new Rect(windowX, windowY, windowWidth, windowHeight), "Kinect初期化");

            GUILayout.BeginArea(new Rect(windowX + 20, windowY + 40, windowWidth - 40, windowHeight - 60));
            GUILayout.BeginVertical();

            GUILayout.Label("Kinect IDを入力してください:");
            GUILayout.Space(10);
            _kinectIdInput = GUILayout.TextField(_kinectIdInput, GUILayout.Height(30));
            GUILayout.Space(20);
            
            GUILayout.Label("IPアドレスを入力してください");
            GUILayout.Space(10);
            _ipAddress = GUILayout.TextField(_ipAddress, GUILayout.Height(30));
            GUILayout.Space(20);

            if (GUILayout.Button("Start", GUILayout.Height(40)))
            {
                _kinectId = int.Parse(_kinectIdInput);
                _isInitialized = true;
            }

            GUILayout.Space(10);

            if (!string.IsNullOrEmpty(_statusMessage))
            {
                GUILayout.Label(_statusMessage);
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
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
}
