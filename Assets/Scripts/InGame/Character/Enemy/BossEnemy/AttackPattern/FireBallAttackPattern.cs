using Cysharp.Threading.Tasks;
using InGame.Message;
using InGame.System.UI;
using UnityEngine;

namespace InGame.Character
{
    [CreateAssetMenu(fileName = "FireBallAttackPattern", menuName = "Character/AttackPattern/FireBall", order = 2)]
    public class FireBallAttackPattern : AttackPattern
    {
        [SerializeField] private FireBall _fireBallPrefab;
        [SerializeField] private int _instanceCount = 10;
        [SerializeField] private float _fireballSpeed = 7.5f;
        [SerializeField] private AttackPoint _attackPoint;
        [SerializeField] private Vector2 _fireBallFireRange = new Vector2(-45f, 45f);

        public override void ExecuteReady(CharacterCore owner)
        {
            if (owner is not BossEnemyCore bossEnemy) return;
            bossEnemy.ShowWarningPublisher.Publish(new ShowWarningMessage(WarningType.FireBall, 2, 0.4f));
        }

        public override void Execute(CharacterCore owner, AttackReadyParam attackReadyParam)
        {
            if (owner is not BossEnemyCore bossEnemy) return;
            FireAsync(bossEnemy).Forget();
        }

        private async UniTask FireAsync(BossEnemyCore bossEnemy)
        {
            var bossTransform = bossEnemy.transform;
            var bossForward = bossTransform.forward;
            var angleRange = _fireBallFireRange.y - _fireBallFireRange.x;
            var angleStep = _instanceCount > 1 ? angleRange / (_instanceCount - 1) : 0;
            var startAngle = _fireBallFireRange.y;

            // await UniTask.Delay(300);
            //
            // for (var i = 0; i < _instanceCount; i++)
            // {
            //     var angle = startAngle - angleStep * i;
            //     var rot = Quaternion.Euler(0, angle, 0);
            //     var dir = rot * bossForward;
            //     
            //     var fireBall = Instantiate(_fireBallPrefab, bossEnemy.FireBallFirePoint.position, Quaternion.identity);
            //     fireBall.Fire(new AttackParam()
            //     {
            //         AttackPoint = _attackPoint,
            //         AttackDirection = dir,
            //         AttackVelocity = _fireballSpeed,
            //         AttackType = AttackType.EnemyToPlayerNormal,
            //     });
            // }

            await UniTask.Delay(500);
            for (var i = 0; i < _instanceCount; i++)
            {
                var angle = startAngle - angleStep * i;
                var rot = Quaternion.Euler(0, angle, 0);
                var dir = rot * bossForward;
                
                var fireBall = Instantiate(_fireBallPrefab, bossEnemy.FireBallFirePoint.position, Quaternion.identity);
                fireBall.Fire(new AttackParam()
                {
                    AttackPoint = _attackPoint,
                    AttackDirection = dir,
                    AttackVelocity = _fireballSpeed,
                    AttackType = AttackType.EnemyToPlayerNormal,
                });
            }
        }
    }
}