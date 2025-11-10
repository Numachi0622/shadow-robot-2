using System;
using UnityEngine;
using UniRx;

namespace InGame.Character
{
    public class HitPointModel
    {
        private readonly ReactiveProperty<int> _hp;
        private readonly Subject<Unit> _onHpDecreased = new();
        public IReadOnlyReactiveProperty<int> Hp => _hp;
        public IObservable<Unit> OnHpDecreased => _onHpDecreased;
        

        public HitPointModel(int mapHp)
        {
            _hp = new ReactiveProperty<int>(mapHp);
        }

        public void SetHp(int currentHp)
        {
            if (_hp.Value == 0f) return;

            _hp.Value = Mathf.Max(currentHp, 0);

            if (_hp.Value <= 0)
            {
                _onHpDecreased.OnNext(Unit.Default);
            }
        }

        public void Decrease(int value)
        {
            int decreasedHp = _hp.Value - value;
            SetHp(decreasedHp);
        }
    }
}
