using InGame.System;
using UnityEngine;

namespace InGame.Character
{
    public class PlayerAttackState : StateMachine<PlayerCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("PlayerAttackState Enter");

            // if (parameter is HandAttackParam param)
            // {
            //     Owner.Attacker.Attack(param.Direction, param.Velocity);
            // }
        }

        public override void OnExit()
        {
            // Debug.Log("PlayerAttackState Exit");
            // Owner.Attacker.AttackEnd();
        }
    }
}