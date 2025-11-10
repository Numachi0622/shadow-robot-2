using System;
using InGame.System;
using UnityEngine;

namespace InGame.Character
{
    public class NormalEnemyAttackReadyState : StateMachine<NormalEnemyCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            if (parameter is EnemyAttackParam param)
            {
                Debug.Log("[NormalEnemyAttackReadyState] OnEnter");
                AttackReady(param);
            }   
        }

        private async void AttackReady(EnemyAttackParam param)
        {
            var token = Owner.CancellationTokenSource.Token;
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