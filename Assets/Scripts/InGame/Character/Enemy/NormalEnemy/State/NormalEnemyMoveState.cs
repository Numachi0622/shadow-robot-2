using InGame.System;
using UnityEngine;

namespace InGame.Character
{
    public class NormalEnemyMoveState : StateMachine<NormalEnemyCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[NormalEnemyMoveState] OnEnter");
        }
        public override void OnUpdate(IStateParameter parameter = null)
        {
            if (!(parameter is EnemyMoveParams param)) return;
            
            var dest = param.Destination;
            var delta = Owner.Params.MoveSpeed * Time.deltaTime;
            var movedPos = Vector3.MoveTowards(Owner.transform.position, dest, delta);
            Owner.Mover.Move(movedPos);
            
            var dir = (dest - Owner.transform.position).normalized;
            Owner.Mover.Rotate(dir);
        }
    }
}