using InGame.System;
using UnityEngine;
using Utility;

namespace InGame.Character
{
    public class NormalEnemyAttackState : StateMachine<NormalEnemyCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            if (parameter is AttackReadyParam param)
            {
                Debug.Log("[NormalEnemyAttackState] OnEnter");
                Owner.Animator.SetTrigger(AnimationUtility.AttackHash);

                var attackPatternParams = Owner.Params.AttackPatternParams[param.AttackIndex];
                if (attackPatternParams.Type == PatternType.Physical)
                {
                    Owner.Attacker.Attack(param.Direction, waitTime: param.AttackImpactWaitTime);
                }
                else if (attackPatternParams.Type == PatternType.LongRange)
                {
                    attackPatternParams.AttackPattern.Execute(Owner, param);
                }

                Owner.OnCoolTimeStart();
            }   
        }
    }
}