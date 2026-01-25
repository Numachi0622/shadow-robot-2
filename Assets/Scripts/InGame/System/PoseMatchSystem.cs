using InGame.Character;
using UnityEngine;

namespace InGame.System
{
    public static class PoseMatchSystem
    {
        public const int MaxMatchValue = 7;
        
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
            float threshold = poseData.AllowedAngleDiff;

            // SpineMid
            if (IsJointMatching(poseData.SpineMidRotation, transforms.FirstSpine.localRotation, threshold))
                matchCount++;

            // Left Arm
            if (IsJointMatching(poseData.ElbowLeftRotation, transforms.LeftArm.localRotation, threshold))
                matchCount++;

            // Right Arm
            if (IsJointMatching(poseData.ElbowRightRotation, transforms.RightArm.localRotation, threshold))
                matchCount++;

            // Left Leg
            if (IsJointMatching(poseData.KneeLeftRotation, transforms.LeftUpLeg.localRotation, threshold))
                matchCount++;
            if (IsJointMatching(poseData.AnkleLeftRotation, transforms.LeftLeg.localRotation, threshold))
                matchCount++;

            // Right Leg
            if (IsJointMatching(poseData.KneeRightRotation, transforms.RightUpLeg.localRotation, threshold))
                matchCount++;
            if (IsJointMatching(poseData.AnkleRightRotation, transforms.RightLeg.localRotation, threshold))
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