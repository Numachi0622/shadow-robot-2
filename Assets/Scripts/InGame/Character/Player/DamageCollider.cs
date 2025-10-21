using UnityEngine;

namespace InGame.Character
{
    public class DamageCollider : MonoBehaviour
    {
        private DamageObserver _damageObserver;
        
        public void Initialize(DamageObserver damageObserver)
        {
            _damageObserver = damageObserver;
        }
        
        public void TakeDamage(AttackParam attackParam)
        {
            _damageObserver?.Observe(attackParam);
        }
    }
}