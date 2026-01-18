using System;
using InGame.System.UI;
using UnityEngine;

namespace InGame.Character
{
    /// <summary>
    /// ポーズデータを格納するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "PoseData", menuName = "Character/PoseData", order = 0)]
    public class PoseData : ScriptableObject
    {
        [Header("Pose Information")]
        public string poseName;

        [Header("Pose Icon")] 
        public Sprite PoseIcon;

        [Header("Upper Body Rotations")]
        public Quaternion spineMidRotation;
        public Quaternion elbowLeftRotation;
        public Quaternion wristLeftRotation;
        public Quaternion handLeftRotation;
        public Quaternion elbowRightRotation;
        public Quaternion wristRightRotation;
        public Quaternion handRightRotation;

        [Header("Lower Body Rotations")]
        public Quaternion kneeLeftRotation;
        public Quaternion ankleLeftRotation;
        public Quaternion kneeRightRotation;
        public Quaternion ankleRightRotation;

        [Header("Recognition Parameters")]
        [Range(0f, 1f)]
        [Tooltip("ポーズ判定の一致しきい値（0-1）")]
        public float matchThreshold = 0.8f;

        [Tooltip("各関節の許容角度差（度）")]
        public float allowedAngleDiff = 30f;

        [Header("Joint Weights")]
        [Tooltip("各関節の重要度の重み")]
        public JointWeights weights = new JointWeights();
    }

    /// <summary>
    /// 各関節の重要度の重み
    /// </summary>
    [Serializable]
    public struct JointWeights
    {
        [Range(0f, 2f)] public float spineMid;
        [Range(0f, 2f)] public float elbowLeft;
        [Range(0f, 2f)] public float wristLeft;
        [Range(0f, 2f)] public float handLeft;
        [Range(0f, 2f)] public float elbowRight;
        [Range(0f, 2f)] public float wristRight;
        [Range(0f, 2f)] public float handRight;
        [Range(0f, 2f)] public float kneeLeft;
        [Range(0f, 2f)] public float ankleLeft;
        [Range(0f, 2f)] public float kneeRight;
        [Range(0f, 2f)] public float ankleRight;

        public JointWeights(float defaultWeight = 1f)
        {
            spineMid = defaultWeight;
            elbowLeft = defaultWeight;
            wristLeft = defaultWeight;
            handLeft = defaultWeight;
            elbowRight = defaultWeight;
            wristRight = defaultWeight;
            handRight = defaultWeight;
            kneeLeft = defaultWeight;
            ankleLeft = defaultWeight;
            kneeRight = defaultWeight;
            ankleRight = defaultWeight;
        }
    }
}
