using UnityEngine;
using SynMotion;

namespace InGame.Character
{
    public class PlayerMovement
    {
        private MovementTransforms _transforms;
        private SynMotionSystem _synMotion;

        public PlayerMovement(MovementTransforms transforms, SynMotionSystem synMotion)
        {
            _transforms = transforms;
            _synMotion = synMotion;
        }

        public void Move()
        {
            
        }

        public void UpdateMotion()
        {
            var motion = _synMotion.GetMotionParam(0);
            
            _transforms.FirstSpine.rotation = motion.SpineMidRotation;

            _transforms.LeftArm.rotation = motion.ElbowLeftRotation;
            _transforms.LeftForeArm.rotation = motion.WristLeftRotation;
            _transforms.LeftHand.rotation = motion.HandLeftRotation;
            
            _transforms.RightArm.rotation = motion.ElbowRightRotation;
            _transforms.RightForeArm.rotation = motion.WristRightRotation;
            _transforms.RightHand.rotation = motion.HandRightRotation;

            _transforms.LeftUpLeg.rotation = motion.KneeLeftRotation;
            _transforms.LeftLeg.rotation = motion.AnkleLeftRotation;

            _transforms.RightUpLeg.rotation = motion.KneeRightRotation;
            _transforms.RightLeg.rotation = motion.AnkleRightRotation;

            //_transforms.Reference.rotation = Quaternion.identity;
        }
    }   
}

[System.Serializable]
public class MovementTransforms
{
    public Transform Reference;
    public Transform LeftUpLeg;
    public Transform LeftLeg;
    public Transform RightUpLeg;
    public Transform RightLeg;
    public Transform FirstSpine;
    public Transform LeftArm;
    public Transform LeftForeArm;
    public Transform LeftHand;
    public Transform RightArm;
    public Transform RightForeArm;
    public Transform RightHand;
}