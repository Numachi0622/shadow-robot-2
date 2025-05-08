using System;
using UniRx;
using Enemy;
using UnityEngine;
using Utility;

public class EnemyStatePresenter : MonoBehaviour
{
    private StateModel<EnemyState> _model;
    
    public EnemyState CurrentState => _model.State.Value;
    
    public Action OnMoveState;
    public Action OnAttackReadyState;
    public Action OnHitDamage;
    public Action OnDead;

    public void Initialize()
    {
        _model = new StateModel<EnemyState>(EnemyState.Idle);

        _model.State
            .Subscribe(state =>
            {
                switch (state)
                {
                    case EnemyState.Move : 
                        OnMoveState?.Invoke();
                        break;
                    case EnemyState.AttackReady : 
                        OnAttackReadyState?.Invoke();
                        break;
                    case EnemyState.Damage : 
                        OnHitDamage?.Invoke();
                        break;
                    case EnemyState.Dead : 
                        OnDead?.Invoke();
                        break;
                }
            })
            .AddTo(this);
    }
    
    public void SetState(EnemyState newState)
    {
        _model.SetState(newState);
    }
}
