using InGame.System;
using UnityEngine;

namespace InGame.Character
{
    public class PlayerIdleState : StateMachine<PlayerCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("PlayerIdleState Enter");
        }
        
        public override void OnExit()
        {
            Debug.Log("PlayerIdleState Exit");
        }
    }
}