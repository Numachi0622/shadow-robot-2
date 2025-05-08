using System;
using Enemy;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class EnemyMovement : MonoBehaviour
{
    public Action OnStartMovement;
    public Action OnStopNearTarget;
    public void Initialize(EnemyParams enemyParams, EnemyStatePresenter statePresenter)
    {
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
        transform.LookAt(targetPos);
        
        var currentPos = transform.position;
        var dist = Vector3.Distance(targetPos, currentPos);
        if (dist <= enemyParams.AttackRange)
        {
            OnStopNearTarget?.Invoke();
            return;
        }
        
        transform.position = Vector3.MoveTowards(currentPos, targetPos, enemyParams.MoveSpeed * Time.deltaTime); 
    }
}
