using System;
using System.IO;
using System.Text;
using System.Threading;
using Windows.Kinect;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Utility.Extensions;

namespace ShadowRobotEditor
{
    public class MotionRecorder : EditorWindow
    {
        private bool _isCountDownStart;
        private bool _isRecording = false;
        private int _fps = 30;
        private string _fileName = "motion";
        private const string FilePath = "Assets/Data/MotionTestData";

        private StreamWriter _writer;
        private readonly string[] _recordButtonText = { "録画開始", "録画中" };
        private Color _defaultColor;
        private CancellationTokenSource _cts;

        public float DeltaTime => 1f / _fps;
        
        [MenuItem("ShadowRobot2/MotionRecorder")]
        public static void ShowWindow()
        {
            var window = GetWindow<MotionRecorder>("Motion Recorder");
            window.titleContent = new GUIContent("Motion Recorder");
            window.Show();
        }

        private void OnEnable()
        {
            _defaultColor = GUI.backgroundColor;
        }

        private void OnGUI()
        {
            GUILayout.Label("Motion Recorder", EditorStyles.boldLabel);

            _fps = EditorGUILayout.IntField("FPS", _fps);
            _isCountDownStart = EditorGUILayout.ToggleLeft("カウントダウンの有無", _isCountDownStart);
            _fileName = EditorGUILayout.TextField("ファイル名", _fileName);

            if (Application.isPlaying)
            {
                ShowRecordButton();
            }
        }

        private void ShowRecordButton()
        {
            var buttonText = _isRecording ? _recordButtonText[1] : _recordButtonText[0];
            var color = _isRecording ? Color.red : _defaultColor;
            GUI.backgroundColor = color;
            if (GUILayout.Button(buttonText))
            {
                _isRecording = !_isRecording;
                
                if (_isRecording)
                {
                    StartRecording();
                }
                else
                {
                    StopRecording();
                }
            }
        }

        private async void StartRecording()
        {
            var fileName = _fileName + ".csv";
            var path = Path.Combine(FilePath, fileName);
            var fileStream = new FileStream(
                path,
                FileMode.Create,
                FileAccess.Write,
                FileShare.Read
            );
            _writer = new StreamWriter(fileStream, new UTF8Encoding(true));
            WriteHeader();
            
            Debug.Log("Recording Start");

            _cts = new CancellationTokenSource();
            try
            {
                await RecordingLoop(_cts.Token, 0);
            }
            catch (Exception e)
            {
                return;
            }
        }

        private async UniTask RecordingLoop(CancellationToken token, int index)
        {
            while (!token.IsCancellationRequested)
            {
                var body = BodySourceManager.Instance.TrackedData[index];
                if (body is not { IsTracked: true }) continue;
                WriteMotionData(body);

                await UniTask.Delay(TimeSpan.FromSeconds(DeltaTime), cancellationToken: token);
            }
        }

        private void WriteHeader()
        {
            var header = new StringBuilder();
            foreach (var joint in Enum.GetValues((typeof(JointType))))
            {
                header.Append($"{joint}_x, {joint}_y, {joint}_z, {joint}_w,");
            }
            _writer.WriteLine(header.ToString());
        }


        private void WriteMotionData(Body body)
        {
            var line = new StringBuilder();
            foreach (JointType joint in Enum.GetValues(typeof(JointType)))
            {
                if (!body.JointOrientations.ContainsKey(joint))
                {
                    line.Append("0,0,0,0,");
                    continue;
                }
                var rot = body.JointOrientations[joint].Orientation
                    .ToQuaternion(BodySourceManager.Instance.Comp);
                line.Append($"{rot.x},{rot.y},{rot.z},{rot.w},");
            }
            _writer.WriteLine(line.ToString());
        }

        private void StopRecording()
        {
            _cts?.Cancel();
            _cts?.Dispose();
                
            _writer?.Flush();
            _writer?.Close();
            _writer = null;

            Debug.Log("Recording Stopped");
        }

        private void OnDisable()
        {
            StopRecording();
        }

        private void OnDestroy()
        {
            StopRecording();
        }
    }
}
