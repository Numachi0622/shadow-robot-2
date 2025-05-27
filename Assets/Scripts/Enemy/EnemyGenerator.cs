using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField] private EnemyPresenterBase _enemyPrefab;
    [SerializeField] private int _count;
    [SerializeField] private Transform _enemyViewParent;

    public void Initialilze()
    {
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.S))
            .Subscribe(_ => Generate())
            .AddTo(this);
    }

    public void Generate()
    {
        for (var i = 0; i < _count; i++)
        {
            var enemy = Instantiate(_enemyPrefab, GetRandomPosition(), Quaternion.identity, transform);
            enemy.Initialize(_enemyViewParent);
        }
    }

    private Vector3 GetRandomPosition() => new Vector3(Random.Range(-20f, 20f), 0f, Random.Range(10, 20));
}
