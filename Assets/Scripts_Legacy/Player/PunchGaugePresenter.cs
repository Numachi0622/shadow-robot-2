using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Player
{
    public class PunchGaugePresenter : MonoBehaviour
    {
        [SerializeField] private PunchGaugeView _view;
        private PunchGaugeModel _model;
        private IDisposable _resetDisposable;

        public void Initialize()
        {
            _view.Initialize();
            _model = new PunchGaugeModel();
        
            Bind();
        }

        private void Bind()
        {
            _model.PunchPoint
                .Skip(1)
                .Subscribe(punchPoint =>
                {
                    _view.UpdateGauge(punchPoint);
                
                    _resetDisposable?.Dispose();

                    _resetDisposable = Observable.Timer(TimeSpan.FromSeconds(GameConst.PUNCH_POINT_RESET_TIME))
                        .Where(_ => punchPoint > 0)
                        .Subscribe(_ => _model.Reset())
                        .AddTo(this);
                })
                .AddTo(this);
        }
        
        public void AddPunchPoint(int value)
        {
            _model.Add(value);
        }
    }
}
