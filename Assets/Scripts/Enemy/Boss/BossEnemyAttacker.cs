using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Enemy.Boss;
using Interface;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossEnemyAttacker : EnemyAttackerBase
{
    [SerializeField] private EnergyBallAttacker _energyBallAttacker;
    [SerializeField] private FirePillarAttacker _firePillarAttacker;
    [SerializeField] private float _backDistance = 7f;
    [SerializeField] private float _backDuration = 0.5f;
    [SerializeField] private int _firePillarCount = 15;
    [SerializeField] private float _firePillarRange = 10f;
    private int _attackIndex;
    private Sequence _backSequence;

    public override void Attack(float waitTime = 0)
    {
        switch (_attackIndex)
        {
            case 0:
                AttackImpact(waitTime).Forget();
                break;
            case 1:
                EnergyBallAttack(waitTime).Forget();
                break;
            case 2:
                FirePillarAttack(waitTime).Forget();
                break;
        }
    }

    protected override async UniTask AttackImpact(float waitTime)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime));

        _attackCollider.enabled = true;
        await UniTask.Delay(TimeSpan.FromSeconds(GameConst.COLLIDER_ACTIVE_TIME));
        _attackCollider.enabled = false;
        
        AttackCoolTime(_params.AttackCoolTime).Forget();
    }
    
    public override async UniTask AttackReady(int attackIndex)
    {
        _attackIndex = attackIndex;

        if (_attackIndex == 0)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_params.AttackReadyTime));
            OnAttackReadied?.Invoke();
            return;
        }
        
        _backSequence?.Kill();
        _backSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Join(transform.parent.DOMove(-transform.parent.forward * _backDistance, _backDuration).SetEase(Ease.Linear).SetRelative(true))
            .AppendInterval(_params.AttackReadyTime)
            .AppendCallback(() => OnAttackReadied?.Invoke());
    }

    private async UniTask EnergyBallAttack(float waitTime)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        
        for(var angle = -90f; angle <= 90f; angle += 10f)
        {
            var rotation = Quaternion.Euler(0, angle, 0);
            var dir = rotation * transform.parent.forward;
            var energyBall = Instantiate(_energyBallAttacker, transform.position + Vector3.up, Quaternion.identity);
            energyBall.Initialize(_params);
            energyBall.Execute(dir);
        }
        
        AttackCoolTime(_params.AttackCoolTime).Forget();
    }
    
    private async UniTask FirePillarAttack(float waitTime)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        
        AttackCoolTime(_params.AttackCoolTime).Forget();
        
        for(var i = 0; i < _firePillarCount; i++)
        {
            var pos = transform.position + Random.insideUnitSphere * _firePillarRange;
            pos.y = 0;
            var firePillar = Instantiate(_firePillarAttacker, pos, Quaternion.identity);
            firePillar.Initialize(_params);
            firePillar.Execute();
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        }
    }
}
