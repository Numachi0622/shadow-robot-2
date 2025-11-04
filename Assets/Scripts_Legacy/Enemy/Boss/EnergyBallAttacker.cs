// using UniRx;
// using UniRx.Triggers;
// using UnityEngine;
//
// namespace Enemy.Boss
// {
//     public class EnergyBallAttacker : EnemyAttackerBase
//     {
//         [SerializeField] private AttackPoint _energyBallAttackPoint;
//         [SerializeField] private float _lifeTime = 5f;
//         [SerializeField] private float _spped = 10f;
//         
//         public override void Initialize(EnemyParams enemyParams)
//         {
//             _attackPoint = _energyBallAttackPoint;
//             _attackInfo = new AttackInfo()
//             {
//                 AttackPoint = _attackPoint,
//                 AttackDirection = Vector3.zero,
//                 AttackVelocity = 0f,
//                 AttackType = AttackType.EnemyToPlayerNormal
//             };
//         }
//
//         public void Execute(Vector3 dir)
//         {
//             this.UpdateAsObservable()
//                 .Subscribe(_ =>
//                 {
//                     transform.localPosition += dir * (_spped * Time.deltaTime);
//                     Destroy(gameObject, _lifeTime);
//                 })
//                 .AddTo(this);
//         }
//     }
// }