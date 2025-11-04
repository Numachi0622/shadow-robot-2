// using System;
// using Cysharp.Threading.Tasks;
// using UnityEngine;
// using Utility;
//
// namespace Enemy.Boss
// {
//     public class FirePillarAttacker : EnemyAttackerBase
//     {
//         [SerializeField] private ParticleSystem _firePillarParticle;
//         [SerializeField] private AttackPoint _firePillarAttackPoint;
//         [SerializeField] private float _delay = 2f;
//         
//         public override void Initialize(EnemyParams enemyParams)
//         {
//             _attackPoint = _firePillarAttackPoint;
//             _attackInfo = new AttackInfo()
//             {
//                 AttackPoint = _attackPoint,
//                 AttackDirection = Vector3.zero,
//                 AttackVelocity = 0f,
//                 AttackType = AttackType.EnemyToPlayerNormal
//             };
//         }
//
//         public void Execute()
//         {
//             AttackImpact(_delay).Forget();
//         }
//
//         protected override async UniTask AttackImpact(float waitTime)
//         {
//             SoundManager.Instance.PlaySe(8);
//             await UniTask.Delay(TimeSpan.FromSeconds(_delay));
//             
//             SoundManager.Instance.PlaySe(7);
//             _attackCollider.enabled = true;
//             _firePillarParticle.Play();
//             Destroy(gameObject, 1.2f);
//         }
//     }
// }