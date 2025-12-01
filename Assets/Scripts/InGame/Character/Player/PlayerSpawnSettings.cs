using System.Collections.Generic;
using UnityEngine;

namespace InGame.Character
{
    [CreateAssetMenu(fileName = "PlayerSpawnSettings", menuName = "Character/PlayerSpawnSettings")]
    public class PlayerSpawnSettings : ScriptableObject
    {
        public List<Vector3> SpawnPositions = new List<Vector3>(4);
    }
}