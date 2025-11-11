using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.Character
{
    public class TestEnemyAttacker : Attacker
    {
        public TestEnemyAttacker(CharacterParams characterParams, AttackCollider attackCollider) : base(characterParams, attackCollider)
        {
        }
        
        public override void Attack(Vector3 dir, float velocity = 0, float waitTime = 0)
        {
            _attackParam.AttackType = AttackType.EnemyToPlayerNormal;
            _attackCollider.AttackImpactAsync(_attackParam, waitTime).Forget();
        }
    }
}