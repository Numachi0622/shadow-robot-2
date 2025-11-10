using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.Character
{
    public class NormalEnemyAttacker : Attacker
    {
        public NormalEnemyAttacker(CharacterParams characterParams, AttackCollider attackCollider) : base(characterParams, attackCollider)
        {
        }
        
        private void SetAttackParam(AttackType attackType)
        {
            _attackParam.AttackType = attackType;
        }
        
        public override async UniTask AttackReady(Vector3 dir, float velocity = 0, float waitTime = 0)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        }
        
        public override void Attack(Vector3 dir, float velocity = 0, float waitTime = 0)
        {
            SetAttackParam(AttackType.EnemyToPlayerNormal);
            _attackCollider.AttackImpactAsync(_attackParam, waitTime).Forget();
        }
    }
}