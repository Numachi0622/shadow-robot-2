using System;
using Cysharp.Threading.Tasks;
using InGame.Message;
using InGame.System;
using Unity.Properties;
using UnityEngine;

namespace InGame.Character
{
    [CreateAssetMenu(fileName = "SummonAttackPattern", menuName = "Character/AttackPattern/Summon", order = 1)]
    public class SummonAttackPattern : AttackPattern
    {
        public override void Execute(CharacterCore owner, AttackReadyParam attackReadyParam)
        {
            if (owner is not BossEnemyCore bossEnemy) return;
            
            Summon(bossEnemy);
        }

        private async void Summon(BossEnemyCore owner)
        {
            for (var i = 0; i < 4; i++)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f * i));
                //Instantiate(owner.NormalEnemyPrefab);
                // todo: MessagePipeで生成
                owner.SummonEnemyPublisher.Publish(new SpawnCharacterMessage(
                    CharacterType.NormalEnemy,
                    owner.transform.position + UnityEngine.Random.insideUnitSphere * 5f,
                    Quaternion.identity,
                    new AreaId(-1),
                    TargetType.Player
                ));
            }
        }
    }
}