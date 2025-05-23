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

                    _resetDisposable = Observable.Timer(TimeSpan.FromSeconds(3f))
                        .Where(_ => punchPoint > 0)
                        .Subscribe(_ => _model.Reset())
                        .AddTo(this);
                })
                .AddTo(this);
        
            // debug
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.P))
                .Subscribe(_ => _model.Add(10))
                .AddTo(this);
        }
    }
}
