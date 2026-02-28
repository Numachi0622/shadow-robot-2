using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.Character
{
    [CreateAssetMenu(fileName = "LongLaserAttackPattern", menuName = "Character/AttackPattern/LongLaser", order = 2)]
    public class LongLaserAttackPattern : AttackPattern
    {
        [SerializeField] private float _attackPointWeight = 2f;
        
        public override void ExecuteReady(CharacterCore owner)
        {
            if (owner is not NormalEnemyCore normalEnemy) return;
            
            var effect = normalEnemy.Effect.LongLaserEffect.GetComponent<LongLaser>();
            effect.PlayWarningEffectAsync(normalEnemy.Params.AttackReadyTime).Forget();
            
            var rotator = owner.Mover as NormalEnemyMover;
            var target = normalEnemy.CharacterRegistry.GetAllPlayers().FirstOrDefault()?.transform;
            if (rotator == null || target == null) return;

            var dir = (target.position - owner.transform.position).normalized;
            rotator.RotateAsync(dir).Forget();
        }
        
        public override void Execute(CharacterCore owner, AttackReadyParam attackReadyParam)
        {
            if (!(owner is NormalEnemyCore normalEnemy)) return;

            var attackEffect = normalEnemy.Effect.LongLaserEffect;
            attackEffect?.Play();

            var attackCollider = attackEffect?.GetComponent<AttackCollider>();
            var attackPoint = normalEnemy.Params.AttackPoint;
            attackPoint.SetWeight(_attackPointWeight);
            
            var attackParam = new AttackParam()
            {
                AttackPoint = attackPoint,
                AttackType = AttackType.EnemyToPlayerNormal,
            };
            attackCollider?.AttackImpactAsync(attackParam, attackReadyParam.AttackImpactWaitTime).Forget();
        }
        
        public override void ExecuteReadyCancel(CharacterCore owner)
        {
            if (owner is not NormalEnemyCore normalEnemy) return;
            
            var effect = normalEnemy.Effect.LongLaserEffect.GetComponent<LongLaser>();
            effect.StopWarningEffect();
        }
    }
}