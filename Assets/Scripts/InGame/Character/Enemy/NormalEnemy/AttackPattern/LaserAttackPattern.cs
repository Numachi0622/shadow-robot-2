using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.Character
{
    [CreateAssetMenu(fileName = "LaserAttackPattern", menuName = "Character/AttackPattern/Laser", order = 1)]
    public class LaserAttackPattern : AttackPattern
    {
        public override void Execute(CharacterCore owner, AttackReadyParam attackReadyParam)
        {
            if (!(owner is NormalEnemyCore normalEnemy)) return;

            var attackEffect = normalEnemy.Effect.AttackEffect;
            attackEffect?.Play();

            var attackCollider = attackEffect?.GetComponent<AttackCollider>();
            var attackParam = new AttackParam()
            {
                AttackPoint = normalEnemy.Params.AttackPoint,
                AttackType = AttackType.EnemyToPlayerNormal,
            };
            attackCollider?.AttackImpactAsync(attackParam, attackReadyParam.AttackImpactWaitTime).Forget();
        }
    }
}