using InGame.System;
using UnityEngine;

namespace InGame.Character
{
    public class NormalEnemyDeadState : StateMachine<NormalEnemyCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[NormalEnemyDeadState] OnEnter");
        }
    }
}