using System;
using Cysharp.Threading.Tasks;
using Unity.Properties;
using UnityEngine;

namespace InGame.Character
{
    [CreateAssetMenu(fileName = "FireBallAttackPattern", menuName = "Character/AttackPattern/FireBall", order = 2)]
    public class FireBallAttackPattern : AttackPattern
    {
        public override void Execute(CharacterCore owner, AttackReadyParam attackReadyParam)
        {
            if (owner is not BossEnemyCore bossEnemy) return;
            
        }

        private async void Fire(BossEnemyCore owner, ParticleSystem prefab)
        {
            // for (var i = 0; i < 4; i++)
            // {
            //     await UniTask.Delay(TimeSpan.FromSeconds(0.5f * i));
            //     Instantiate(owner.NormalEnemyPrefab);
            // }
        }
    }
}