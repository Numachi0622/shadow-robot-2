using UnityEngine;

namespace InGame.Character
{
    [CreateAssetMenu(fileName = "EnemyEffectParams", menuName = "Character/Enemy/EnemyEffectParams")]
    public class EnemyEffectParams : ScriptableObject
    {
        public float KnockBackDuration = 0.5f;
        public float KnockBackDistance = 5f;
        public float DeadKnockBackDistance = 8f;
    }
}