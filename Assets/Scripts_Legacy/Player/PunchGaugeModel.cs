using UniRx;
using UnityEngine;

namespace Player
{
    public class PunchGaugeModel
    {
        private ReactiveProperty<int> _punchPoint;
        public IReactiveProperty<int> PunchPoint => _punchPoint;

        public PunchGaugeModel(int initValue = 0)
        {
            _punchPoint = new ReactiveProperty<int>(initValue);
        }

        public void Add(int value)
        {
            _punchPoint.Value += value;
        }

        public void Reset()
        {
            _punchPoint.Value = 0;
        }
    }
}
