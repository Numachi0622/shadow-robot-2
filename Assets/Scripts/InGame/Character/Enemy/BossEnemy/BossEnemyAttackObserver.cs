using System;
using UniRx;
using UnityEngine;

namespace InGame.Character
{
    public class BossEnemyAttackObserver
    {
        private readonly Subject<AttackReadyParam> _onAttackStart = new();
        private readonly Transform _transform;
        private readonly float _attackInterval;
        private readonly float _readyTime;
        private float _currentTime = 0f;
        
        public IObservable<AttackReadyParam> OnAttackStart => _onAttackStart;
        

        public BossEnemyAttackObserver(Transform transform, float interval, float readyTime)
        {
            _transform = transform;
            _attackInterval = interval;
            _readyTime = readyTime;
        }
        
        public void Observe(Vector3 destination)
        {
            _currentTime += Time.deltaTime;
            if (_currentTime >= _attackInterval)
            {
                var param = new AttackReadyParam()
                {
                    Direction = (destination - _transform.position).normalized,
                    AttackReadyTime = _readyTime,
                };
                _onAttackStart.OnNext(param);
                _currentTime = 0f;
            }
        }
    }
}