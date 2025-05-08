using UnityEngine;
using Utility;

public class HitEffectManager : Singleton<HitEffectManager>
{
    [SerializeField] private ParticleSystem _toEnemyHitEffect;

    public void Play(AttackType attackType, Vector3 pos)
    {
        var hitEffect = Instantiate(_toEnemyHitEffect, transform);
        hitEffect.transform.position = pos;
        hitEffect.Play();
    }
}
