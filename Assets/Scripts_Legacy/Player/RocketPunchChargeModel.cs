using UniRx;
using UnityEngine;

public class RocketPunchChargeModel
{
    private ReactiveProperty<float> _chargeValue;
    public IReadOnlyReactiveProperty<float> ChargeValue => _chargeValue;
    public float CurrentValue => _chargeValue.Value;

    public RocketPunchChargeModel(float initValue = 0)
    {
        _chargeValue = new ReactiveProperty<float>(initValue);
    }

    public void Increase(float value)
    {
        _chargeValue.Value = Mathf.Clamp(_chargeValue.Value + value, 0, GameConst.MAX_ROCKET_PUNCH_CHARGE);
    }

    public void Decrease(float value)
    {
        _chargeValue.Value = Mathf.Clamp(_chargeValue.Value - value, 0, GameConst.MAX_ROCKET_PUNCH_CHARGE);
    }

    public void Reset()
    {
        _chargeValue.Value = 0f;
    }
}
