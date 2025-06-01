using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class RocketPunchDetector : MonoBehaviour
{
    [SerializeField] private Transform _leftArm, _leftHand;
    [SerializeField] private Transform _rightArm, _rightHand;
    [SerializeField] private Transform _bodyTransform;
    [SerializeField, Range(0f, 1f)] private float _threshold = 0.8f;
    [SerializeField] private float _maxReadyTime = 1f; 
    private float _readyTime = 0f;
    private bool _isReady = false;
    
    private readonly Subject<Unit> _onReadyRocketPunch = new Subject<Unit>();
    public IObservable<Unit> OnReadyRocketPunch => _onReadyRocketPunch;
    
    private readonly Subject<Unit> _onResetRocketPunch = new Subject<Unit>();
    public IObservable<Unit> OnResetRocketPunch => _onResetRocketPunch;

    public void Initialize()
    {
        this.UpdateAsObservable()
            .Where(_ => GameStatePresenter.Instance.CurrentGameState == GameState.InGame)
            .Select(_ =>
            {
                var leftHandDir = ((-_leftArm.right - _leftHand.right) * 0.5f).normalized;
                var dotLeft = Vector3.Dot(leftHandDir, _bodyTransform.forward);

                var rightHandDir = ((-_rightArm.right - _rightHand.right) * 0.5f).normalized;
                var dotRight = Vector3.Dot(rightHandDir, _bodyTransform.forward);

                return dotLeft > _threshold && dotRight > _threshold;
            })
            .Subscribe(isReady =>
            {
                if (!isReady)
                {
                    _readyTime = 0f;
                    if (_isReady)
                    {
                        _onResetRocketPunch.OnNext(Unit.Default);
                        _isReady = false;
                    }
                    return;
                }
                
                _readyTime += Time.deltaTime;
                if (_readyTime > _maxReadyTime && !_isReady)
                {
                    _onReadyRocketPunch.OnNext(Unit.Default);
                    _isReady = true;
                }
            })
            .AddTo(this);
    }
}