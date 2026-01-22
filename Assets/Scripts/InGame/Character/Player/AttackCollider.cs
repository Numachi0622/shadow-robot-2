using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using Utility;

namespace InGame.Character
{
    [RequireComponent(typeof(Collider))]
    public class AttackCollider : MonoBehaviour
    {
        [SerializeField] private Collider _attackCollider;
        private AttackParam _attackParam;

        public Action<Vector3> OnDealDamageAction { get; set; }

        private void OnTriggerEnter(Collider targetCollider) => DealDamage(targetCollider);

        private void DealDamage(Collider targetCollider)
        {
            if(!targetCollider.TryGetComponent<DamageCollider>(out var damageCollider)) return;

            _attackParam.HitPosition = targetCollider.ClosestPoint(transform.position);
            OnDealDamageAction?.Invoke(_attackParam.HitPosition);
            damageCollider.TakeDamage(_attackParam);
        }

        public void AttackImpact(AttackParam attackParam)
        {
            _attackParam = attackParam;
            
            if(_attackCollider.enabled) return;
            _attackCollider.enabled = true;
        }
        
        public async UniTask AttackImpactAsync(AttackParam attackParam, float waitTime = 0f)
        {
            _attackParam = attackParam;

            await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
            
            _attackCollider.enabled = true;
            await UniTask.Delay(TimeSpan.FromSeconds(GameConst.COLLIDER_ACTIVE_TIME));
            _attackCollider.enabled = false;
        }

        public void AttackImpactEnd()
        {
            if(!_attackCollider.enabled) return;
            _attackCollider.enabled = false;
        }
    }
}