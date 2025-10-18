using UnityEngine;

namespace SynMotion
{
    public struct MotionParam
    {
        public bool IsTracked;
        
        public Quaternion SpineMidRotation;
        public Quaternion SpineShoulderRotation;
    
        public Quaternion ShoulderLeftRotation;
        public Quaternion ElbowLeftRotation;
        public Quaternion WristLeftRotation;
        public Quaternion HandLeftRotation;

        public Quaternion ShoulderRightRotation;
        public Quaternion ElbowRightRotation;
        public Quaternion WristRightRotation;
        public Quaternion HandRightRotation;

        public Quaternion SpineBaseRotation;
        public Quaternion KneeLeftRotation;
        public Quaternion AnkleLeftRotation;
        public Quaternion KneeRightRotation;
        public Quaternion AnkleRightRotation;

        public Quaternion SpineMidPosition;
    }   
}