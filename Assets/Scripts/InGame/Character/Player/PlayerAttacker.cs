using System;
using UnityEngine;

namespace InGame.Character
{
    public class PlayerAttacker : Attacker
    {
        public PlayerAttacker(CharacterParams characterParams, AttackCollider attackCollider) : base(characterParams, attackCollider)
        {
        }

        private void SetAttackParam(Vector3 dir, float velocity, AttackType attackType)
        {
            _attackParam = new AttackParam()
            {
                AttackPoint = _attackPoint,
                AttackDirection = dir,
                AttackVelocity = velocity,
                AttackType = attackType
            };
        }

        public override void Attack(Vector3 dir, float velocity = 0f, float waitTime = 0f)
        {
            SetAttackParam(dir, velocity, AttackType.PlayerToEnemyNormal);
            
            _attackCollider.AttackImpact(_attackParam);
        }

        public override void AttackEnd()
        {
            _attackCollider.AttackImpactEnd();
        }
    }
}
