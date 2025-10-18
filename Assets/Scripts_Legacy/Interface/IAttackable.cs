using UnityEngine;

namespace Interface
{
    public interface IAttackable
    {
        public AttackInfo AttackInfo { get; }
        public void Attack(float waitTime = 0);

        public void Attack(Vector3 dir, float velocity);

        public void AttackEnd();
    }
}