using System;
using Cysharp.Threading.Tasks;
using Enemy;
using Interface;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class EnemyPresenter : EnemyPresenterBase
{
    private readonly int IS_MOVE = Animator.StringToHash("IsMove");
    private readonly int ATTACK_READY = Animator.StringToHash("AttackReady");
    private readonly int ATTACK = Animator.StringToHash("Attack");
    private readonly int DAMAGE = Animator.StringToHash("Damage");
    private readonly int DEAD = Animator.StringToHash("Dead");

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
                var targetDir = (transform.position - targetTransform.position).normalized;
                var direction = ((targetDir + Vector3.forward) * 0.5f).normalized;
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
            _animator.SetTrigger(ATTACK_READY);
            _attacker.AttackReady().Forget();
            _enemyEffect.ShakeBody(3);
            _enemyEffect.BlinkColor(_params.AttackReadyColor, 4);
        };
        _enemyStatePresenter.OnStateChanged[EnemyState.Attack] = () =>
        {
            _animator.ResetTrigger(ATTACK_READY);
            _animator.SetTrigger(ATTACK);
        };
        _enemyStatePresenter.OnStateChanged[EnemyState.Damage] = () =>
        {
            _takeDamageCollider.enabled = false;
            _animator.SetTrigger(DAMAGE);
        };
        _enemyStatePresenter.OnStateChanged[EnemyState.Dead] = () =>
        {
            _takeDamageCollider.enabled = false;
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
}
