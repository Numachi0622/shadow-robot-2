using System;
using UniRx;

namespace InGame.Character
{
    public class DamageObserver
    {
        private readonly Subject<AttackParam> _onDamageSubject;

        public IObservable<AttackParam> OnTakeDamage => _onDamageSubject;

        public void Observe(AttackParam attackParam)
        {
            _onDamageSubject.OnNext(attackParam);
        }
    }
}