using InGame.System;
using UnityEngine;

namespace InGame.Character
{
    public class PlayerDeadState : StateMachine<PlayerCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[PlayerDeadState] OnEnter");
        }
    }
}