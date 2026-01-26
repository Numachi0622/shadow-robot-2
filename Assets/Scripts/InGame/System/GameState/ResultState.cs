using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Message;
using UnityEngine;

namespace InGame.System
{
    public class ResultState : StateMachine<InGameCore>.State
    {
        public override async void OnEnter(IStateParameter parameter = null)
        {
            if (parameter is not ResultContextMessage context) return;
            
            Debug.Log("[ResultState] OnEnter");
            Owner.AllEnemyDespawnMessage.Publish(new AllEnemyDespawnMessage());

            var cts = new CancellationTokenSource();
            try
            {
                await OnEnterAsync(context, cts.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async UniTask OnEnterAsync(ResultContextMessage context, CancellationToken token)
        {
            await Owner.InGameUIController.PlayResultSequenceAsync(context.TotalBuildingCount);
        }
    }
}
