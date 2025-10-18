using System;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

public class GameStatePresenter : Utility.Singleton<GameStatePresenter>
{
    private StateModel<GameState> _model;
    public GameState CurrentGameState => _model.State.Value;

    public Dictionary<GameState, Action> OnStateChanged;

    public override void Initialize()
    {
        base.Initialize();
        
        _model = new StateModel<GameState>(GameState.Select);
        OnStateChanged = new Dictionary<GameState, Action>()
        {
            { GameState.InGame , null },
            { GameState.GameOver , null },
            { GameState.Clear , null}
        };

        _model.State
            .Where(state => state != GameState.Select)
            .Subscribe(state => OnStateChanged[state]?.Invoke())
            .AddTo(this);
    }

    public void SetState(GameState state)
    {
        if(state == GameState.Select) return;
        _model.SetState(state);
        OnStateChanged[state]?.Invoke();
    }
}
