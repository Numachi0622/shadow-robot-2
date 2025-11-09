using System;
using UnityEngine;

namespace InGame.Character
{
    public class PlayerAttacker : Attacker
    {
        public PlayerAttacker(CharacterParams characterParams, AttackCollider attackCollider) : base(characterParams, attackCollider)
        {
        }

        private void SetAttackParam(float velocity, AttackType attackType)
        {
            _attackParam.AttackVelocity = velocity;
            _attackParam.AttackType = attackType;
        }

        public override void Attack(Vector3 dir, float velocity = 0f)
        {
            SetAttackParam(velocity, AttackType.PlayerToEnemyNormal);
            
            _attackCollider.AttackImpact(_attackParam);
        }

        public override void AttackEnd()
        {
            _attackCollider.AttackImpactEnd();
        }
    }
}
