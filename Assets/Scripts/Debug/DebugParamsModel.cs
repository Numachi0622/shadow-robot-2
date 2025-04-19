using UnityEngine;
using UniRx;

public class DebugParamsModel
{
    private ReactiveProperty<int> _trackedCount;
    public IReadOnlyReactiveProperty<int> TrackedCount => _trackedCount;

    private ReactiveProperty<int> _rightTrackedId;
    public IReadOnlyReactiveProperty<int> RightTrackedId => _rightTrackedId;
    
    private ReactiveProperty<int> _leftTrackedId;
    public IReadOnlyReactiveProperty<int> LeftTrackedId => _leftTrackedId;
    
    public DebugParamsModel()
    {
        _trackedCount = new ReactiveProperty<int>(0);
        _rightTrackedId = new ReactiveProperty<int>(0);
        _leftTrackedId = new ReactiveProperty<int>(0);
    }
    
    public void SetTrackedCount(int count)
    {
        _trackedCount.Value = count;
    }
    
    public void SetRightTrackedId()
    {
        if(_trackedCount.Value == 0) return;
        _rightTrackedId.Value = (_rightTrackedId.Value + 1) % _trackedCount.Value;
    }
    
    public void SetLeftTrackedId()
    {
        if(_trackedCount.Value == 0) return;
        _leftTrackedId.Value = (_leftTrackedId.Value + 1) % _trackedCount.Value;
    }
}
