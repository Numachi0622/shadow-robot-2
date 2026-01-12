using Cysharp.Threading.Tasks;
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

        public override void Execute(CharacterCore owner, AttackReadyParam attackReadyParam)
        {
            if (owner is not BossEnemyCore bossEnemy) return;
            // todo: 挙動変更予定
            FireAsync(bossEnemy).Forget();
        }

        private async UniTask FireAsync(BossEnemyCore bossEnemy)
        {
            var bossTransform = bossEnemy.transform;

            await UniTask.Delay(500);

            for (var i = 0; i < _instanceCount; i++)
            {
                await UniTask.Delay(50);
                var fireBall = Instantiate(_fireBallPrefab, bossEnemy.FireBallFirePoint.position, Quaternion.identity);
                fireBall.Fire(new AttackParam()
                {
                    AttackPoint = _attackPoint,
                    AttackDirection = bossEnemy.FireBallFirePoint.forward,
                    AttackVelocity = _fireballSpeed,
                    AttackType = AttackType.EnemyToPlayerNormal,
                });
            }
        }
    }
}