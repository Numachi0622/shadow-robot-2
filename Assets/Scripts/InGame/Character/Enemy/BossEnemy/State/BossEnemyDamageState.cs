using System;
using InGame.System;
using UnityEngine;
using Utility;
using Utility.Extensions;
using Cysharp.Threading.Tasks;
using UniRx;

namespace InGame.Character
{
    public class BossEnemyDamageState : StateMachine<BossEnemyCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            if (parameter is AttackParam param)
            {
                Debug.Log($"<color=red>[BossEnemyDamageState] OnEnter : {param.AttackPoint.RandomValue}</color>");
                
                var damageValue = param.AttackPoint.RandomValue;
                
                // 既にトリガーされていたアニメーションをキャンセルしてからダメージアニメーションを再生する
                Owner.Animator.ResetAllTriggers();
                Owner.Animator.SetTrigger(AnimationUtility.DamageHash);
                Owner.Damager.Damage(damageValue);
                Owner.AttackCancel();
                Owner.EnemyEffect.ShakeBody();
                HitEffectManager.Instance.Play(param.AttackType, param.HitPosition);
                
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
