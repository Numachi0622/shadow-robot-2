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
                
                // アニメーション再生
                var attackIndex = Random.Range(0, 4);
                var animationHash = AnimationUtility.AttackReadyHash(attackIndex);
                Owner.Animator.SetTrigger(animationHash);
                param.AttackIndex = attackIndex;
                
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