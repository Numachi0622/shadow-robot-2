using System;
using UnityEngine;

namespace InGame.Character
{
    public class FireHoming : MonoBehaviour
    {
        [SerializeField] private AttackCollider _attackCollider;
        [SerializeField] private ParticleSystem _explosionParticle;

        private Transform _owner;
        private LongRangeAttacker _attacker;
        private AttackParam _attackParam;
        private const int PlayerAttackLayer = 10;

        private Transform _target;
        [SerializeField] private float _turnSpeed = 2f;

        public void Fire(AttackParam attackParam, Transform owner, Transform target)
        {
            _attackCollider.OnDealDamageAction = FireExplosion;
            _owner = owner;
            _attackParam = attackParam;
            _target = target;
            _attacker = new LongRangeAttacker(_attackCollider);
            _attacker.SetAttackParam(_attackParam);
            _attacker.Attack(_attackParam.AttackDirection);
        }

        private void Update()
        {
            // ターゲット方向へ徐々に旋回
            if (_target == null)
            {
                FireExplosion(transform.position);
                return;
            }
            
            var targetDirection = (_target.position - transform.position).normalized;
            var currentDir = Vector3.Slerp(_attackParam.AttackDirection, targetDirection, _turnSpeed * Time.deltaTime);
            _attackParam.AttackDirection = currentDir.normalized;
            transform.position += _attackParam.AttackDirection * (_attackParam.AttackVelocity * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(_attackParam.AttackDirection);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<AttackCollider>(out var hitAttackCollider)) return;

            _attackParam.AttackType = AttackType.PlayerToEnemyNormal;
            _attackParam.AttackDirection = hitAttackCollider.AttackParam.AttackDirection;
            _target = _owner;
            gameObject.layer = PlayerAttackLayer;
        }

        private void FireExplosion(Vector3 pos)
        {
            var explosion = Instantiate(_explosionParticle, pos, Quaternion.identity);
            explosion.Play();
            Destroy(gameObject);
        }
    }
}