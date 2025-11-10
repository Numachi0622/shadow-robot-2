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
        
        public override async UniTask AttackReady(Vector3 dir, float velocity = 0, float waitTime = 0)
        {
            
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