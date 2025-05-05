using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HitPointView : MonoBehaviour
{
    [SerializeField] private Image _gauge;
    [SerializeField] private Image _diffGauge;
    private Tween _hpGaugeSequence;

    public void UpdateHp(int currentHp, int maxHp)
    {
        var rate = (float)currentHp / maxHp;
        
        _hpGaugeSequence?.Kill();

        _hpGaugeSequence = DOTween.Sequence()
            .Append(_gauge.DOFillAmount(rate, 0.075f))
            .Append(_diffGauge.DOFillAmount(rate, 0.5f).SetDelay(0.15f));
    }
}
