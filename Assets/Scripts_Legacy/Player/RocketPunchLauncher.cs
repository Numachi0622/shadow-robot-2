// using System;
// using Interface;
// using UnityEngine;
//
// public class RocketPunchLauncher : MonoBehaviour
// {
//     [SerializeField] private RocketPunchAttacker _rocketPunchPrefab;
//     [SerializeField] private Transform _playerTransform;
//
//     public void Launch(Vector3 dir, Action onStart = null, Action onComplete = null)
//     {
//         var rocketPunch = Instantiate(_rocketPunchPrefab, _playerTransform.position + Vector3.up, Quaternion.identity);
//         rocketPunch.Initialize(_playerTransform);
//         onStart?.Invoke();
//         rocketPunch.Execute(dir, onComplete);
//     }
// }
