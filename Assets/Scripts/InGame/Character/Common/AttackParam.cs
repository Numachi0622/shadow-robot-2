using InGame.System;
using UnityEngine;

namespace InGame.Character
{
    public struct AttackParam : IStateParameter
    {
        public AttackPoint AttackPoint;
        public Vector3 AttackDirection;
        public Vector3 HitPosition;
        public float AttackVelocity;
        public AttackType AttackType;
    }
}