using System;
using UnityEngine;

public class EnemyTester : MonoBehaviour
{
    [SerializeField] private EnemyGenerator _enemyGenerator;

    private void Start()
    {
        _enemyGenerator.Initialilze();
        _enemyGenerator.Generate();
    }
}
