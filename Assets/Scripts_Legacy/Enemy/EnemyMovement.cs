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
                if (GameStatePresenter.Instance.CurrentGameState == GameState.GameOver)
                {
                    statePresenter.SetState(EnemyState.Idle);
                    return;
                }
                statePresenter.SetState(EnemyState.Move);
                var targetPos = target.position;
                targetPos.y = 0;
                Move(targetPos, enemyParams);
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

    public void KnockBack(Vector3 dir, bool isDead = false)
    {
        _knockBackSequence?.Kill();

        var mag = isDead ? 8f : 5f;
        var destination = transform.position + dir * mag;
        destination.y = 0f;
        
        _knockBackSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .Append(_enemyTransform.DOMove(destination, 0.5f).SetEase(Ease.OutCubic));
    }
}
