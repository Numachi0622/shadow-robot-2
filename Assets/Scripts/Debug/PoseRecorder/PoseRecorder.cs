using System;
using System.IO;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Character;
using SynMotion;
using UnityEngine;
using Utility.Extensions;

namespace ShadowRobotDebug
{
    public class PoseRecorder
    {
        private string _fileName = "pose";
        private const string FilePath = "Assets/Data/PoseData";
        private StreamWriter _writer;
        private CancellationTokenSource _cts;
        private float _recordingTime;
        
        public async UniTask<MotionParam> StartRecording(PlayerCore.MovementTransforms transforms)
        {
            var fileName = _fileName + ".csv";
            var path = Path.Combine(FilePath, fileName);

            // ディレクトリが存在しない場合は作成
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

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
            MotionParam result = default;
            try
            {
                result = await RecordingLoop(transforms, _cts.Token);

                // 平均値をCSVに書き込み
                WriteMotionData(result);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                StopRecording();
            }

            return result;
        }
        
        private async UniTask<MotionParam> RecordingLoop(PlayerCore.MovementTransforms transforms, CancellationToken token)
        {
            float currentTime = 0;
            int attemptCount = 0;
            var motionParam = new MotionParam();
            while (currentTime < _recordingTime)
            {
                motionParam.SpineMidRotation.Add(transforms.FirstSpine.rotation);
                motionParam.ElbowLeftRotation.Add(transforms.LeftArm.rotation);
                motionParam.WristLeftRotation.Add(transforms.LeftForeArm.rotation);
                motionParam.HandLeftRotation.Add(transforms.LeftHand.rotation);
                motionParam.ElbowRightRotation.Add(transforms.RightArm.rotation);
                motionParam.WristRightRotation.Add(transforms.RightForeArm.rotation);
                motionParam.HandRightRotation.Add(transforms.RightHand.rotation);
                motionParam.KneeLeftRotation.Add(transforms.LeftUpLeg.rotation);
                motionParam.AnkleLeftRotation.Add(transforms.LeftLeg.rotation);
                motionParam.KneeRightRotation.Add(transforms.LeftUpLeg.rotation);
                motionParam.AnkleRightRotation.Add(transforms.RightLeg.rotation);

                currentTime += Time.deltaTime;
                attemptCount++;
                await UniTask.Yield();
            }

            motionParam.SpineMidRotation.Divide(attemptCount);
            motionParam.ElbowLeftRotation.Divide(attemptCount);
            motionParam.WristLeftRotation.Divide(attemptCount);
            motionParam.HandLeftRotation.Divide(attemptCount);
            motionParam.ElbowRightRotation.Divide(attemptCount);
            motionParam.WristRightRotation.Divide(attemptCount);
            motionParam.HandRightRotation.Divide(attemptCount);
            motionParam.KneeLeftRotation.Divide(attemptCount);
            motionParam.AnkleLeftRotation.Divide(attemptCount);
            motionParam.KneeRightRotation.Divide(attemptCount);
            motionParam.AnkleRightRotation.Divide(attemptCount);

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