using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.Character
{
    public class EnemyAttacker : Attacker
    {
        public EnemyAttacker(CharacterParams characterParams, AttackCollider attackCollider) : base(characterParams, attackCollider)
        {
        }

        private void SetAttackParam(AttackType attackType)
        {
            _attackParam.AttackType = attackType;
        }
        
        public override async UniTask AttackReady(CancellationToken token, Vector3 dir, float velocity = 0, float waitTime = 0)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: token);
        }
        
        public override void Attack(Vector3 dir, float velocity = 0, float waitTime = 0)
        {
            SetAttackParam(AttackType.EnemyToPlayerNormal);
            _attackCollider.AttackImpact(_attackParam);
        }
        
        public override void AttackEnd()
        {
            _attackCollider.AttackImpactEnd();
        }
    }
}