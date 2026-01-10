using System;
using UnityEngine;
using UniRx;

namespace InGame.Character
{
    public class HitPointModel
    {
        private int _currentHp;
        private readonly int _maxHp;
        private readonly ReactiveProperty<float> _hpRate = new(1f);
        private readonly Subject<Unit> _onHpDecreased = new();
        
        public int CurrentHp => _currentHp;
        public IReadOnlyReactiveProperty<float> HpRate => _hpRate;
        public IObservable<Unit> OnHpDecreased => _onHpDecreased;
        

        public HitPointModel(int maxHp)
        {
            _maxHp = maxHp;
            _currentHp = _maxHp;
        }

        public void SetHp(int currentHp)
        {
            if (_currentHp <= 0f) return;
                
            _currentHp = Mathf.Max(currentHp, 0); 
            _hpRate.Value = (float)_currentHp / _maxHp;

            if (_currentHp <= 0)
            {
                _onHpDecreased.OnNext(Unit.Default);
            }
        }

        public void Decrease(int value)
        {
            int decreasedHp = _currentHp - value;
            SetHp(decreasedHp);
        }
    }
}
