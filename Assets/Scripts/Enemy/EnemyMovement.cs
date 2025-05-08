using System;
using DG.Tweening;
using Enemy;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class EnemyMovement : MonoBehaviour
{
    public Action OnStartMovement;
    public Action OnStopNearTarget;

    private Transform _enemyTransform;
    private Sequence _knockBackSequence;
    
    public void Initialize(EnemyParams enemyParams, EnemyStatePresenter statePresenter)
    {
        _enemyTransform = transform;
        var target = GameObject.FindGameObjectWithTag("Player").transform;
        OnStartMovement?.Invoke();
        
        this.UpdateAsObservable()
            .Where(_ => statePresenter.CurrentState == EnemyState.Idle || statePresenter.CurrentState == EnemyState.Move)
            .Where(_ =>
            {
                var dist = Vector3.Distance(target.position, transform.position);
                return dist < enemyParams.SearchRange;
            })
            .Subscribe(dir =>
            {
                statePresenter.SetState(EnemyState.Move);
                Move(target.position, enemyParams);
            })
            .AddTo(this);
    }

    private void Move(Vector3 targetPos, EnemyParams enemyParams)
    {
        _enemyTransform.LookAt(targetPos);
        
        var currentPos = _enemyTransform.position;
        var dist = Vector3.Distance(targetPos, currentPos);
        if (dist <= enemyParams.AttackRange)
        {
            OnStopNearTarget?.Invoke();
            return;
        }
        
        _enemyTransform.position = Vector3.MoveTowards(currentPos, targetPos, enemyParams.MoveSpeed * Time.deltaTime); 
    }

    public void KnockBack(Vector3 dir)
    {
        _knockBackSequence?.Kill();

        var destination = transform.position + dir * 5f;
        destination.y = 0f;
        
        _knockBackSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(_enemyTransform.DOMove(destination, 0.5f).SetEase(Ease.OutCubic));
    }
}
