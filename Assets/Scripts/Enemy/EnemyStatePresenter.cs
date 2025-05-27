using System;
using System.Collections.Generic;
using UniRx;
using Enemy;
using UnityEngine;
using Utility;

public class EnemyStatePresenter : MonoBehaviour
{
    private StateModel<EnemyState> _model;
    
    public EnemyState CurrentState => _model.State.Value;

    public Dictionary<EnemyState, Action> OnStateChanged;

    public void Initialize()
    {
        _model = new StateModel<EnemyState>(EnemyState.Idle);

        OnStateChanged = new Dictionary<EnemyState, Action>()
        {
            { EnemyState.Idle , null},
            { EnemyState.Move, null },
            { EnemyState.AttackReady, null },
            { EnemyState.Attack, null },
            { EnemyState.AttackCoolTime, null },
            { EnemyState.Damage, null },
            { EnemyState.Dead, null }
        };

        _model.State
            .Subscribe(state => OnStateChanged[state]?.Invoke())
            .AddTo(this);
    }
    
    public void SetState(EnemyState newState)
    {
        if(CurrentState == EnemyState.Dead) return;
        _model.SetState(newState);
    }
}
