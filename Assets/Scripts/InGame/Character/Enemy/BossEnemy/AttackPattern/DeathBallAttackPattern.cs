using Unity.Properties;
using UnityEngine;

namespace InGame.Character
{
    [CreateAssetMenu(fileName = "DeathBallAttackPattern", menuName = "Character/AttackPattern/DeathBall", order = 3)]
    public class DeathBallAttackPattern : AttackPattern
    {
        public override void Execute(CharacterCore owner, AttackReadyParam attackReadyParam)
        {
            if (owner is not BossEnemyCore bossEnemy) return;
        }
    }
}