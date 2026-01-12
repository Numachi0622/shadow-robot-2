using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.Character
{
    public abstract class Attacker : IAttackable
    {
        protected AttackCollider _attackCollider;
        protected AttackPoint _attackPoint;
        protected AttackParam _attackParam;
        
        public AttackParam AttackParam => _attackParam;
        
        public Attacker(CharacterParams characterParams, AttackCollider attackCollider)
        {
            _attackPoint = characterParams?.AttackPoint ?? default;
            _attackParam.AttackPoint = _attackPoint;
            _attackCollider = attackCollider;
        }

        public virtual async UniTask AttackReady(CancellationToken token, Vector3 dir, float velocity = 0, float waitTime = 0)
        {
        }

        public virtual void Attack(Vector3 dir, float velocity = 0, float waitTime = 0)
        {
        }

        public virtual void AttackEnd()
        {
        }
    }
}