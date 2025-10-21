using UnityEngine;

namespace InGame.Character
{
    public interface IAttackable
    {
        public void AttackReady();

        public void Attack(Vector3 dir, float velocity = 0f, float waitTime = 0f);

        public void AttackEnd();
    }
}