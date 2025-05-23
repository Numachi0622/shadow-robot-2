using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PunchGaugeView : MonoBehaviour
    {
        [SerializeField] private Image _gauge;
        private Sequence _gaugeSequence;

        public void Initialize()
        {
            _gauge.fillAmount = 0;
        }
    
        public void UpdateGauge(float value)
        {
            var rate = value / GameConst.MAX_PUNCH_POINT;
        
            _gaugeSequence?.Kill();

            _gaugeSequence = DOTween.Sequence()
                .SetLink(gameObject)
                .Append(_gauge.DOFillAmount(rate, 0.3f).SetEase(Ease.OutCubic));
        }
    }
}
