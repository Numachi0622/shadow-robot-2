using System;
using Cysharp.Threading.Tasks.Triggers;
using MessagePipe;
using UnityEngine;

namespace InGame.System
{
    public class TitleState : StateMachine<InGameCore>.State
    {
        private IDisposable _subscription;
        
        public override void OnEnter(IStateParameter parameter = null)
        {
            Debug.Log("[TitleState] OnEnter");
            
            SubscriptionBind();
            Owner.StageReferences.TitleStage.SetActive(true);
        }
        
        public override void OnExit()
        {
            Owner.StageReferences.TitleStage.SetActive(false);
            Owner.TitlePresenter.gameObject.SetActive(false);
            _subscription?.Dispose();
        }

        private void SubscriptionBind()
        {
            var bag = DisposableBag.CreateBuilder();
            
            Owner.CharacterSpawnRequestSubscriber.Subscribe(request =>
            {
                Owner.SpawnCharacterPublisher.Publish(request.SpawnCharacterMessage);
            }).AddTo(bag);
            
            Owner.CharacterDespawnRequestSubscriber.Subscribe(request =>
            {
                Owner.DespawnCharacterPublisher.Publish(request.DespawnCharacterMessage);
            }).AddTo(bag);

            _subscription = bag.Build();
        }
    }
}