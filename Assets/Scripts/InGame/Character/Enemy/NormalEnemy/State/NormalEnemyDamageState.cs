using System;
using Cysharp.Threading.Tasks;
using InGame.System;
using UniRx;
using UnityEngine;

namespace InGame.Character
{
    public class NormalEnemyDamageState : StateMachine<NormalEnemyCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            if (parameter is AttackParam param)
            {
                Owner.AttackCancel();
                Debug.Log($"<color=red>[NormalEnemyDamageState] OnEnter : {param.AttackPoint.RandomValue}</color>");
                Owner.Damager.Damage(param.AttackPoint.RandomValue);
                DamageCoolTime();
            }
        }
        
        private async void DamageCoolTime()
        {
            var token = Owner.CancellationTokenSource.Token;
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(Owner.Params.DamageCoolTime), cancellationToken: token);
            }
            catch (OperationCanceledException e)
            {
                return;
            }
            Owner.OnIdleStart(Unit.Default);
        }
    }
}