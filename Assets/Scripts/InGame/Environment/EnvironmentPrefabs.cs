using UnityEngine;

namespace InGame.Environment
{
    [CreateAssetMenu(fileName = "EnvironmentPrefabs", menuName = "Environment/EnvironmentPrefabs")]
    public class EnvironmentPrefabs : ScriptableObject
    {
        public GameObject StageElementPrefab;
    }
}