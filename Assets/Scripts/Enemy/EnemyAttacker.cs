using System;
using Cysharp.Threading.Tasks;
using Interface;
using UnityEngine;

public class EnemyAttacker : MonoBehaviour, IAttackable
{
    [SerializeField] private Collider _attackColllider;
    private EnemyParams _params;
    private AttackPoint _attackPoint;
    private AttackInfo _attackInfo;
    public AttackInfo AttackInfo => _attackInfo;

    public Action OnAttackReadied;
    public Action OnAttackCoolTimeStarted;
    public Action OnAttackCoolTimeEnded;

    public void Initialize(EnemyParams enemyParams)
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

    public void Attack(float waitTime)
    {
        AttackImpact(waitTime).Forget();    
    }
    
    private async UniTaskVoid AttackImpact(float waitTime)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        
        _attackColllider.enabled = true;
        await UniTask.Delay(TimeSpan.FromSeconds(GameConst.COLLIDER_ACTIVE_TIME));
        _attackColllider.enabled = false;
        
        AttackCoolTime(_params.AttackCoolTime).Forget();
    }

    public async UniTaskVoid AttackReady()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_params.AttackReadyTime));
        OnAttackReadied?.Invoke();
    }

    private async UniTaskVoid AttackCoolTime(float coolTime)
    {
        OnAttackCoolTimeStarted?.Invoke();
        await UniTask.Delay(TimeSpan.FromSeconds(coolTime));
        OnAttackCoolTimeEnded?.Invoke();
    }
    
    public void AttackEnd()
    {
        throw new System.NotImplementedException();
    }

    public void Attack(Vector3 dir, float velocity)
    {
    }
}
