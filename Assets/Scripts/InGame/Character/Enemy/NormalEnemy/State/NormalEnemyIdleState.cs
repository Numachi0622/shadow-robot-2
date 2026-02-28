using InGame.System;
using UnityEngine;

namespace InGame.Character
{
    public class NormalEnemyIdleState : StateMachine<NormalEnemyCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[NormalEnemyIdleState] OnEnter");
        }
    }
}