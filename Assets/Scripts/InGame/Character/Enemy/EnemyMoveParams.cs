using InGame.System;
using UnityEngine;

namespace InGame.Character
{
    public struct EnemyMoveParams : IStateParameter
    {
        public Vector3 Destination;
    }
}