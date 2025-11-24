using UnityEngine;

namespace InGame.Character
{
    [CreateAssetMenu(fileName = "PlayerParams", menuName = "Character/Params/PlayerParams")]
    public class PlayerParams : CharacterParams
    {
        public float AttackableVelocity = 1.0f;
        [Range(0f, 1f)] public float RocketPunchDirThreshold = 0.8f;
        [Range(0f, 1f)] public float RocketPunchDistThreshold = 0.5f;
        public float MaxRocketPunchReadyTime = 1f;
        public float RocketPunchChargeDistanceThreshold = 1f;
    }
}