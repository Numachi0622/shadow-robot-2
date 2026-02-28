using UnityEngine;

namespace InGame.Character
{
    [CreateAssetMenu(fileName = "RocketPunchAttackPattern", menuName = "Character/AttackPattern/RocketPunch", order = 3)]
    public class RocketPunchAttackPattern : AttackPattern
    {
        [SerializeField] private RocketPunch _prefab;
        [SerializeField] private AttackPoint _attackPoint;
        [SerializeField] private float _speed = 10f;

        public override void Execute(CharacterCore owner, AttackParam attackParam)
        {
            if (owner is not PlayerCore player) return;

            var spawnPos = attackParam.Origin;
            var instance = Instantiate(_prefab, spawnPos, Quaternion.identity);
            instance.Fire(player, attackParam);
        }
    }
}