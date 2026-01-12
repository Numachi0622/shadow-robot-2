using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.Character
{
    [CreateAssetMenu(fileName = "FlameThrow", menuName = "Character/AttackPattern/FlameThrow", order = 2)]
    public class FlameThrowPattern : AttackPattern
    {
        [SerializeField] private AttackPoint _attackPoint;
        public override void Execute(CharacterCore owner, AttackReadyParam attackReadyParam)
        {
            if (owner is not BossEnemyCore bossEnemy) return;
            FireAsync(bossEnemy).Forget();
        }
        
        private async UniTask FireAsync(BossEnemyCore bossEnemy)
        {
            var effect = bossEnemy.EffectComponents.FlameThrowEffect;
            var attackCollider = effect?.GetComponent<AttackCollider>();
            if (effect == null || attackCollider == null)
            {
                Debug.LogError("[FlameThrowPattern] Effect or AttackCollider is null");
                return;
            }

            await UniTask.Delay(700);
            effect.Play();
            attackCollider.AttackImpact(new AttackParam()
            {
                AttackPoint = _attackPoint,
                AttackType = AttackType.EnemyToPlayerNormal,
            });
            
            await UniTask.WaitUntil(() => !effect.isPlaying);
            attackCollider.AttackImpactEnd();
        }
    }
}