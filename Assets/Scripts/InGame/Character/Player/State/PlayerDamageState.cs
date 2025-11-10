using InGame.System;
using UnityEngine;

namespace InGame.Character
{
    public class PlayerDamageState : StateMachine<PlayerCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            if (parameter is AttackParam param)
            {
                Debug.Log($"[PlayerDamageState] OnEnter : {param.AttackPoint.RandomValue}");
                Owner.Damager.Damage(param.AttackPoint.RandomValue);
            }
        }
    }
}