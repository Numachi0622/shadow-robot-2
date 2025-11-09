using System;
using UniRx;

namespace InGame.Character
{
    public abstract class AttackObserver<T>
    {
        protected readonly Subject<T> _onAttackStart = new();
        protected readonly Subject<Unit> _onAttackEnd = new();
        
        public IObservable<T> OnAttackStart => _onAttackStart;
        public IObservable<Unit> OnAttackEnd => _onAttackEnd;

        public virtual void Observe()
        {
        }
    }
}