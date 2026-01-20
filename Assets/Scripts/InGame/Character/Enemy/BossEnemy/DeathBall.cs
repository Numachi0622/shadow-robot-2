using UnityEngine;

namespace InGame.Character
{
    public class DeathBall : MonoBehaviour
    {
        [SerializeField] private AttackCollider _attackCollider;
        private LongRangeAttacker _attacker;
        private AttackParam _attackParam;
        private Transform _originTransform;
        private bool _isFired;
        private const int PlayerAttackLayer = 10;
        
        public void Fire(AttackParam attackParam, Transform origin, Transform playerTransform)
        {
            if (_isFired) return;

            _attacker = new LongRangeAttacker(_attackCollider);
            _originTransform = origin;
            _attackParam = attackParam;
            _attacker.SetAttackParam(_attackParam);
            _attacker.Attack(_attackParam.AttackDirection);
            _isFired = true;
        }

        private void Update()
        {
            if (!_isFired) return;

            var dir = _attackParam.AttackDirection;
            transform.position += dir * (_attackParam.AttackVelocity * Time.deltaTime);
        }

        private void OnCollisionEnter(Collision other)
        {
            // Damage処理はAttackerに任せる
            if (other.collider.GetComponent<DamageCollider>()) return;

            _attackParam.AttackType = AttackType.PlayerToEnemyNormal;
            _attackParam.AttackDirection = (_originTransform.position - transform.position).normalized;
            gameObject.layer = PlayerAttackLayer;
        }
    }
}