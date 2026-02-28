using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.Character
{
    public class RocketPunch : MonoBehaviour
    {
        [SerializeField] private AttackCollider _attackCollider;
        [SerializeField] private float _homingStartDelay = 0.5f;
        [SerializeField] private float _turnSpeed = 2f;
        [SerializeField] private float _speed = 5f;

        private CharacterCore _owner;
        private AttackParam _attackParam;
        private Transform _target;
        private float _elapsedTime;
        private bool _isHoming;
        private LongRangeAttacker _attacker;
        private bool _isFired;
        
        private void Update()
        {
            if (!_isFired) return;
            
            if (_target != null)
            {
                var targetDir = (_target.position - transform.position).normalized;
                var currentDir = Vector3.Slerp(_attackParam.AttackDirection, targetDir, _turnSpeed * Time.deltaTime);
                _attackParam.AttackDirection = currentDir.normalized;
            }
            
            transform.position += _attackParam.AttackDirection * (_attackParam.AttackVelocity * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(_attackParam.AttackDirection);
        }

        public void Fire(CharacterCore owner, AttackParam attackParam)
        {
            _owner = owner;
            _attackCollider.OnDealDamageAction = OnHit;
            _attackParam = attackParam;
            _attackParam.AttackVelocity = _speed;
            _attacker = new LongRangeAttacker(_attackCollider);
            _attacker.SetAttackParam(_attackParam);
            _attacker.Attack(_attackParam.AttackDirection);

            transform.rotation = Quaternion.LookRotation(_attackParam.AttackDirection);

            _isFired = true;
            
            SearchTargetAsync().Forget();
        }

        private async UniTask SearchTargetAsync()
        {
            if (_owner is not PlayerCore player) return;

            await UniTask.Delay(TimeSpan.FromSeconds(_homingStartDelay));
            _target = player.GetNearestEnemy(transform.position)?.transform;
        }

        private void OnHit(Vector3 pos)
        {
            Destroy(gameObject);
        }
    }
}