using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utility;

public class JumpCalibrationPresenter : Singleton<JumpCalibrationPresenter>
{
    [SerializeField] private JumpCalibrationView _view;
    private JumpCalibrationModel _model;
    public float CalibratePosition => _model.CalibratePosition;

    public override void Initialize()
    {
        _model = new JumpCalibrationModel();

        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.J))
            .Subscribe(_ => _model.SetCalibration())
            .AddTo(this);

        _model.OnCalibrated += () => _view.HideCalibrationText().Forget();
        
        base.Initialize();
    }

    public void Calibrate(float data)
    {
        if(!_model.IsCalibrating) return;
        _model.Calibrate(data);
        _view.ShowCalibrationText();
    }
}
