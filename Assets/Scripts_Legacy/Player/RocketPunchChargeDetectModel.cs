using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class RocketPunchChargeDetectModel
{
    private readonly Transform _leftFoot;
    private readonly Transform _rightFoot;
    private readonly float _chargeDistanceThreshold;
    private bool _isUppedLeft = false;
    private bool _isUppedRight = false;
    private readonly Subject<Unit> _onUppedFoot;
    public IObservable<Unit> OnUppedFoot => _onUppedFoot;

    public RocketPunchChargeDetectModel(PlayerParams param, Transform leftFoot, Transform rightFoot)
    {
        _chargeDistanceThreshold = param.RocketPunchChargeDistanceThreshold;
        _leftFoot = leftFoot;
        _rightFoot = rightFoot;
        _onUppedFoot = new Subject<Unit>();

        _leftFoot.UpdateAsObservable()
            .Subscribe(_ =>
            {
                var dist = MathF.Abs(_leftFoot.position.y - rightFoot.position.y);
                if(IsUpLeft(dist))
                {
                    _isUppedLeft = true;
                    _isUppedRight = false;
                    _onUppedFoot.OnNext(Unit.Default);
                }
                else if(IsUpRight(dist))
                {
                    _isUppedRight = true;
                    _isUppedLeft = false;
                    _onUppedFoot.OnNext(Unit.Default);
                }
            })
            .AddTo(_leftFoot);
    }
    
    private bool IsUpLeft(float dist) 
        => _leftFoot.position.y > _rightFoot.position.y 
            && dist > _chargeDistanceThreshold
            && !_isUppedLeft;
    
    private bool IsUpRight(float dist)
        => _rightFoot.position.y > _leftFoot.position.y 
            && dist > _chargeDistanceThreshold
            && !_isUppedRight;
}
