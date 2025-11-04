// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using Utility;
//
// public class HitEffectManager : Singleton<HitEffectManager>
// {
//     [SerializeField] private Transform _parent;
//     [SerializeField] private ParticleSystem _enemyHitEffectPrefab;
//     [SerializeField] private ParticleSystem _enemyDeadHitEffectPrefab;
//     [SerializeField] private ParticleSystem _playerHitEffectPrefab;
//
//     private Dictionary<AttackType, ParticleSystem> _hitEffectPrefabs;
//     private Dictionary<AttackType, List<ParticleSystem>> _hitEffectPool;
//     
//     public override void Initialize()
//     {
//         _hitEffectPrefabs = new Dictionary<AttackType, ParticleSystem>()
//         {
//             { AttackType.EnemyToPlayerNormal , _playerHitEffectPrefab },
//             { AttackType.PlayerToEnemyNormal , _enemyHitEffectPrefab },
//             { AttackType.PlayerToEnemyDead , _enemyDeadHitEffectPrefab }
//         };
//         
//         _hitEffectPool = new Dictionary<AttackType, List<ParticleSystem>>()
//         {
//             { AttackType.EnemyToPlayerNormal , new List<ParticleSystem>() },
//             { AttackType.PlayerToEnemyNormal , new List<ParticleSystem>() },
//             { AttackType.PlayerToEnemyDead , new List<ParticleSystem>() }
//         };
//         
//         base.Initialize();
//     }
//     public void Play(AttackType attackType, Vector3 pos)
//     {
//         var hitEffect = Get(attackType);
//         hitEffect.transform.position = pos;
//         hitEffect.Play();
//     }
//
//     private ParticleSystem Get(AttackType attackType)
//     {
//         ParticleSystem hitEffect;
//
//         for (var i = 0; i < _hitEffectPool[attackType].Count; i++)
//         {
//             hitEffect = _hitEffectPool[attackType][i];
//             if(hitEffect.gameObject.activeSelf) continue;
//             hitEffect.gameObject.SetActive(true);
//             return hitEffect;
//         }
//
//         hitEffect = Instantiate(_hitEffectPrefabs[attackType], _parent);
//         _hitEffectPool[attackType].Add(hitEffect);
//         return hitEffect;
//     }
// }
