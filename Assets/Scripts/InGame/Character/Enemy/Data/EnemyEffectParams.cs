using System;
using NaughtyAttributes;

namespace InGame.Character
{
    [Serializable]
    public class EnemyEffectParams
    {
        public float KnockBackDuration = 0.5f;
        public float KnockBackDistance = 5f;
        public float DeadKnockBackDistance = 8f;
    }
}