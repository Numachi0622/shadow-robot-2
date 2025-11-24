using System;
using Cysharp.Threading.Tasks;
using InGame.Character;
using UnityEngine;
using NaughtyAttributes;
using Utility;

namespace InGame.System
{
    public class DebugCommand : MonoBehaviour
    {
        [SerializeField] private CharacterCore _testEnemyCore;
        [SerializeField] private AttackCollider _attackCollider;
        [SerializeField] private AttackPoint _saikyoAttackParam;
        [SerializeField] private AttackPoint _saijakuAttackParam;

        [Button]
        public void SaikyoAttack()
        {
            var attackParam = new AttackParam()
            {
                AttackPoint = _saikyoAttackParam,
                AttackDirection = Vector3.forward,
                AttackVelocity = 0f,
                AttackType = AttackType.PlayerToEnemyNormal
            };
            _attackCollider.AttackImpactAsync(attackParam).Forget();
        }
        
        [Button]
        public void SaijakuAttack()
        {
            var attackParam = new AttackParam()
            {
                AttackPoint = _saijakuAttackParam,
                AttackDirection = Vector3.forward,
                AttackVelocity = 0f,
                AttackType = AttackType.PlayerToEnemyNormal
            };
            _attackCollider.AttackImpactAsync(attackParam).Forget();
        }
        
        [Button]
        public void TestEnemyAttack()
        {
            _testEnemyCore?.Attacker.Attack(Vector3.zero);
        }
    }
}