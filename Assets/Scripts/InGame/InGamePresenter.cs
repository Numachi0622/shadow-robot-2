using Interface;
using UnityEngine;
using TNRD;
using UniRx.Triggers;
using UniRx;

public class InGamePresenter : MonoBehaviour
{
    [SerializeField] private PlayerPresenter _playerPresenter;
    [SerializeField] private ColorBodySourceView _colorBodySourceView;
    [SerializeField] private EnemyGenerator _enemyGenerator;
    [SerializeField] private TrackingDebugView _debugView;
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
        _colorBodySourceView.Initialize();
        _enemyGenerator.Initialilze();

        Application.targetFrameRate = 60;
        
        Bind();
    }

    private void Bind()
    {
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.D))
            .Subscribe(_ =>
            {
                var isActive = _debugView.gameObject.activeSelf;
                _debugView.gameObject.SetActive(!isActive);
            })
            .AddTo(this);

        this.UpdateAsObservable()
            .Select(fps => Mathf.RoundToInt(1.0f / Time.deltaTime))
            .Subscribe(_debugView.UpdateFpsView)
            .AddTo(this);
    }
}
