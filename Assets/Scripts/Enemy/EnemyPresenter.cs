using Enemy;
using Interface;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class EnemyPresenter : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private EnemyParams _params;
    [SerializeField] private EnemyMovement _enemyMovement;
    [SerializeField] private EnemyStatePresenter _enemyStatePresenter;
    [SerializeField] private HitPointPresenter _hpPresenter;
    [SerializeField] private HitPointView _hPView;
    [SerializeField] private Attacker _attacker;
    
    // TODO Generator側に後で移動
    [SerializeField] private Transform _enemyHpParent;
    
    
    private readonly int IS_MOVE = Animator.StringToHash("IsMove");
    private readonly int ATTACK = Animator.StringToHash("Attack");
    private readonly int DAMAGE = Animator.StringToHash("Damage");
    private readonly int DEAD = Animator.StringToHash("Dead");

    public void Initialize()
    {
        // Initialize
        _enemyStatePresenter.Initialize();
        _enemyMovement.Initialize(_params, _enemyStatePresenter);
        _attacker.Initialize(_params);

        var hPView = Instantiate(_hPView, _enemyHpParent).GetComponent<HitPointView>();
        _hpPresenter.Initialize(_params, hPView);
        
        // Bind
        Bind(hPView);
        
        // Set Events
        SetEvents();
    }

    private void Bind(HitPointView hpView)
    {
        // Take Damage
        this.UpdateAsObservable()
            .Select(hitCollider =>
            {
                if (TryGetComponent<IAttackable>(out var attacker))
                {
                    return attacker;
                }
                
                return null;
            })
            .Where(attacker => attacker != null)
            .Select(attacker => attacker.AttackPoint)
            .Subscribe(damage =>
            {
                _hpPresenter.DecreaseHp(damage.RandomValue);
                _enemyStatePresenter.SetState(EnemyState.Damage);
            })
            .AddTo(this);
        
        // Follow HP Gauge
        this.UpdateAsObservable()
            .Where(_ => hpView != null)
            .Subscribe(_ =>
            {
                hpView.transform.position =
                    RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position + Vector3.up * 2f);
            })
            .AddTo(this);
        
        // Debug
        // debug
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.A))
            .Subscribe(_ =>
            {
                _hpPresenter.DecreaseHp(10);
                _enemyStatePresenter.SetState(EnemyState.Damage);
            })
            .AddTo(this);
    }

    private void SetEvents()
    {
        // State
        _enemyStatePresenter.OnMoveState = () => _animator.SetBool(IS_MOVE, true);
        _enemyStatePresenter.OnAttackReadyState = () =>
        {
            _animator.SetBool(IS_MOVE, false);
            _animator.SetTrigger(ATTACK);
        };
        _enemyStatePresenter.OnHitDamage = () => _animator.SetTrigger(DAMAGE);
        _enemyStatePresenter.OnDead = () => _animator.SetTrigger(DEAD);
        
        // Movement Stop
        _enemyMovement.OnStopNearTarget = () => _enemyStatePresenter.SetState(EnemyState.AttackReady);
        
        // Damage
        _hpPresenter.OnHpDecreased(() => _enemyStatePresenter.SetState(EnemyState.Damage));
        
        // Dead
        _hpPresenter.OnHpDeleted(() => _enemyStatePresenter.SetState(EnemyState.Dead));
    }
}
