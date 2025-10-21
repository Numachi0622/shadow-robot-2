using System;
using InGame.System;
using UniRx;
using UnityEngine;

namespace InGame.Character
{
    public class HandAttackObserver
    {
        private readonly Subject<HandAttackParam> _onAttackStart = new();
        private readonly Subject<Unit> _onAttackEnd = new();
        private readonly float _velocityThreshold;
        private readonly Transform _handTransform;
        private Vector3 _prevPos;
        private bool _isAttacking;

        public IObservable<HandAttackParam> OnAttackStart => _onAttackStart;
        public IObservable<Unit> OnAttackEnd => _onAttackEnd;

        public HandAttackObserver(Transform handTransform, float velocityThreshold)
        {
            _handTransform = handTransform;
            _velocityThreshold = velocityThreshold;
        }

        public void Observe()
        {
            var currentPos = _handTransform.position;
            var diff = currentPos - _prevPos;
            var direction = diff.normalized;
            var velocity = diff.magnitude / Time.deltaTime;
            _prevPos = currentPos;

            if (velocity > _velocityThreshold)
            {
                _onAttackStart.OnNext(new HandAttackParam()
                {
                    Velocity = velocity,
                    Direction = direction   
                });
                if (!_isAttacking) _isAttacking = true;
                return;
            }

            if (_isAttacking)
            {
                _isAttacking = false;
                _onAttackEnd.OnNext(Unit.Default);
            }
        }
    }

    public struct HandAttackParam : IStateParameter
    {
        public float Velocity;
        public Vector3 Direction;
    }
}
