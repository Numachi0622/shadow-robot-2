using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace InGame.Character
{
    public class HitPointView : MonoBehaviour
    {
        [SerializeField] private Image _gauge;
        [SerializeField] private Image _diffGauge;
        [SerializeField] private TextMeshProUGUI _hpText;
        private Tween _hpGaugeSequence;

        public void Initialize(int currentHp, int maxHp)
        {
            _hpText.text = $"{currentHp} / {maxHp}";
        }

        public void UpdateHp(int currentHp, int maxHp)
        {
            _hpText.text = $"{currentHp} / {maxHp}";

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
}
