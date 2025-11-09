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
                Owner.Damager.Damage(param.AttackPoint.RandomValue);
            }
        }
        
        public override void OnUpdate()
        {
            base.OnUpdate();
        }
        
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}