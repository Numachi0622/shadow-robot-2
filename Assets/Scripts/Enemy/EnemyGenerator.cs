using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using Random = UnityEngine.Random;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField] private EnemyPresenterBase _enemyPrefab, _bossPrefab;
    [SerializeField] private KnockDownCountPresenter _knockDownCountPresenter;
    [SerializeField] private Transform _enemyViewParent;
    [SerializeField] private float _generateInterval;

    private List<EnemyPresenterBase> _enemyList = new List<EnemyPresenterBase>();

    public void Initialilze()
    {
        _knockDownCountPresenter.Initialize();
        _knockDownCountPresenter.OnKnockDownBossGenerationCount = () =>
        {
            GenerateBoss();
            //GenerateLoop().Forget();
        };
    }

    private void GenerateBoss()
    {
        var enemy = Instantiate(_bossPrefab, GetRandomPosition(), Quaternion.identity, transform);
        enemy.Initialize(_enemyViewParent);
        enemy.OnDead += () =>
        {
            _knockDownCountPresenter.IsBossKnocedDown = true;
            AllEnemyRemove();
        };
    }

    private async UniTask GenerateLoop()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(10f));
        while (true)
        {
            if(_knockDownCountPresenter.IsBossKnocedDown) break;
            await UniTask.Delay(TimeSpan.FromSeconds(_generateInterval));
            var enemy = Instantiate(_enemyPrefab, GetRandomPosition(), Quaternion.identity, transform);
            enemy.Initialize(_enemyViewParent);
            _enemyList.Add(enemy);
            enemy.OnDead += () =>
            {
                _enemyList.Remove(enemy);
                _knockDownCountPresenter.KnockDownCount();
            };
        }
    }

    public async UniTask GenerateLoopBeforeBoss()
    {
        for (var i = 0; i < GameConst.BOSS_GENERATION_KNOCK_DOWN_COUNT; i++)
        {
            var enemy = Instantiate(_enemyPrefab, GetRandomPosition(), Quaternion.identity, transform);
            enemy.Initialize(_enemyViewParent);
            _enemyList.Add(enemy);
            enemy.OnDead += () =>
            {
                _enemyList.Remove(enemy);
                _knockDownCountPresenter.KnockDownCount();
            };
            
            await UniTask.Delay(TimeSpan.FromSeconds(_generateInterval));
        }
    }

    private void AllEnemyRemove()
    {
        foreach (var enemy in _enemyList)
        {
            enemy.gameObject.SetActive(false);
        }
    }

    private Vector3 GetRandomPosition() => new Vector3(Random.Range(-20f, 20f), 0f, Random.Range(10, 20));
}
