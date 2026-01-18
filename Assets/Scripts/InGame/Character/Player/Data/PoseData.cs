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
        public string PoseName;

        [Header("Pose Icon")]
        public Sprite PoseIcon;

        [Header("Upper Body Rotations")]
        public Quaternion SpineMidRotation;
        public Quaternion ElbowLeftRotation;
        public Quaternion WristLeftRotation;
        public Quaternion HandLeftRotation;
        public Quaternion ElbowRightRotation;
        public Quaternion WristRightRotation;
        public Quaternion HandRightRotation;

        [Header("Lower Body Rotations")]
        public Quaternion KneeLeftRotation;
        public Quaternion AnkleLeftRotation;
        public Quaternion KneeRightRotation;
        public Quaternion AnkleRightRotation;

        [Header("Recognition Parameters")]
        [Range(0f, 1f)]
        [Tooltip("ポーズ判定の一致しきい値（0-1）")]
        public float MatchThreshold = 0.8f;

        [Tooltip("各関節の許容角度差（度）")]
        public float AllowedAngleDiff = 30f;
    }
}
