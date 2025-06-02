using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Utility;

public class RocketPunchPresenter : MonoBehaviour
{
    [SerializeField] private Transform _leftArm, _leftHand;
    [SerializeField] private Transform _rightArm, _rightHand;
    [SerializeField] private Transform _body;
    [SerializeField] private Transform _leftFoot, _rightFoot;
    [SerializeField] private RocketPunchView _rocketPunchView;
    [SerializeField] private FollowingCamera _followingCamera;
    [SerializeField] private RocketPunchLauncher _launcher;
    [SerializeField] private SkinnedMeshRenderer _armMesh;
    [SerializeField] private MeshRenderer _leftBallMesh, _rightBallMesh;
    [SerializeField] private ParticleSystem[] _launchEffects;
    [SerializeField] private float _increaseChargeValue = 30f;
    private RocketPunchDetectModel _detectModel;
    private RocketPunchChargeModel _chargeModel;
    private RocketPunchChargeDetectModel _chargeDetectModel;
    
    private bool _isCharging = false;
    public bool IsCharging => _isCharging;

    private bool _isLaunching = false;
    public bool IsLaunching => _isLaunching;
    
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
            .Where(_ => !_isLaunching)
            .Subscribe(_ =>
            {
                _followingCamera.SetRocketPunchCamera();
                _rocketPunchView.ShowView();
                _isCharging = true;
            })
            .AddTo(this);
        
        _detectModel.OnReadyRocketPunch
            .Where(_ => !_isLaunching)
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
            .Subscribe(_ => _chargeModel.Increase(_increaseChargeValue))
            .AddTo(this);

        _chargeModel.ChargeValue
            .Where(_ => GameStatePresenter.Instance.CurrentGameState == GameState.InGame)
            .Where(_ => _isCharging)
            .Subscribe(value =>
            {
                _rocketPunchView.UpdateChargeGauge(value);
                SoundManager.Instance.PlaySe(13);
                if (value >= GameConst.MAX_ROCKET_PUNCH_CHARGE)
                {
                    Launch(_detectModel.HandDirection).Forget();
                }
            })
            .AddTo(this);
    }

    private async UniTask Launch(Vector3 dir)
    {
        _rocketPunchView.ShowLaunchMsg();
        await UniTask.Delay(TimeSpan.FromSeconds(0.7f));
        
        CancelRocketPunch();
        _launcher.Launch(dir, 
            () =>
            {
                _armMesh.enabled = false;
                _leftBallMesh.enabled = false;
                _rightBallMesh.enabled = false;
                _isLaunching = true;
                foreach (var eff in _launchEffects) eff.Play();
                SoundManager.Instance.PlaySe(10);
            },
            () =>
            {
                _armMesh.enabled = true;
                _leftBallMesh.enabled = true;
                _rightBallMesh.enabled = true;
                _isLaunching = false;
                foreach (var eff in _launchEffects) eff.Play();
                SoundManager.Instance.PlaySe(12);
            });
        }
    
    public void CancelRocketPunch()
    {
        _detectModel.Cancel();
    }
}
