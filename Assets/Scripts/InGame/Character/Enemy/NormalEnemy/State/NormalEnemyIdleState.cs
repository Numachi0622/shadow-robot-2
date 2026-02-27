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

        public override void OnUpdate(IStateParameter parameter = null)
        {
            if (parameter is not EnemyMoveParams param) return;

            var dir = (param.Destination - Owner.transform.position).normalized;
            Owner.Mover.Rotate(dir);
        }
    }
}