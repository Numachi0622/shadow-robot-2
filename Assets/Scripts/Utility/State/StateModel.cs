using System;
using UniRx;
using UnityEngine;

public class StateModel<T>
{
    private ReactiveProperty<T> _state;
    public IReactiveProperty<T> State => _state;
    
    public StateModel(T initialState)
    {
        _state = new ReactiveProperty<T>(initialState);
    }
    
    public void SetState(T newState)
    {
        if(_state.Value.Equals(newState)) return;
        //SDebug.Log($"{_state.Value} -> {newState}");
        _state.Value = newState;
    }
}
