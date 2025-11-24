using System;
using Cysharp.Threading.Tasks;
using InGame.System;
using UnityEngine;
using UniRx;

namespace InGame.Character
{
    public class BossEnemyCoolTimeState : StateMachine<BossEnemyCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[BossEnemyCoolTimeState] OnEnter");
            CoolTime();
        }
        
        private async void CoolTime()
        {
            var token = Owner.CancellationTokenSource.Token;
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(Owner.Params.AttackCoolTime), cancellationToken: token);
            }
            catch (OperationCanceledException e)
            {
                return;
            }
            Owner.OnIdleStart(Unit.Default);
        }    
    }
}
