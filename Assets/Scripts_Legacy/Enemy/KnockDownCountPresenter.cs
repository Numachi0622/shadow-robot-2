using System;
using UniRx;
using UnityEngine;
using Utility;

public class KnockDownCountPresenter : MonoBehaviour
{
    private KnockDownCountModel _model;
    public bool IsBossKnocedDown { get; set; } = false;
    public int CurrentKnockDownCount => _model.KnockDownCount.Value;
    public Action OnKnockDownBossGenerationCount;

    public void Initialize()
    {
        _model = new KnockDownCountModel();
        
        Bind();
    }

    private void Bind()
    {
        _model.KnockDownCount
            .Where(count => count == GameConst.BOSS_GENERATION_KNOCK_DOWN_COUNT)
            .Subscribe(count => OnKnockDownBossGenerationCount?.Invoke())
            .AddTo(this);
    }

    public void KnockDownCount()
    {
        _model.Add();
    }
}
