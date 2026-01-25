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
        private const int DeathBallTurnCount = 4;
        private int _attackCount;
        private int _prevAttackCount = -1;
        private UniTaskCompletionSource _poseMatchEventEndSource;
        private IDisposable _subscription;
        
        public override void OnEnter(IStateParameter parameter = null)
        {
            if (parameter is AttackReadyParam param)
            {
                Debug.Log("[BossEnemyAttackReady] OnEnter");
                _attackCount++;

                int attackIndex;
                if (_attackCount % DeathBallTurnCount == 0)
                {
                    attackIndex = DeathBallIndex;
                }
                else
                {
                    do
                    {
                        attackIndex = Owner.Params.SelectAttackPatternIndex();
                    } while (attackIndex == DeathBallIndex || attackIndex == _prevAttackCount);
                }
                var animationHash = AnimationUtility.AttackReadyHash(attackIndex);
                Owner.Animator.SetTrigger(animationHash);
                param.AttackIndex = attackIndex;
                _prevAttackCount = attackIndex;

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
                    
                    // DeathBallは生成しておく
                    var attackPatternParams = Owner.Params.AttackPatternParams[param.AttackIndex];
                    var attackPattern = attackPatternParams.AttackPattern as DeathBallAttackPattern;
                    if (attackPattern == null)
                    {
                        Debug.LogError("[BossEnemyAttackReady] DeathBallAttackPattern is null.");
                        return;
                    }
                    attackPattern.ExecuteReady(Owner);
                    
                    // オーラエフェクト再生
                    Owner.EffectComponents.DeathBallAura.Play();
                    
                    // Summonで生成した雑魚敵は一旦削除する
                    Owner.AllEnemyDespawnPublisher.Publish(new AllEnemyDespawnMessage());
                    
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
            DeathBallAuraStopAsync().Forget();
            _subscription?.Dispose();
            _poseMatchEventEndSource = null;
        }

        private async UniTask DeathBallAuraStopAsync()
        {
            await UniTask.Delay(1000);
            Owner.EffectComponents.DeathBallAura.Stop();
        }
    }
}
