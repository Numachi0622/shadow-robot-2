using System;
using Cysharp.Threading.Tasks;
using Interface;
using UnityEngine;

public abstract class EnemyAttackerBase : MonoBehaviour, IAttackable
{
    [SerializeField] protected Collider _attackCollider;
    protected EnemyParams _params;
    protected AttackPoint _attackPoint;
    protected AttackInfo _attackInfo;
    public AttackInfo AttackInfo => _attackInfo;

    public Action OnAttackReadied;
    public Action OnAttackCoolTimeStarted;
    public Action OnAttackCoolTimeEnded;

    public virtual void Initialize(EnemyParams enemyParams)
    {
        _params = enemyParams;
        _attackPoint = _params.AttackPoint;
        _attackInfo = new AttackInfo()
        {
            AttackPoint = _attackPoint,
            AttackDirection = Vector3.zero,
            AttackVelocity = 0f,
            AttackType = AttackType.EnemyToPlayerNormal,
        };
    }

    protected virtual async UniTask AttackImpact(float waitTime)
    {
    }
    
    public virtual async UniTask AttackReady()
    {
    }
    
    public virtual async UniTask AttackReady(int attackIndex)
    {
    }
    
    protected virtual async UniTaskVoid AttackCoolTime(float coolTime)
    {
        OnAttackCoolTimeStarted?.Invoke();
        await UniTask.Delay(TimeSpan.FromSeconds(coolTime));
        OnAttackCoolTimeEnded?.Invoke();
    }
    
    public virtual void Attack(float waitTime = 0)
    {
    }

    public virtual void Attack(Vector3 dir, float velocity)
    {
    }

    public virtual void AttackEnd()
    {
    }
}
