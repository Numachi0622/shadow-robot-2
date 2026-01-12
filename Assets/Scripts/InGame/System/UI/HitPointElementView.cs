using UnityEngine;
using UnityEngine.UI;

namespace InGame.System.UI
{
    public class HitPointElementView : MonoBehaviour
    {
        [SerializeField] private Image _rageImage;

        public void UpdateRate(float rate)
        {
            var clampedRate = Mathf.Clamp01(rate);
            _rageImage.fillAmount = clampedRate;
        }
    }
}