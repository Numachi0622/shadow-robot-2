using InGame.System;
using UniRx;
using UnityEngine;
using Utility;

namespace InGame.Character
{
    public class TestEnemyCore : EnemyCore
    {
        private StateMachine<TestEnemyCore> _stateMachine;
        private AttackObserver<Unit> _attackObserver;
        
        public IAttackable Attacker => _attacker;
        
        public override void Initialize()
        {
            base.Initialize();
            _stateMachine = new StateMachine<TestEnemyCore>(this);
            _attackObserver = new TestEnemyAttackObserver();
            _attacker = new TestEnemyAttacker(_params, _attackCollider); 
            Bind();
        }
        
        public override void OnUpdate()
        {
            _attackObserver.Observe();
        }

        private void Bind()
        {
            _attackObserver.OnAttackStart
                .Subscribe(OnAttackStart)
                .AddTo(this);
            
            _damageObserver.OnTakeDamage
                .Subscribe(OnTakeDamage)
                .AddTo(this);
        }
        
        #region State Event
        private void OnAttackStart(Unit unit)
        {
            _attacker.Attack(Vector3.zero);
        }
        private void OnTakeDamage(AttackParam param)
        {
            HitEffectManager.Instance.Play(param.AttackType, param.HitPosition);
        }
        #endregion
    }
}