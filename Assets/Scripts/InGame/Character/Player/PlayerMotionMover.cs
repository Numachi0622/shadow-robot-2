using SynMotion;
using UnityEngine;

namespace InGame.Character
{
    public class PlayerMotionMover
    {
        private readonly PlayerCore.MovementTransforms _transforms;
        
        public PlayerMotionMover(PlayerCore.MovementTransforms movementTransforms)
        {
            _transforms = movementTransforms;
        }
        
        public void UpdateMotion(MotionParam param)
        {
            var motion = param;
                
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
            
            _transforms.Reference.rotation = Quaternion.identity;
        }
    }
}