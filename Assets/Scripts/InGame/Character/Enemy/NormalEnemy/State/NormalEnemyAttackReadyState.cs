using System;
using InGame.System;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace InGame.Character
{
    public class NormalEnemyAttackReadyState : StateMachine<NormalEnemyCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            if (parameter is AttackReadyParam param)
            {
                Debug.Log("[NormalEnemyAttackReadyState] OnEnter");

                // OccurrenceRateを使った重み付けランダム選択
                var attackIndex = Owner.Params.SelectAttackPatternIndex();
                var selectedPattern = Owner.Params.AttackPatternParams[attackIndex];

                // アニメーション再生
                var animationHash = AnimationUtility.AttackReadyHash(attackIndex);
                Owner.Animator.SetTrigger(animationHash);

                param.AttackIndex = attackIndex;
                param.AttackImpactWaitTime = selectedPattern.AttackImpactWaitTime();

                AttackReady(param);
            }
        }

        private async void AttackReady(AttackReadyParam param)
        {
            var token = Owner.CancellationTokenSource.Token;

            var shakeCount = Mathf.CeilToInt(param.AttackReadyTime) * 2;
            Owner.EnemyEffect.ShakeBody(shakeCount);
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