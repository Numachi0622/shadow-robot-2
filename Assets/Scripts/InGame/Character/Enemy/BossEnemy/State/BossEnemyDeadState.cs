using InGame.System;
using UnityEngine;
using Utility;

namespace InGame.Character
{
    public class BossEnemyDeadState : StateMachine<BossEnemyCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[BossEnemyDeadState] OnEnter");
            Owner.Animator.SetTrigger(AnimationUtility.DeathHash);
            
            //CharacterRegistry.Remove(Owner);
        }
    }
}
