using Cysharp.Threading.Tasks;
using Interface;
using UnityEngine;
using TNRD;
using UniRx.Triggers;
using UniRx;
using UnityEngine.SceneManagement;
using Utility;

public class InGamePresenter : MonoBehaviour
{
    [SerializeField] private PlayerPresenter _playerPresenter;
    [SerializeField] private ColorBodySourceView _colorBodySourceView;
    [SerializeField] private EnemyGenerator _enemyGenerator;
    [SerializeField] private TrackingDebugView _debugView;
    [SerializeField] private FollowingCamera _followingCamera;
    
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
                InGameView.Instance.PlayTransition(() =>
                {
                    var current = (int)GameStatePresenter.Instance.CurrentGameState;
                    current = Mathf.Min(current + 1, 2);
                    GameStatePresenter.Instance.SetState((GameState)current);
                }, () => 
                {
                    InGameView.Instance.UpdateCountDown(0.3f, () =>
                    {
                        if(_isSkip) _enemyGenerator.GenerateBoss();
                        else _enemyGenerator.GenerateLoopBeforeBoss().Forget();
                    }).Forget();
                });

            })
            .AddTo(this);
        
        // Restart
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.R))
            .Subscribe(_ => SceneManager.LoadScene(0))
            .AddTo(this);
    }

    private void SetEvents()
    {
        GameStatePresenter.Instance.OnStateChanged[GameState.InGame] = () =>
        {
            _followingCamera.SetInGameCamera();
            InGameView.Instance.SetInGameView();
        };

        GameStatePresenter.Instance.OnStateChanged[GameState.Clear] = () => InGameView.Instance.ShowResultView(true, 1.2f);
        
        GameStatePresenter.Instance.OnStateChanged[GameState.GameOver] = () => InGameView.Instance.ShowResultView(false);
    }
}
