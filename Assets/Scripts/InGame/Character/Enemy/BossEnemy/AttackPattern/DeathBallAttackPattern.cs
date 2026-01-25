using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.Character
{
    [CreateAssetMenu(fileName = "DeathBallAttackPattern", menuName = "Character/AttackPattern/DeathBall", order = 3)]
    public class DeathBallAttackPattern : AttackPattern
    {
        [SerializeField] private DeathBall _deathBallPrefab;
        [SerializeField] private AttackPoint _attackPoint;
        [SerializeField] private float _speed = 5f;
        private DeathBall _instance;

        public override void ExecuteReady(CharacterCore owner)
        {
            if (owner is not BossEnemyCore bossEnemy) return;
            _instance = Instantiate(_deathBallPrefab, bossEnemy.DeathBallFirePoint.position, Quaternion.identity);
            _instance.InitializeAsync().Forget();
        }
        
        public override void Execute(CharacterCore owner, AttackReadyParam attackReadyParam)
        {
            if (owner is not BossEnemyCore bossEnemy) return;
            FireAsync(bossEnemy).Forget();
        }

        private async UniTask FireAsync(BossEnemyCore bossEnemy)
        {
            await UniTask.Delay(1500);

            var player = bossEnemy.TargetTransform;
            if (player == null)
            {
                Debug.LogError("[DeathBallAttackPattern] No target player found.");
                return;
            }
            
            var attackParam = new AttackParam()
            {
                AttackPoint = _attackPoint,
                AttackDirection = (player.position - _instance.transform.position).normalized,
                AttackVelocity = _speed,
                AttackType = AttackType.EnemyToPlayerNormal
            };
            
            _instance.Fire(attackParam, bossEnemy.transform);
        }
    }
}