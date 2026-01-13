using InGame.Character;
using UnityEngine;

namespace InGame.System
{
    public static class PoseMatchSystem
    {
        public const int MaxMatchValue = 11;
        
        /// <summary>
        /// ポーズとの一致度を判定
        /// </summary>
        /// <param name="poseData">判定対象のポーズデータ</param>
        /// <param name="transforms">現在のモーショントランスフォーム</param>
        /// <returns>閾値以内の部位数</returns>
        public static int MatchPose(PoseData poseData, PlayerCore.MovementTransforms transforms)
        {
            if (poseData == null || transforms == null)
                return 0;

            int matchCount = 0;
            float threshold = poseData.allowedAngleDiff;

            // SpineMid
            if (IsJointMatching(poseData.spineMidRotation, transforms.FirstSpine.rotation, threshold))
                matchCount++;

            // Left Arm
            if (IsJointMatching(poseData.elbowLeftRotation, transforms.LeftArm.rotation, threshold))
                matchCount++;
            if (IsJointMatching(poseData.wristLeftRotation, transforms.LeftForeArm.rotation, threshold))
                matchCount++;
            if (IsJointMatching(poseData.handLeftRotation, transforms.LeftHand.rotation, threshold))
                matchCount++;

            // Right Arm
            if (IsJointMatching(poseData.elbowRightRotation, transforms.RightArm.rotation, threshold))
                matchCount++;
            if (IsJointMatching(poseData.wristRightRotation, transforms.RightForeArm.rotation, threshold))
                matchCount++;
            if (IsJointMatching(poseData.handRightRotation, transforms.RightHand.rotation, threshold))
                matchCount++;

            // Left Leg
            if (IsJointMatching(poseData.kneeLeftRotation, transforms.LeftUpLeg.rotation, threshold))
                matchCount++;
            if (IsJointMatching(poseData.ankleLeftRotation, transforms.LeftLeg.rotation, threshold))
                matchCount++;

            // Right Leg
            if (IsJointMatching(poseData.kneeRightRotation, transforms.RightUpLeg.rotation, threshold))
                matchCount++;
            if (IsJointMatching(poseData.ankleRightRotation, transforms.RightLeg.rotation, threshold))
                matchCount++;

            return matchCount;
        }

        /// <summary>
        /// 関節が閾値以内かどうかを判定
        /// </summary>
        private static bool IsJointMatching(Quaternion targetRotation, Quaternion currentRotation, float threshold)
        {
            float angleDiff = Quaternion.Angle(targetRotation, currentRotation);
            return angleDiff <= threshold;
        }
    }
}