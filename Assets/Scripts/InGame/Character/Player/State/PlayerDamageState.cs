using InGame.System;
using UnityEngine;

namespace InGame.Character
{
    public class PlayerDamageState : StateMachine<PlayerCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            base.OnEnter(parameter);
            if (parameter is AttackParam param)
            {
                Debug.Log($"{param.AttackPoint}のダメージを受けた");   
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