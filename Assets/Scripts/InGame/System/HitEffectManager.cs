using System;
using System.Collections.Generic;
using InGame.Character;
using UnityEngine;
using UnityEngine.Pool;
using Utility;

namespace InGame.System
{
    public class HitEffectManager : Singleton<HitEffectManager>
    {
        [SerializeField] private SerializableDictionary<AttackType, ParticleSystem> _hitEffectPrefabs;
        private readonly int _defaultCapacity = 10;
        private readonly int _maxSize = 20;
        
        private Dictionary<AttackType, ObjectPool<ParticleSystem>> _hitEffectPools = new();

        public override void Initialize()
        {
            foreach (var attackType in _hitEffectPrefabs.Keys)
            {
                var prefab = _hitEffectPrefabs[attackType];
                _hitEffectPools[attackType] = new ObjectPool<ParticleSystem>(
                    createFunc: () => CreateParticleSystem(prefab, attackType),
                    actionOnGet: OnGetFromPool,
                    actionOnRelease: OnReleaseToPool,
                    actionOnDestroy: OnDestroyPoolObject,
                    collectionCheck: true,
                    defaultCapacity: _defaultCapacity,
                    maxSize: _maxSize
                );
            }

            base.Initialize();
        }

        public void Play(AttackType attackType, Vector3 pos)
        {
            var hitEffect = _hitEffectPools[attackType].Get();
            hitEffect.transform.position = pos;
            hitEffect.Play();
        }

        private ParticleSystem CreateParticleSystem(ParticleSystem prefab, AttackType attackType)
        {
            var instance = Instantiate(prefab, transform);
            var main = instance.main;
            main.stopAction = ParticleSystemStopAction.Callback;

            var particleCallback = instance.gameObject.AddComponent<ParticleSystemCallback>();
            particleCallback.OnParticleSystemStopped = () => ReleaseParticleSystem(attackType, instance);

            return instance;
        }

        private void OnGetFromPool(ParticleSystem effect)
        {
            effect.gameObject.SetActive(true);
        }

        private void OnReleaseToPool(ParticleSystem effect)
        {
            effect.gameObject.SetActive(false);
        }

        private void OnDestroyPoolObject(ParticleSystem effect)
        {
            if (effect != null)
            {
                Destroy(effect.gameObject);
            }
        }

        private void ReleaseParticleSystem(AttackType attackType, ParticleSystem effect)
        {
            if (_hitEffectPools != null && _hitEffectPools.ContainsKey(attackType))
            {
                _hitEffectPools[attackType].Release(effect);
            }
        }
    }

    public class ParticleSystemCallback : MonoBehaviour
    {
        public Action OnParticleSystemStopped;

        private void OnStopped()
        {
            Debug.Log("OnStop");
            OnParticleSystemStopped?.Invoke();
        }
    }
}
