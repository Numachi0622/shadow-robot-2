using System;
using Cysharp.Threading.Tasks;
using Interface;
using UnityEngine;

public class EnemyAttacker : MonoBehaviour, IAttackable
{
    [SerializeField] private Collider _attackColllider;
    private EnemyParams _enemyParams;
    private AttackPoint _attackPoint;
    private AttackInfo _attackInfo;
    public AttackInfo AttackInfo => _attackInfo;

    public Action OnAttackReadied;

    public void Initialize(EnemyParams enemyParams)
    {
        _enemyParams = enemyParams;
        _attackPoint = enemyParams.AttackPoint;
        _attackInfo = new AttackInfo()
        {
            AttackPoint = _attackPoint,
            AttackDirection = Vector3.zero,
            AttackVelocity = 0f,
            AttackType = AttackType.EnemyToPlayerNormal,
        };
    }
    
    public void Attack()
    {
        
    }

    private async UniTaskVoid AttackReady(float waitTime)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_enemyParams.AttackReadyTime));
        OnAttackReadied?.Invoke();
        
        
    }
    
    public void AttackEnd()
    {
        throw new System.NotImplementedException();
    }

    public void Attack(Vector3 dir, float velocity)
    {
    }
}
