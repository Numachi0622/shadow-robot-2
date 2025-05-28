using System;
using UniRx;
using UnityEngine;

public class KnockDownCountModel
{
    private ReactiveProperty<int> _knockDownCount;
    public IReadOnlyReactiveProperty<int> KnockDownCount => _knockDownCount;
    
    public KnockDownCountModel()
    {
        _knockDownCount = new ReactiveProperty<int>(0);
    }

    public void Add()
    {
        _knockDownCount.Value++;        
    }
}
