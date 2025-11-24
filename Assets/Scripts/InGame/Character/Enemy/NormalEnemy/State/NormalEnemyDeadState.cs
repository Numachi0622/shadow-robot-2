using InGame.System;
using UnityEngine;
using Utility;
using Utility.Extensions;

namespace InGame.Character
{
    public class NormalEnemyDeadState : StateMachine<NormalEnemyCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[NormalEnemyDeadState] OnEnter");
            Owner.Animator.SetTrigger(AnimationUtility.DeathHash);
            
            CharacterRegistry.Remove(Owner);
        }
    }
}