using System.Security.Cryptography;
using Cysharp.Threading.Tasks;
using Enemy;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Interface;

public class BossEnemyPresenter : EnemyPresenterBase
{
    private int _lastAttackIndex;
    
    private readonly int IS_MOVE = Animator.StringToHash("IsMove");
    private readonly int DAMAGE = Animator.StringToHash("Damage");
    private readonly int DEAD = Animator.StringToHash("Dead");

    private readonly int[] ATTACK_READY = new[]
    {
        Animator.StringToHash("AttackReady01"),
        Animator.StringToHash("AttackReady02"),
        Animator.StringToHash("AttackReady03")
    };
    
    private readonly int[] ATTACK = new[]
    {
        Animator.StringToHash("Attack01"),
        Animator.StringToHash("Attack02"),
        Animator.StringToHash("Attack03")
    };

    public override void Initialize(Transform viewParent)
    {
        base.Initialize(viewParent);
    }

    protected override void Bind(HitPointView hpView)
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
    }

    protected override void SetEvents()
    {
        // State
        _enemyStatePresenter.OnStateChanged[EnemyState.Idle] = () => _takeDamageCollider.enabled = true;
        _enemyStatePresenter.OnStateChanged[EnemyState.Move] = () => _animator.SetBool(IS_MOVE, true);
        _enemyStatePresenter.OnStateChanged[EnemyState.AttackReady] = () =>
        {
            _animator.SetBool(IS_MOVE, false);
            _lastAttackIndex = GetRandomAttackIndex();
            _animator.SetTrigger(ATTACK_READY[_lastAttackIndex]);
            _attacker.AttackReady(_lastAttackIndex).Forget();
            _enemyEffect.ShakeBody(3);
            _enemyEffect.BlinkColor(_params.AttackReadyColor, 4);
        };
        _enemyStatePresenter.OnStateChanged[EnemyState.Attack] = () =>
        {
            _animator.ResetTrigger(ATTACK_READY[_lastAttackIndex]);
            _animator.SetTrigger(ATTACK[_lastAttackIndex]);
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
        
        // Start CoolTime
        _attacker.OnAttackCoolTimeStarted = () => _enemyStatePresenter.SetState(EnemyState.AttackCoolTime);
        
        // End CoolTime
        _attacker.OnAttackCoolTimeEnded = () => _enemyStatePresenter.SetState(EnemyState.Idle);
        
        // Damage
        _hpPresenter.OnHpDecreased(() => _enemyStatePresenter.SetState(EnemyState.Damage));
        
        // Dead
        _hpPresenter.OnHpDeleted(() => _enemyStatePresenter.SetState(EnemyState.Dead));
    }

    private int GetRandomAttackIndex()
    {
        var rand = Random.value;

        if (rand < 0.6f)
            return 0;
        else if (rand < 0.8f)
            return 1;
        else
            return 2;
    }
}
