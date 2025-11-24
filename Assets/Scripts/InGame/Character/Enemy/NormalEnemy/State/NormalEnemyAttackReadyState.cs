using System;
using InGame.System;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace InGame.Character
{
    public class NormalEnemyAttackReadyState : StateMachine<NormalEnemyCore>.State
    {
        private readonly int _shakeCount = 2;
        
        public override void OnEnter(IStateParameter parameter = null)
        {
            if (parameter is EnemyAttackParam param)
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

        private async void AttackReady(EnemyAttackParam param)
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