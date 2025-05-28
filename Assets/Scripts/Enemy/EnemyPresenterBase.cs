using System;
using UnityEngine;

public abstract class EnemyPresenterBase : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    [SerializeField] protected EnemyParams _params;
    [SerializeField] protected EnemyMovement _enemyMovement;
    [SerializeField] protected EnemyStatePresenter _enemyStatePresenter;
    [SerializeField] protected EnemyEffect _enemyEffect;
    [SerializeField] protected HitPointPresenter _hpPresenter;
    [SerializeField] protected HitPointView _hpView;
    [SerializeField] protected EnemyAttackerBase _attacker;
    [SerializeField] protected Collider _takeDamageCollider;

    public EnemyStatePresenter EnemyStatePresenter => _enemyStatePresenter;
    public EnemyAttackerBase Attacker => _attacker;
    public EnemyParams Params => _params;

    public Action OnDead;

    public virtual void Initialize(Transform viewParent)
    {
        // Initialize
        _enemyStatePresenter.Initialize();
        _enemyMovement.Initialize(_params, _enemyStatePresenter);
        _attacker?.Initialize(_params);
        _enemyEffect.Initialize();

        var hpView = Instantiate(_hpView, viewParent).GetComponent<HitPointView>();
        _hpPresenter.Initialize(_params, hpView);
        OnDead += () => Destroy(hpView.gameObject, 1.5f);
        
        // Bind
        Bind(hpView);
        
        // Set Events
        SetEvents();
    }

    protected virtual void Bind(HitPointView hpView)
    {
    }

    protected virtual void SetEvents()
    {
    }

    protected int Damage(AttackPoint attackPoint, float velocity)
    {
        var velRate = velocity / GameConst.MAX_PUNCH_VELOCITY;
        var damage = Mathf.Lerp(attackPoint.MinAttackPoint, attackPoint.MaxAttackPoint,
            velRate);

        return Mathf.RoundToInt(damage);
    }
}
