using System;
using InGame.System;
using UniRx;
using UnityEngine;

namespace InGame.Character
{
    public class NormalEnemyAttackObserver
    {
        private readonly Subject<AttackReadyParam> _onAttackStart = new();
        private readonly Transform _transform;
        private float _attackRange;
        private float _readyTime;
        
        public IObservable<AttackReadyParam> OnAttackStart => _onAttackStart;
        
        public NormalEnemyAttackObserver(Transform transform, float attackRange, float readyTime)
        {
            _transform = transform;
            SetParam(attackRange, readyTime);
        }
        
        public void SetParam(float attackRange, float readyTime)
        {
            _attackRange = attackRange;
            _readyTime = readyTime;
        }
        
        public void Observe(Vector3 destination)
        {
            var dist = Vector3.Distance(_transform.position, destination);
            if (dist < _attackRange)
            {
                var dir = (destination - _transform.position).normalized;
                var param = new AttackReadyParam()
                {
                    Direction = dir,
                    AttackReadyTime = _readyTime,
                };
                _onAttackStart.OnNext(param);   
            }
        }
    }
}