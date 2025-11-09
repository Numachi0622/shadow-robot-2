using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace InGame.Character
{
    public class NormalEnemyMoveObserver
    {
        private readonly Subject<Unit> _onMoveStart = new();
        private readonly Subject<Unit> _onMoveEnd = new();
        private readonly Transform _transform;
        private readonly float _searchRange;
        private readonly float _attackRange;
        
        public IObservable<Unit> OnMoveStart => _onMoveStart;
        public IObservable<Unit> OnMoveEnd => _onMoveEnd;
        
        public NormalEnemyMoveObserver(Transform transform, float searchRange, float attackRange)
        {
            _transform = transform;
            _searchRange = searchRange;
            _attackRange = attackRange;
        }

        public void Observe(Vector3 destination)
        {
            var dist = Vector3.Distance(_transform.position, destination);
            if (dist < _searchRange && dist > _attackRange)
            {
                _onMoveStart.OnNext(Unit.Default);
            }
            else
            {
                _onMoveEnd.OnNext(Unit.Default);
            }
        }
    }
}