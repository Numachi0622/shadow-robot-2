using InGame.System;
using UnityEngine;

namespace InGame.Character
{
    public struct EnemyAttackParam : IStateParameter
    {
        public Vector3 Direction;
        public float AttackReadyTime;
        public int AttackIndex;
    }
}