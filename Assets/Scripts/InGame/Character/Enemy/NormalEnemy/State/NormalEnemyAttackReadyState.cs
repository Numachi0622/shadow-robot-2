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
            await Owner.Attacker.AttackReady(param.Direction, 0f, param.AttackReadyTime);
            Owner.OnAttackStart(param);
        }
    }
}