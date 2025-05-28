using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyTester : MonoBehaviour
{
    [SerializeField] private EnemyGenerator _enemyGenerator;

    private void Start()
    {
        _enemyGenerator.Initialilze();
        _enemyGenerator.GenerateLoopBeforeBoss().Forget();
    }
}
