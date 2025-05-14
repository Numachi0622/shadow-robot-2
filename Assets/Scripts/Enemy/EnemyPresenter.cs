using System;
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
    [SerializeField] private EnemyEffect _enemyEffect;
    [SerializeField] private HitPointPresenter _hpPresenter;
    [SerializeField] private HitPointView _hpView;
    [SerializeField] private EnemyAttacker _attacker;
    [SerializeField] private Collider _takeDamageCollider;
    
    // TODO Generator側に後で移動
    [SerializeField] private Transform _enemyHpParent;
    
    private readonly int IS_MOVE = Animator.StringToHash("IsMove");
    private readonly int ATTACK_READY = Animator.StringToHash("AttackReady");
    private readonly int ATTACK = Animator.StringToHash("Attack");
    private readonly int DAMAGE = Animator.StringToHash("Damage");
    private readonly int DEAD = Animator.StringToHash("Dead");
    
    public EnemyStatePresenter EnemyStatePresenter => _enemyStatePresenter;

    public Action OnDead;


    public void Initialize()
    {
        // Initialize
        _enemyStatePresenter.Initialize();
        _enemyMovement.Initialize(_params, _enemyStatePresenter);
        _attacker.Initialize(_params);
        _enemyEffect.Initialize();

        var hpView = Instantiate(_hpView, _enemyHpParent).GetComponent<HitPointView>();
        _hpPresenter.Initialize(_params, hpView);
        OnDead = () => Destroy(hpView.gameObject, 1.5f);
        
        // Bind
        Bind(hpView);
        
        // Set Events
        SetEvents();
    }

    private void Bind(HitPointView hpView)
    {
        // Take Damage
        this.OnTriggerEnterAsObservable()
            .Select(hitCollider =>
            {
                if (hitCollider.TryGetComponent<IAttackable>(out var attacker))
                {
                    return (attacker, hitCollider);
                }
                
                return (null, null);
            })
            .Where(attacker => attacker.attacker != null)
            .Select(attacker =>
            {
                var damageInfo = attacker.attacker.AttackInfo;
                var targetTransform = attacker.hitCollider.transform;
                var hitPos = _takeDamageCollider.ClosestPointOnBounds(targetTransform.position);
                var direction = (transform.position - targetTransform.position).normalized;
                damageInfo.AttackDirection = direction;

                return (damageInfo, hitPos);
            })
            .Subscribe(info =>
            {
                var damageInfo = info.damageInfo;
                var damage = Damage(damageInfo.AttackPoint, damageInfo.AttackVelocity);
                _hpPresenter.DecreaseHp(damage);

                var dir = damageInfo.AttackDirection;
                _enemyMovement.KnockBack(dir);
                
                _enemyEffect.BlinkColor(_params.DamagedColor);
                _enemyEffect.ShakeBody();
                DamageTextView.Instance.Play(damageInfo.AttackType, damage, info.hitPos);
                HitEffectManager.Instance.Play(damageInfo.AttackType, info.hitPos);
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
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.A))
            .Subscribe(_ =>
            {
                _hpPresenter.DecreaseHp(10);
            })
            .AddTo(this);
    }

    private void SetEvents()
    {
        // State
        _enemyStatePresenter.OnStateChanged[EnemyState.Idle] = () => _takeDamageCollider.enabled = true;
        _enemyStatePresenter.OnStateChanged[EnemyState.Move] = () => _animator.SetBool(IS_MOVE, true);
        _enemyStatePresenter.OnStateChanged[EnemyState.AttackReady] = () =>
        {
            _animator.SetBool(IS_MOVE, false);
            _animator.SetTrigger(ATTACK_READY);
        };
        _enemyStatePresenter.OnStateChanged[EnemyState.Attack] = () =>
        {
            _animator.ResetTrigger(ATTACK_READY);
            _animator.ResetTrigger(ATTACK);
        };
        _enemyStatePresenter.OnStateChanged[EnemyState.Damage] = () =>
        {
            _takeDamageCollider.enabled = false;
            _animator.SetTrigger(DAMAGE);
        };
        _enemyStatePresenter.OnStateChanged[EnemyState.Dead] = () =>
        {
            _animator.SetTrigger(DEAD);
            OnDead?.Invoke();
            Destroy(gameObject, 2f);
        };
        
        // Movement Stop
        _enemyMovement.OnStopNearTarget = () => _enemyStatePresenter.SetState(EnemyState.AttackReady);
        
        // Attack Readied
        _attacker.OnAttackReadied = () => _enemyStatePresenter.SetState(EnemyState.Attack);
        
        // Damage
        _hpPresenter.OnHpDecreased(() => _enemyStatePresenter.SetState(EnemyState.Damage));
        
        // Dead
        _hpPresenter.OnHpDeleted(() => _enemyStatePresenter.SetState(EnemyState.Dead));
    }
    
    
    // TODO: あとでDamageModelつくる
    private int Damage(AttackPoint attackPoint, float velocity)
    {
        const float maxVel = 20f;

        var velRate = velocity / maxVel;
        var damage = Mathf.Lerp(attackPoint.MinAttackPoint, attackPoint.MaxAttackPoint,
            velRate);

        return Mathf.RoundToInt(damage);
    }
}
