using System;
using Cysharp.Threading.Tasks;
using InGame.System;
using UniRx;
using UnityEngine;
using Utility;
using Utility.Extensions;

namespace InGame.Character
{
    public class NormalEnemyDamageState : StateMachine<NormalEnemyCore>.State
    {
        public override void OnEnter(IStateParameter parameter = null)
        {
            if (parameter is AttackParam param)
            {
                Debug.Log($"<color=red>[NormalEnemyDamageState] OnEnter : {param.AttackPoint.RandomValue}</color>");
                
                // 既にトリガーされていたアニメーションをキャンセルしてからダメージアニメーションを再生する
                Owner.Animator.ResetAllTriggers();
                Owner.Animator.SetTrigger(AnimationUtility.DamageHash);
                
                Owner.AttackCancel();
                Owner.Damager.Damage(param.AttackPoint.RandomValue);
                
                // 正面とパンチした平均の方向に制御
                if (Owner.PlayType == PlayType.Solo)
                {
                    var aveDir = ((param.AttackDirection + Vector3.forward) * 0.5f).normalized;
                    Owner.EnemyEffect.KnockBack(aveDir);
                }
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