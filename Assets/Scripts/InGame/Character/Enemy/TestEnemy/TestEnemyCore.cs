using InGame.System;
using UniRx;
using UnityEngine;

namespace InGame.Character
{
    public class TestEnemyCore : EnemyCore
    {
        private StateMachine<TestEnemyCore> _stateMachine;
        private AttackObserver<Unit> _attackObserver;
        public override void Initialize()
        {
            _stateMachine = new StateMachine<TestEnemyCore>(this);
            _attackObserver = new TestEnemyAttackObserver();
            base.Initialize();
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
            Debug.Log($"[TestEnemyCore] OnAttackStart");
        }
        private void OnTakeDamage(AttackParam param)
        {
            Debug.Log($"[TestEnemyCore] OnTakeDamage : {param.AttackPoint}");
        }
        #endregion
    }
}