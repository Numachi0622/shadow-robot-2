using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Player
{
    public class HandAttackDetector
    {
        private Transform _handTransform;
        private float _velocityThreshold;
        private Vector3 _previousPosition;
        private bool _isAttacking = false;
        
        private Subject<Unit> _onAttack;
        public IObservable<Unit> OnAttack => _onAttack;
        
        public HandAttackDetector(Transform handTransform, float velocityThreshold)
        {
            _handTransform = handTransform;
            _velocityThreshold = velocityThreshold;
            _previousPosition = _handTransform.position;
            _onAttack = new Subject<Unit>();

            _handTransform.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    var currentPos = _handTransform.position;
                    var speed = (currentPos - _previousPosition).magnitude / Time.deltaTime;
                    _previousPosition = currentPos;
                    if(speed > _velocityThreshold)
                    {
                        _onAttack.OnNext(Unit.Default);
                        return;
                    }
                    
                    
                })
                .AddTo(_handTransform.gameObject);
        }
    }
}
