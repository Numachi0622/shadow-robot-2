using UnityEngine;

namespace InGame.Character
{
    public class FireBallAttacker : Attacker
    {
        public FireBallAttacker(AttackCollider attackCollider) : base(null, attackCollider)
        {
        }
        
        public void SetAttackParam(AttackParam attackParam)
        {
            _attackParam = attackParam;
        }
        
        public override void Attack(Vector3 dir, float velocity = 0, float waitTime = 0)
        {
            _attackCollider.AttackImpact(_attackParam);
        }
    }
}