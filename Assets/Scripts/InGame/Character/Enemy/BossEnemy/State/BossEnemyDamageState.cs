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
                
                HitEffectManager.Instance.Play(param.AttackType, param.HitPosition);
                
                var damageValue = param.AttackPoint.RandomValue;
                if (Owner.CurrentHp - damageValue > 0 && damageValue < 300)
                {
                    Owner.Damager.Damage(damageValue);
                    Owner.OnIdleStart(Unit.Default);
                    return;
                }
                
                // 既にトリガーされていたアニメーションをキャンセルしてからダメージアニメーションを再生する
                Owner.Animator.ResetAllTriggers();
                Owner.Animator.SetTrigger(AnimationUtility.DamageHash);
                Owner.Damager.Damage(damageValue);
                Owner.AttackCancel();
                Owner.EnemyEffect.ShakeBody();
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
