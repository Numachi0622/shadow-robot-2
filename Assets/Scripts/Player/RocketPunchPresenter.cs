using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class RocketPunchPresenter : MonoBehaviour
{
    [SerializeField] private Transform _leftArm, _leftHand;
    [SerializeField] private Transform _rightArm, _rightHand;
    [SerializeField] private Transform _body;
    [SerializeField] private Transform _leftFoot, _rightFoot;
    [SerializeField] private RocketPunchView _rocketPunchView;
    [SerializeField] private FollowingCamera _followingCamera;
    private RocketPunchDetectModel _detectModel;
    private RocketPunchChargeModel _chargeModel;
    private RocketPunchChargeDetectModel _chargeDetectModel;
    
    private bool _isCharging = false;
    public bool IsCharging => _isCharging;
    
    public void Initialize(PlayerParams param)
    {
        _detectModel = new RocketPunchDetectModel(param, _leftArm, _leftHand, _rightArm, _rightHand, _body);
        _chargeModel = new RocketPunchChargeModel();
        _chargeDetectModel = new RocketPunchChargeDetectModel(param, _leftFoot, _rightFoot);
        _rocketPunchView.Initialize();
        
        Bind();
    }

    private void Bind()
    {
        // Detect Rocket Punch
        _detectModel.OnStartReadyRocketPunch
            .Subscribe(_ =>
            {
                _followingCamera.SetRocketPunchCamera();
                _rocketPunchView.ShowView();
                _isCharging = true;
            })
            .AddTo(this);
        
        _detectModel.OnReadyRocketPunch
            .Subscribe(dir => _rocketPunchView.UpdateTarget(dir))
            .AddTo(this);
        
        _detectModel.OnResetRocketPunch
            .Subscribe(_ =>
            {
                _followingCamera.CancelRocketPunchCamera();
                _rocketPunchView.HideView();
                _chargeModel.Reset();
                _isCharging = false;
            })
            .AddTo(this);
        
        // Charge Rocket Punch
        this.UpdateAsObservable()
            .Where(_ => GameStatePresenter.Instance.CurrentGameState == GameState.InGame)
            .Where(_ => _chargeModel.CurrentValue is > 0 and < GameConst.MAX_ROCKET_PUNCH_CHARGE && !_rocketPunchView.IsCharging)
            .Subscribe(_ => _chargeModel.Decrease(10 * Time.deltaTime))
            .AddTo(this);

        _chargeDetectModel.OnUppedFoot
            .Where(_ => GameStatePresenter.Instance.CurrentGameState == GameState.InGame)
            .Subscribe(_ => _chargeModel.Increase(20))
            .AddTo(this);

        _chargeModel.ChargeValue
            .Where(_ => GameStatePresenter.Instance.CurrentGameState == GameState.InGame)
            .Subscribe(value => _rocketPunchView.UpdateChargeGauge(value))
            .AddTo(this);
    }
    
    public void CancelRocketPunch()
    {
        _detectModel.Cancel();
    }
}
