using InGame.System;
using UnityEngine;

namespace InGame.Character
{
    public class BossEnemyIdleState : StateMachine<BossEnemyCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[BossEnemyIdleState] OnEnter");
        }
    }
}