using System;
using System.IO;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Character;
using SynMotion;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Utility.Extensions;

namespace ShadowRobotDebug
{
    public class PoseRecorder
    {
        private const string FilePath = "Assets/Data/PoseData";
        private StreamWriter _writer;
        private CancellationTokenSource _cts;
        private readonly PlayerCore.MovementTransforms _transforms;

        public PoseRecorder(PlayerCore.MovementTransforms transforms)
        {
            _transforms = transforms;
        }
        
        public async UniTask<PoseData> StartRecording(string fileName, float recordingTime)
        {
            var csvFileName = string.IsNullOrEmpty(fileName) ? "pose" : fileName;
            var csvPath = Path.Combine(FilePath, csvFileName + ".csv");

            // ディレクトリが存在しない場合は作成
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

            var fileStream = new FileStream(
                csvPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.Read
            );
            _writer = new StreamWriter(fileStream, new UTF8Encoding(true));
            WriteHeader();

            Debug.Log("Recording Start");

            _cts = new CancellationTokenSource();
            MotionParam motionParam = default;
            PoseData poseData = null;

            try
            {
                motionParam = await RecordingLoop(recordingTime, _cts.Token);

                // 平均値をCSVに書き込み（デバッグ用）
                WriteMotionData(motionParam);

                // ScriptableObjectを作成
                poseData = CreatePoseDataAsset(csvFileName, motionParam);
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("Recording was cancelled");
            }
            finally
            {
                StopRecording();
            }

            return poseData;
        }
        
        private async UniTask<MotionParam> RecordingLoop(float recordingTime , CancellationToken token)
        {
            float currentTime = 0;
            int attemptCount = 0;
            var motionParam = new MotionParam();
            while (currentTime < recordingTime)
            {
                if (attemptCount == 0)
                {
                    // 最初のフレームは代入（初期値(0,0,0,0)を避ける）
                    motionParam.SpineMidRotation = _transforms.FirstSpine.localRotation;
                    motionParam.ElbowLeftRotation = _transforms.LeftArm.localRotation;
                    motionParam.WristLeftRotation = _transforms.LeftForeArm.localRotation;
                    motionParam.HandLeftRotation = _transforms.LeftHand.localRotation;
                    motionParam.ElbowRightRotation = _transforms.RightArm.localRotation;
                    motionParam.WristRightRotation = _transforms.RightForeArm.localRotation;
                    motionParam.HandRightRotation = _transforms.RightHand.localRotation;
                    motionParam.KneeLeftRotation = _transforms.LeftUpLeg.localRotation;
                    motionParam.AnkleLeftRotation = _transforms.LeftLeg.localRotation;
                    motionParam.KneeRightRotation = _transforms.RightUpLeg.localRotation;
                    motionParam.AnkleRightRotation = _transforms.RightLeg.localRotation;
                }
                else
                {
                    // 2フレーム目以降は加算
                    motionParam.SpineMidRotation = motionParam.SpineMidRotation.Add(_transforms.FirstSpine.localRotation);
                    motionParam.ElbowLeftRotation = motionParam.ElbowLeftRotation.Add(_transforms.LeftArm.localRotation);
                    motionParam.WristLeftRotation = motionParam.WristLeftRotation.Add(_transforms.LeftForeArm.localRotation);
                    motionParam.HandLeftRotation = motionParam.HandLeftRotation.Add(_transforms.LeftHand.localRotation);
                    motionParam.ElbowRightRotation = motionParam.ElbowRightRotation.Add(_transforms.RightArm.localRotation);
                    motionParam.WristRightRotation = motionParam.WristRightRotation.Add(_transforms.RightForeArm.localRotation);
                    motionParam.HandRightRotation = motionParam.HandRightRotation.Add(_transforms.RightHand.localRotation);
                    motionParam.KneeLeftRotation = motionParam.KneeLeftRotation.Add(_transforms.LeftUpLeg.localRotation);
                    motionParam.AnkleLeftRotation = motionParam.AnkleLeftRotation.Add(_transforms.LeftLeg.localRotation);
                    motionParam.KneeRightRotation = motionParam.KneeRightRotation.Add(_transforms.RightUpLeg.localRotation);
                    motionParam.AnkleRightRotation = motionParam.AnkleRightRotation.Add(_transforms.RightLeg.localRotation);
                }

                currentTime += Time.deltaTime;
                attemptCount++;
                await UniTask.Yield();
            }

            // 平均を計算して正規化
            motionParam.SpineMidRotation = motionParam.SpineMidRotation.Divide(attemptCount).normalized;
            motionParam.ElbowLeftRotation = motionParam.ElbowLeftRotation.Divide(attemptCount).normalized;
            motionParam.WristLeftRotation = motionParam.WristLeftRotation.Divide(attemptCount).normalized;
            motionParam.HandLeftRotation = motionParam.HandLeftRotation.Divide(attemptCount).normalized;
            motionParam.ElbowRightRotation = motionParam.ElbowRightRotation.Divide(attemptCount).normalized;
            motionParam.WristRightRotation = motionParam.WristRightRotation.Divide(attemptCount).normalized;
            motionParam.HandRightRotation = motionParam.HandRightRotation.Divide(attemptCount).normalized;
            motionParam.KneeLeftRotation = motionParam.KneeLeftRotation.Divide(attemptCount).normalized;
            motionParam.AnkleLeftRotation = motionParam.AnkleLeftRotation.Divide(attemptCount).normalized;
            motionParam.KneeRightRotation = motionParam.KneeRightRotation.Divide(attemptCount).normalized;
            motionParam.AnkleRightRotation = motionParam.AnkleRightRotation.Divide(attemptCount).normalized;

            return motionParam;
        }

        private void WriteHeader()
        {
            var header = new StringBuilder();
            header.Append("SpineMid_x,SpineMid_y,SpineMid_z,SpineMid_w,");
            header.Append("ElbowLeft_x,ElbowLeft_y,ElbowLeft_z,ElbowLeft_w,");
            header.Append("WristLeft_x,WristLeft_y,WristLeft_z,WristLeft_w,");
            header.Append("HandLeft_x,HandLeft_y,HandLeft_z,HandLeft_w,");
            header.Append("ElbowRight_x,ElbowRight_y,ElbowRight_z,ElbowRight_w,");
            header.Append("WristRight_x,WristRight_y,WristRight_z,WristRight_w,");
            header.Append("HandRight_x,HandRight_y,HandRight_z,HandRight_w,");
            header.Append("KneeLeft_x,KneeLeft_y,KneeLeft_z,KneeLeft_w,");
            header.Append("AnkleLeft_x,AnkleLeft_y,AnkleLeft_z,AnkleLeft_w,");
            header.Append("KneeRight_x,KneeRight_y,KneeRight_z,KneeRight_w,");
            header.Append("AnkleRight_x,AnkleRight_y,AnkleRight_z,AnkleRight_w");
            _writer.WriteLine(header.ToString());
        }

        private void WriteMotionData(MotionParam motionParam)
        {
            var line = new StringBuilder();
            line.Append($"{motionParam.SpineMidRotation.x},{motionParam.SpineMidRotation.y},{motionParam.SpineMidRotation.z},{motionParam.SpineMidRotation.w},");
            line.Append($"{motionParam.ElbowLeftRotation.x},{motionParam.ElbowLeftRotation.y},{motionParam.ElbowLeftRotation.z},{motionParam.ElbowLeftRotation.w},");
            line.Append($"{motionParam.WristLeftRotation.x},{motionParam.WristLeftRotation.y},{motionParam.WristLeftRotation.z},{motionParam.WristLeftRotation.w},");
            line.Append($"{motionParam.HandLeftRotation.x},{motionParam.HandLeftRotation.y},{motionParam.HandLeftRotation.z},{motionParam.HandLeftRotation.w},");
            line.Append($"{motionParam.ElbowRightRotation.x},{motionParam.ElbowRightRotation.y},{motionParam.ElbowRightRotation.z},{motionParam.ElbowRightRotation.w},");
            line.Append($"{motionParam.WristRightRotation.x},{motionParam.WristRightRotation.y},{motionParam.WristRightRotation.z},{motionParam.WristRightRotation.w},");
            line.Append($"{motionParam.HandRightRotation.x},{motionParam.HandRightRotation.y},{motionParam.HandRightRotation.z},{motionParam.HandRightRotation.w},");
            line.Append($"{motionParam.KneeLeftRotation.x},{motionParam.KneeLeftRotation.y},{motionParam.KneeLeftRotation.z},{motionParam.KneeLeftRotation.w},");
            line.Append($"{motionParam.AnkleLeftRotation.x},{motionParam.AnkleLeftRotation.y},{motionParam.AnkleLeftRotation.z},{motionParam.AnkleLeftRotation.w},");
            line.Append($"{motionParam.KneeRightRotation.x},{motionParam.KneeRightRotation.y},{motionParam.KneeRightRotation.z},{motionParam.KneeRightRotation.w},");
            line.Append($"{motionParam.AnkleRightRotation.x},{motionParam.AnkleRightRotation.y},{motionParam.AnkleRightRotation.z},{motionParam.AnkleRightRotation.w}");
            _writer.WriteLine(line.ToString());
        }

        private PoseData CreatePoseDataAsset(string fileName, MotionParam motionParam)
        {
            // ScriptableObjectのインスタンスを作成
            var poseData = ScriptableObject.CreateInstance<PoseData>();

            // データを設定
            poseData.PoseName = fileName;
            poseData.SpineMidRotation = motionParam.SpineMidRotation;
            poseData.ElbowLeftRotation = motionParam.ElbowLeftRotation;
            poseData.WristLeftRotation = motionParam.WristLeftRotation;
            poseData.HandLeftRotation = motionParam.HandLeftRotation;
            poseData.ElbowRightRotation = motionParam.ElbowRightRotation;
            poseData.WristRightRotation = motionParam.WristRightRotation;
            poseData.HandRightRotation = motionParam.HandRightRotation;
            poseData.KneeLeftRotation = motionParam.KneeLeftRotation;
            poseData.AnkleLeftRotation = motionParam.AnkleLeftRotation;
            poseData.KneeRightRotation = motionParam.KneeRightRotation;
            poseData.AnkleRightRotation = motionParam.AnkleRightRotation;

            // デフォルト値を設定
            poseData.MatchThreshold = 0.8f;
            poseData.AllowedAngleDiff = 60f;

            // アセットとして保存
            var assetPath = Path.Combine(FilePath, fileName + ".asset");
#if UNITY_EDITOR
            AssetDatabase.CreateAsset(poseData, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif

            Debug.Log($"PoseData ScriptableObject created: {assetPath}");

            return poseData;
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
    }
}