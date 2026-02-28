using System;
using Cysharp.Threading.Tasks;
using InGame.Message;
using InGame.System;
using UnityEngine;

namespace InGame.Character
{
    [CreateAssetMenu(fileName = "SummonAttackPattern", menuName = "Character/AttackPattern/Summon", order = 1)]
    public class SummonAttackPattern : AttackPattern
    {
        [SerializeField] private int _summonCount = 2;
        [SerializeField] private float _summonAreaRadius = 3f;

        private static readonly Vector3[] MultiPlaySummonPositions = new[]
        {
            new Vector3(-5, 0, 7), new Vector3(5, 0, 7),
        };

        public override void Execute(CharacterCore owner, AttackReadyParam attackReadyParam)
        {
            if (owner is not BossEnemyCore bossEnemy) return;
            
            Summon(bossEnemy);
        }

        private async void Summon(BossEnemyCore owner)
        {
            if (owner.TotalPlayerCount != 1)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
                foreach (var pos in MultiPlaySummonPositions)
                {
                    owner.SummonEnemyPublisher.Publish(new SpawnCharacterMessage(
                        CharacterType.NormalEnemy,
                        pos,
                        Quaternion.Euler(0, 180, 0),
                        new AreaId(-1),
                        TargetType.Player,
                        owner.TotalPlayerCount
                    ));
                }
                return;
            }
            
            for (var i = 0; i < _summonCount; i++)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f * i));
                owner.SummonEnemyPublisher.Publish(new SpawnCharacterMessage(
                    CharacterType.NormalEnemy,
                    owner.transform.position + UnityEngine.Random.insideUnitSphere * _summonAreaRadius,
                    Quaternion.Euler(0, 180, 0),
                    new AreaId(-1),
                    TargetType.Player,
                    owner.TotalPlayerCount
                ));
            }
        }
    }
}