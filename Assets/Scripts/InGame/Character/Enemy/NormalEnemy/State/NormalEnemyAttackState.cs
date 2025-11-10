using InGame.System;
using UnityEngine;

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
                Owner.Effect.AttackEffect.Play();
                Owner.OnCoolTimeStart();
            }   
        }
    }
}