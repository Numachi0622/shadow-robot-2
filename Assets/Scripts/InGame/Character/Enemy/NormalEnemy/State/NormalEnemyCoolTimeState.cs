using System;
using Cysharp.Threading.Tasks;
using InGame.System;
using UniRx;
using UnityEngine;

namespace InGame.Character
{
    public class NormalEnemyCoolTimeState : StateMachine<NormalEnemyCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[NormalEnemyCoolTimeState] OnEnter");
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