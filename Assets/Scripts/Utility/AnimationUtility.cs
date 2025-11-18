using UnityEngine;

namespace Utility
{
    public static class AnimationUtility
    {
        public static readonly int AttackReady01Hash = Animator.StringToHash("AttackReady01");
        public static readonly int AttackReady02Hash = Animator.StringToHash("AttackReady02");
        public static readonly int AttackReady03Hash = Animator.StringToHash("AttackReady03");
        public static readonly int AttackHash = Animator.StringToHash("Attack");
        public static readonly int DamageHash = Animator.StringToHash("Damage");
        public static readonly int DeathHash = Animator.StringToHash("Die");
        
        public static int AttackReadyHash(int index)
        {
            return index switch
            {
                0 => AttackReady01Hash,
                1 => AttackReady02Hash,
                2 => AttackReady03Hash,
                _ => AttackReady01Hash
            };
        }
    }
}