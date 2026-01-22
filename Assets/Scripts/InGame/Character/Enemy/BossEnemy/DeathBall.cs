using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace InGame.Character
{
    public class DeathBall : MonoBehaviour
    {
        [SerializeField] private AttackCollider _attackCollider;
        [SerializeField] private ParticleSystem _ballParticle;
        [SerializeField] private ParticleSystem _explosionParticle;
        private LongRangeAttacker _attacker;
        private Material _ballMaterial;
        private AttackParam _attackParam;
        private Transform _originTransform;
        private bool _isFired;
        private const int PlayerAttackLayer = 10;
        private const int LifeDistance = 100;

        public async UniTask InitializeAsync()
        {
            _ballMaterial = _ballParticle.GetComponent<ParticleSystemRenderer>().material;
            var color = _ballMaterial.color;
            color.a = 0f;
            _ballMaterial.color = color;
            
            await _ballMaterial.DOFade(1f, 1f).ToUniTask();
        }

        public void Fire(AttackParam attackParam, Transform origin)
        {
            if (_isFired) return;

            _attackCollider.OnDealDamageAction = FireExplosion;
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

            if (Vector3.Distance(_originTransform.position, transform.position) >= LifeDistance)
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            // Damage処理はAttackerに任せる
            if (other.collider.GetComponent<DamageCollider>())
            {
                return;
            }

            _attackParam.AttackType = AttackType.PlayerToEnemyNormal;
            _attackParam.AttackDirection = (_originTransform.position - transform.position).normalized;
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