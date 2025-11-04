// using System;
// using Cysharp.Threading.Tasks;
// using UnityEngine;
// using Utility;
//
// public class EnemyAttacker : EnemyAttackerBase
// {
//     public override void Attack(float waitTime = 0)
//     {
//         AttackImpact(waitTime).Forget();    
//     }
//     
//     protected override async UniTask AttackImpact(float waitTime)
//     {
//         await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
//         
//         _attackCollider.enabled = true;
//         await UniTask.Delay(TimeSpan.FromSeconds(GameConst.COLLIDER_ACTIVE_TIME));
//         _attackCollider.enabled = false;
//         
//         AttackCoolTime(_params.AttackCoolTime).Forget();
//     }
//
//     public override async UniTask AttackReady()
//     {
//         await UniTask.Delay(TimeSpan.FromSeconds(_params.AttackReadyTime));
//         SoundManager.Instance.PlaySe(3);
//         OnAttackReadied?.Invoke();
//     }
// }
