using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class HitPointView : MonoBehaviour
{
    [SerializeField] private Image _gauge;
    [SerializeField] private Image _diffGauge;
    private Tween _hpGaugeSequence;

    public void Initialize()
    {
    }

    public void UpdateHp(int currentHp, int maxHp)
    {
        var rate = (float)currentHp / maxHp;
        
        _hpGaugeSequence?.Kill();

        _hpGaugeSequence = DOTween.Sequence()
            .SetLink(gameObject)
            .SetUpdate(true)
            .Append(_gauge.DOFillAmount(rate, 0.075f))
            .AppendInterval(0.1f)
            .Append(_diffGauge.DOFillAmount(rate, 0.5f));
    }
}
