using System;
using UnityEngine;

namespace InGame.Character
{
    public class PlayerAttacker : Attacker
    {
        public PlayerAttacker(CharacterParams characterParams, Collider attackCollider) : base(characterParams, attackCollider)
        {
        }

        private void SetAttackParam(float velocity, AttackType attackType)
        {
            _attackParam.AttackVelocity = velocity;
            _attackParam.AttackType = attackType;
        }
        
        public override void AttackReady()
        {
        }

        public override void Attack(Vector3 dir, float velocity = 0f, float waitTime = 0f)
        {
            SetAttackParam(velocity, AttackType.PlayerToEnemyNormal);
            
            if (_attackCollider.enabled) return;
            _attackCollider.enabled = true;
        }

        public override void AttackEnd()
        {
            if (!_attackCollider.enabled) return;
            _attackCollider.enabled = false;
        }
    }
}
