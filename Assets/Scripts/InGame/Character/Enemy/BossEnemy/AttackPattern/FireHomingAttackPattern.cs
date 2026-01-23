using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.Character
{
    [CreateAssetMenu(fileName = "FireHoming", menuName = "Character/AttackPattern/FireHoming", order = 2)]
    public class FireHomingAttackPattern : AttackPattern
    {
        [SerializeField] private FireHoming _fireHomingPrefab;
        [SerializeField] private AttackPoint _attackPoint;
        [SerializeField] private float _speed = 5f;
        [SerializeField] private int _instanceCount = 5;
        
        public override void Execute(CharacterCore owner, AttackReadyParam attackReadyParam)
        {
            if (owner is not BossEnemyCore bossEnemy) return;
            FireAsync(bossEnemy).Forget();
        }
        
        private async UniTask FireAsync(BossEnemyCore bossEnemy)
        {
            await UniTask.Delay(700);
            
            var target = bossEnemy.TargetTransform;

            for (var i = 0; i < _instanceCount; i++)
            {
                var attackParam = new AttackParam()
                {
                    AttackPoint = _attackPoint,
                    AttackDirection = RandomDirection,
                    AttackVelocity = _speed,
                    AttackType = AttackType.EnemyToPlayerNormal
                };

                var fireHoming = Instantiate(_fireHomingPrefab, bossEnemy.FireHomingFirePoint.position, Quaternion.identity);
                fireHoming.Fire(attackParam, bossEnemy.Center, target);
                
                await UniTask.Delay(450);
            }
        }

        private Vector3 RandomDirection => new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(0f, 1f),
            Random.Range(-1f, 0f)
        );
    }
}