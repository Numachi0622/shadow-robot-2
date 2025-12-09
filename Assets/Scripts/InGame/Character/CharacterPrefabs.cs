using System.Collections.Generic;
using UnityEngine;

namespace InGame.Character
{
    [CreateAssetMenu(fileName = "CharacterPrefabs", menuName = "Character/CharacterPrefabs")]
    public class CharacterPrefabs : ScriptableObject
    {
        public PlayerCore PlayerPrefab;
        public NormalEnemyCore NormalEnemyPrefab;
        public BossEnemyCore BossEnemyPrefab;
        public List<BuildingCore> BuildingPrefabs;
    }
}