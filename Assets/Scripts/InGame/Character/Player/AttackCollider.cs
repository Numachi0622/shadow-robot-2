using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;

namespace InGame.Character
{
    [RequireComponent(typeof(Collider))]
    public class AttackCollider : MonoBehaviour
    {
        [SerializeField] private Collider _attackCollider;
        private AttackParam _attackParam;

        public void Initialize()
        {
            _attackCollider.OnTriggerEnterAsObservable()
                .Subscribe(DealDamage)
                .AddTo(this);
        }

        private void DealDamage(Collider targetCollider)
        {
            if(!targetCollider.TryGetComponent<DamageCollider>(out var damageCollider)) return;
            
            damageCollider.TakeDamage(_attackParam);
        }

        public void AttackImpact(AttackParam attackParam)
        {
            _attackParam = attackParam;
            
            if(_attackCollider.enabled) return;
            _attackCollider.enabled = true;
        }
        
        public UniTask AttackImpactAsync(float waitTime)
        {
            _attackCollider.enabled = true;
            return UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        }

        public void AttackImpactEnd()
        {
            if(!_attackCollider.enabled) return;
            _attackCollider.enabled = false;
        }
    }
}