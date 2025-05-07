using Interface;
using UnityEngine;
using TNRD;

public class InGamePresenter : MonoBehaviour
{
    [SerializeField] private PlayerPresenter _playerPresenter;
    [SerializeField] private DebugParamsPresenter _debugParamsPresenter;
    [SerializeField] private ColorBodySourceView _colorBodySourceView;
    [SerializeField] private EnemyPresenter _enemyPresenter;

    [SerializeField] private SerializableInterface<ISingleton>[] _singletons;

    private void Awake()
    {
        foreach (var t in _singletons)
        {
            t.Value.Initialize();
        }
    }

    private void Start()
    {
        _playerPresenter.Initialize();
        _debugParamsPresenter.Initialize();
        _colorBodySourceView.Initialize();
        
        // debug
        _enemyPresenter.Initialize();
    }
}
