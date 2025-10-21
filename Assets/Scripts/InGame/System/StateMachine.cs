using System;
using System.Collections.Generic;

namespace InGame.System
{
    public interface IStateParameter
    {
    }
    
    public class StateMachine<T>
    {
        public abstract class State
        {
            private StateMachine<T> _stateMachine;
            
            public T Owner => _stateMachine.Owner;

            protected State()
            {
            }
            
            internal void Initialize(StateMachine<T> stateMachine)
            {
                _stateMachine = stateMachine;
            }
            
            public virtual void OnEnter(IStateParameter parameter = null)
            {
            }

            public virtual void OnUpdate()
            {
            }

            public virtual void OnExit()
            {
            }
        }
        
        private readonly T _owner;
        private readonly Dictionary<Type, State> _states = new Dictionary<Type, State>();
        private State _currentState;

        public T Owner => _owner;

        public StateMachine(T owner)
        {
            _owner = owner;
        }

        public void SetState<TTo>(IStateParameter parameter = null) where TTo : State, new()
        {
            if (_states.TryGetValue(typeof(TTo), out var state))
            {
                _currentState?.OnExit();
                _currentState = state;
                _currentState.OnEnter(parameter);
                return;
            }

            _currentState?.OnExit();
            TTo newState = new TTo();
            newState.Initialize(this);
            _currentState = newState;
            _states.Add(typeof(TTo), newState);
            _currentState.OnEnter(parameter);
        }

        public void OnUpdate()
        {
            _currentState.OnUpdate();
        }
    }
}
