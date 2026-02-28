using System;
using InGame.System;
using UniRx;
using UnityEngine;

namespace InGame.Character
{
    public class EnemyCore : CharacterCore
    {
        [SerializeField] protected EnemyParams _params;
        [SerializeField] protected AttackCollider _attackCollider;
        [SerializeField] protected DamageCollider _damageCollider;
        [SerializeField] protected Transform _center;

        protected DamageObserver _damageObserver;
        
        public Transform Center => _center;

        public override void Initialize()
        {
            _attacker = new EnemyAttacker(_params, _attackCollider);
            _damageObserver = new DamageObserver();
            
            _damageCollider.Initialize(_damageObserver);
        }

        public override void OnUpdate()
        {
        }

        protected virtual void ForceStop()
        {
        }
    }
}