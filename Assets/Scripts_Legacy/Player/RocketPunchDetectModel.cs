using System;
using InGame.Character;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class RocketPunchDetectModel
{
    private Transform _leftArm, _leftHand;
    private Transform _rightArm, _rightHand;
    private Transform _bodyTransform;
    private float _threshold = 0.8f;
    private float _maxReadyTime = 1f; 
    private float _readyTime = 0f;
    private bool _isReady = false;
    private Vector3 _handDirection;
    public Vector3 HandDirection => _handDirection;
    
    private readonly Subject<Unit> _onStartReadyRocketPunch = new Subject<Unit>();
    public IObservable<Unit> OnStartReadyRocketPunch => _onStartReadyRocketPunch;
    private readonly Subject<Vector3> _onReadyRocketPunch = new Subject<Vector3>();
    public IObservable<Vector3> OnReadyRocketPunch => _onReadyRocketPunch;
    
    private readonly Subject<Unit> _onResetRocketPunch = new Subject<Unit>();
    public IObservable<Unit> OnResetRocketPunch => _onResetRocketPunch;

    public RocketPunchDetectModel(PlayerParams param, Transform leftArm, Transform leftHand, Transform rightArm, Transform rightHand, Transform body)
    {
        _leftArm = leftArm;
        _leftHand = leftHand;
        _rightArm = rightArm;
        _rightHand = rightHand;
        _bodyTransform = body;
        _threshold = param.RocketPunchDirThreshold;
        _maxReadyTime = param.MaxRocketPunchReadyTime;
        
        _bodyTransform.UpdateAsObservable()
            .Where(_ => GameStatePresenter.Instance.CurrentGameState == GameState.InGame)
            .Select(_ =>
            {
                var leftHandDir = ((-_leftArm.right - _leftHand.right) * 0.5f).normalized;
                var dotLeft = Vector3.Dot(leftHandDir, _bodyTransform.forward);

                var rightHandDir = ((-_rightArm.right - _rightHand.right) * 0.5f).normalized;
                var dotRight = Vector3.Dot(rightHandDir, _bodyTransform.forward);
                
                var dirAverage = (leftHandDir + rightHandDir).normalized;
                _handDirection = new Vector3(dirAverage.x, 0f, dirAverage.z).normalized;
                
                var dist = Vector3.Distance(_leftHand.position, _rightHand.position);

                var isDetect = (dotLeft > _threshold || dotRight > _threshold) && dist < param.RocketPunchDistThreshold;
                
                (bool IsDetect, Vector3 Dir) info  = (isDetect, dirAverage);
                return info;
            })
            .Subscribe(info =>
            {
                if (!info.IsDetect)
                {
                    _readyTime = 0f;
                    if (_isReady)
                    {
                        Cancel();
                        _isReady = false;
                    }
                    return;
                }
                
                _readyTime += Time.deltaTime;
                if (_readyTime > _maxReadyTime)
                {
                    if (!_isReady)
                    {
                        _onStartReadyRocketPunch.OnNext(Unit.Default);
                        _isReady = true;   
                    }
                    _onReadyRocketPunch.OnNext(info.Dir);
                }
            })
            .AddTo(_bodyTransform);
    }

    public void Cancel()
    {
        _onResetRocketPunch.OnNext(Unit.Default);
    }
}