using Cysharp.Threading.Tasks;
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
    [SerializeField] private FollowingCamera _followingCamera; 
    [SerializeField] private GameStatePresenter _gameStatePresenter;
    
    [SerializeField] private SerializableInterface<ISingleton>[] _singletons;

    [SerializeField] private bool _isSkip = false; 

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
        _followingCamera.Initialize();
        _gameStatePresenter.Initialize();

        Application.targetFrameRate = 60;
        
        Bind();
        SetEvents();
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
        
        // Tracking State
        BodySourceManager.Instance.TrackedData
            .ObserveCountChanged()
            .Subscribe(_ => InGameView.Instance.UpdateTrackingStateView(BodySourceManager.Instance.TrackedData))
            .AddTo(this);

        // Debug
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.Space))
            .Subscribe(_ =>
            {
                var current = (int)_gameStatePresenter.CurrentGameState;
                current = Mathf.Min(current + 1, 2);
                _gameStatePresenter.SetState((GameState)current);
            })
            .AddTo(this);
    }

    private void SetEvents()
    {
        _gameStatePresenter.OnStateChanged[GameState.InGame] = () =>
        {
            InGameView.Instance.PlayTransition(() =>
            {
                _followingCamera.SetInGameCamera();
                InGameView.Instance.SetInGameView();
                _playerPresenter.SetMovable(true);
            }, () =>
            {
                InGameView.Instance.UpdateCountDown(0.3f, () =>
                {
                    if(_isSkip) _enemyGenerator.GenerateBoss();
                    else _enemyGenerator.GenerateLoopBeforeBoss().Forget();
                }).Forget();
            });
        };

        _gameStatePresenter.OnStateChanged[GameState.Result] = () =>
        {
            
        };
    }
}
