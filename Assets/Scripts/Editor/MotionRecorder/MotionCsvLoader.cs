using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading;
using Windows.Kinect;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ShadowRobotEditor
{
    public class MotionCsvLoader : EditorWindow
    {
        private enum TargetPlayer
        {
            Player01 = 0,
            Player02 = 1,
        }
        
        private const string DataPath = "Assets/Data/MotionTestData/MotionCsvData.asset";

        private int _fps = 30;
        private TargetPlayer _targetPlayer = TargetPlayer.Player01;
        private MotionCsvData.CsvMotionType _motionType = MotionCsvData.CsvMotionType.Sample1;
        private MotionCsvData _motionCsvData;
        private CancellationTokenSource _cts;
        
        private int DeltaTime => 1 / _fps;
        
        [MenuItem("ShadowRobot2/MotionCSVLoader")]
        public static void ShowWindow()
        {
            var window = GetWindow<MotionCsvLoader>("Motion CSV Loader");
            window.titleContent = new GUIContent("Motion CSV Loader");
            window.Show();
        }

        private void OnEnable()
        {
            _motionCsvData = AssetDatabase.LoadAssetAtPath<MotionCsvData>(DataPath);
        }

        private void OnGUI()
        {
            GUILayout.Label("Motion CSV Loader", EditorStyles.boldLabel);
            
            _fps = EditorGUILayout.IntField("FPS", _fps);
            
            if(!Application.isPlaying) return;
            _targetPlayer = (TargetPlayer)EditorGUILayout.EnumPopup("Target Player", _targetPlayer);
            _motionType = (MotionCsvData.CsvMotionType)EditorGUILayout.EnumPopup("Motion Type", _motionType);

            if (GUILayout.Button("再生"))
            {
                PlayMotion();
            }
        }

        private async void PlayMotion()
        {
            if (_motionCsvData == null)
            {
                Debug.LogError("MotionCsvDataが見つかりません。");
                return;
            }
            
            if (!_motionCsvData.CsvData.TryGetValue(_motionType, out var csvData))
            {
                Debug.LogError($"{_motionType}のCSVデータが見つかりません。");
                return;
            }

            try
            {
                await PlayMotionAsync(csvData);
            }
            catch (Exception e)
            {
                return;
            }
        }

        private async UniTask PlayMotionAsync(TextAsset data)
        {
            _cts = new CancellationTokenSource();
            var isHeader = true;
            var deviceId = 0;
            var playerId = (int)_targetPlayer;

            using (var reader = new StringReader(data.text))
            {
                while (await reader.ReadLineAsync() is { } line)
                {
                    if (isHeader)
                    {
                        isHeader = false;
                        continue;
                    }
                    if (string.IsNullOrEmpty(line)) continue;
                    if (line[^1] == ',')
                    {
                        line = line.Remove(line.Length - 1);
                    }

                    var values = line.Split(",")
                        .Select(v => v.Trim())
                        .ToArray();

                    if (values.Length % 4 != 0)
                    {
                        Debug.LogError($"CSVデータのフォーマットが不正です。{values.Length}");
                        return;
                    }
                    
                    var quaternions = values
                        .Select(v => float.TryParse(v, out var result) ? result : 0f)
                        .Select((value, index) => new { value, index })
                        .GroupBy(x => x.index / 4)
                        .Select(g => g.Select(x => x.value).ToArray())
                        .Select(array => new Quaternion(array[0], array[1], array[2], array[3]))
                        .ToArray();
                    
                    // 送信処理
                    BodySourceManager.Instance?.MotionSender.SendFlag(
                        OscAddress.GetFlagAddress(deviceId, playerId), 1
                    );

                    var index = 0;
                    foreach (JointType joint in Enum.GetValues(typeof(JointType)))
                    {
                        BodySourceManager.Instance?.MotionSender.SendMotion(
                            OscAddress.GetRotationAddress(deviceId, playerId, joint),
                            quaternions[index++]
                        );
                    }

                    await UniTask.Delay(TimeSpan.FromSeconds(DeltaTime), cancellationToken:_cts.Token);
                }
                
                BodySourceManager.Instance?.MotionSender.SendFlag(
                    OscAddress.GetFlagAddress(deviceId, playerId), 0
                );
            }
        }
    }
}