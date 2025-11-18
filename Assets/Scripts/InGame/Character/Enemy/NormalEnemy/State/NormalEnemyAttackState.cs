using InGame.System;
using UnityEngine;
using Utility;

namespace InGame.Character
{
    public class NormalEnemyAttackState : StateMachine<NormalEnemyCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            if (parameter is EnemyAttackParam param)
            {
                Debug.Log("[NormalEnemyAttackState] OnEnter");
                Owner.Attacker.Attack(param.Direction);
                Owner.Animator.SetTrigger(AnimationUtility.AttackHash);
                if (param.AttackIndex == 3)
                {
                    Owner.Effect.AttackEffect.Play();
                }

                Owner.OnCoolTimeStart();
            }   
        }
    }
}