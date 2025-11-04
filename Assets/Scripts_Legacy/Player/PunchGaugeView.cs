using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Player
{
    public class PunchGaugeView : MonoBehaviour
    {
        [SerializeField] private Image _gauge;
        [SerializeField] private Transform _leftArmTransform;
        [SerializeField] private Transform _rightArmTransform;
        private Sequence _gaugeSequence;

        public void Initialize()
        {
            _gauge.fillAmount = 0;
        }
    
        public void UpdateGauge(float value)
        {
            var rate = value / GameConst.MAX_PUNCH_POINT;
            var targetScale = Vector3.Lerp(Vector3.one, Vector3.one * GameConst.MAX_ARM_SCALE, rate);
        
            _gaugeSequence?.Kill();

            _gaugeSequence = DOTween.Sequence()
                .SetLink(gameObject)
                .Append(_gauge.DOFillAmount(rate, 0.3f).SetEase(Ease.OutCubic))
                .Join(_leftArmTransform.DOScale(targetScale, 0.7f).SetEase(Ease.OutCubic))
                .Join(_rightArmTransform.DOScale(targetScale, 0.7f).SetEase(Ease.OutCubic));
        }
    }
}
