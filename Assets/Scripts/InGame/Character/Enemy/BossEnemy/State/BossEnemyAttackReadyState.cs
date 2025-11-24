using System;
using InGame.System;
using UnityEngine;
using Utility;

namespace InGame.Character
{
    public class BossEnemyAttackReadyState : StateMachine<BossEnemyCore>.State
    {
        private readonly int _shakeCount = 2;
        
        public override void OnEnter(IStateParameter parameter = null)
        {
            if (parameter is AttackReadyParam param)
            {
                Debug.Log("[BossEnemyAttackReady] OnEnter");

                var attackIndex = Owner.Params.SelectAttackPatternIndex();
                //var selectedPattern = Owner.Params.AttackPatternParams[attackIndex];

                var animationHash = AnimationUtility.AttackReadyHash(attackIndex);
                Owner.Animator.SetTrigger(animationHash);

                param.AttackIndex = attackIndex;
                //param.AttackImpactWaitTime = selectedPattern.AttackImpactWaitTime();

                AttackReady(param);
            }
        }
        
        private async void AttackReady(AttackReadyParam param)
        {
            var token = Owner.CancellationTokenSource.Token;
            Owner.EnemyEffect.ShakeBody(_shakeCount);
            try
            {
                await Owner.Attacker.AttackReady(token, param.Direction, 0f, param.AttackReadyTime);
            }
            catch (OperationCanceledException e)
            {
                return;
            }
            Owner.OnAttackStart(param);
        }
    }
}
