using UnityEngine;

[CreateAssetMenu(fileName = "PlayerParams", menuName = "ScriptableObjects/PlayerParams")]
public class PlayerParams : CharacterParams
{
    public float AttackableVelocity = 1.0f;
    [Range(0f, 1f)] public float RocketPunchDirThreshold = 0.8f;
    public float MaxRocketPunchReadyTime = 1f;
    public float RocketPunchChargeDistanceThreshold = 1f;
}
