using System;
using Cysharp.Threading.Tasks;
using InGame.Message;
using InGame.System;
using MessagePipe;
using UnityEngine;
using Utility;

namespace InGame.Character
{
    public class BossEnemyAttackReadyState : StateMachine<BossEnemyCore>.State
    {
        private const int ShakeCount = 2;
        private const int DeathBallIndex = 2;
        private UniTaskCompletionSource _poseMatchEventEndSource;
        private IDisposable _subscription;
        
        public override void OnEnter(IStateParameter parameter = null)
        {
            if (parameter is AttackReadyParam param)
            {
                Debug.Log("[BossEnemyAttackReady] OnEnter");

                var attackIndex = Owner.Params.SelectAttackPatternIndex();
                var animationHash = AnimationUtility.AttackReadyHash(attackIndex);
                Owner.Animator.SetTrigger(animationHash);
                param.AttackIndex = attackIndex;

                AttackReadyAsync(param).Forget();
            }
        }
        
        private async UniTask AttackReadyAsync(AttackReadyParam param)
        {
            var token = Owner.CancellationTokenSource.Token;
            Owner.EnemyEffect.ShakeBody(ShakeCount);
            
            try
            {
                await Owner.Attacker.AttackReady(token, param.Direction, 0f, param.AttackReadyTime);
                
                if (param.AttackIndex == DeathBallIndex)
                {
                    _poseMatchEventEndSource = new UniTaskCompletionSource();
            
                    // 購読対象を新規登録
                    var bag = DisposableBag.CreateBuilder();
                    Owner.PoseMatchEventEndSubscriber.Subscribe(_ => _poseMatchEventEndSource.TrySetResult()).AddTo(bag);
                    _subscription = bag.Build();
                    
                    Owner.PoseMatchEventStartPublisher.Publish(new PoseMatchEventStartMessage());
                    await _poseMatchEventEndSource.Task;
                }
            }
            catch (OperationCanceledException e)
            {
                return;
            }
            Owner.OnAttackStart(param);
        }
        
        public override void OnExit()
        {
            _subscription?.Dispose();
            _poseMatchEventEndSource = null;
        }
    }
}
