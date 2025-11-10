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
        private int _maxHp; 

        public void Initialize(int currentHp, int maxHp)
        {
            _maxHp = maxHp;
            _hpText.text = $"{currentHp} / {_maxHp}";
        }

        public void UpdateHp(int currentHp)
        {
            _hpText.text = $"{currentHp} / {_maxHp}";

            var rate = (float)currentHp / _maxHp;

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
