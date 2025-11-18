using UnityEngine;

namespace InGame.Character
{
    [CreateAssetMenu(fileName = "LaserAttackPattern", menuName = "Character/AttackPattern/Laser", order = 1)]
    public class LaserAttackPattern : AttackPattern
    {
        public override void Execute(CharacterCore owner)
        {
            if (!(owner is NormalEnemyCore normalEnemy)) return;
            normalEnemy.Effect.AttackEffect.Play();
        }
    }
}