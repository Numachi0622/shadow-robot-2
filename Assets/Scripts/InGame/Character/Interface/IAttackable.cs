using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.Character
{
    public interface IAttackable
    {
        public UniTask AttackReady(Vector3 dir, float velocity = 0f, float waitTime = 0f);

        public void Attack(Vector3 dir, float velocity = 0f, float waitTime = 0f);

        public void AttackEnd();
    }
}