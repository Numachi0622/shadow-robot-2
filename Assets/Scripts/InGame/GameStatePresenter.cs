using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameStatePresenter : MonoBehaviour
{
    private StateModel<GameState> _model;
    public GameState CurrentGameState => _model.State.Value;

    public Dictionary<GameState, Action> OnStateChanged;

    public void Initialize()
    {
        _model = new StateModel<GameState>(GameState.Select);
        OnStateChanged = new Dictionary<GameState, Action>()
        {
            { GameState.InGame , null },
            { GameState.Result , null },
        };

        _model.State
            .Subscribe(SetState)
            .AddTo(this);
    }

    public void SetState(GameState state)
    {
        if(state == GameState.Select) return;
        _model.SetState(state);
        OnStateChanged[state]?.Invoke();
    }
}
