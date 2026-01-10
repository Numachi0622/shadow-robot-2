using UnityEngine;
using UnityEngine.UI;

namespace InGame.System.UI
{
    public class HitPointView : MonoBehaviour
    {
        [SerializeField] private Image[] _rateImage;

        public void UpdateHp(float hpRate)
        {
            float totalFill = hpRate * _rateImage.Length;
            int fullImages = Mathf.FloorToInt(totalFill);
            float partialFill = totalFill - fullImages;

            for (int i = 0; i < _rateImage.Length; i++)
            {
                if (i < fullImages)
                {
                    // 完全に埋める
                    _rateImage[i].fillAmount = 1f;
                }
                else if (i == fullImages)
                {
                    // 部分的に埋める
                    _rateImage[i].fillAmount = partialFill;
                }
                else
                {
                    // 空にする
                    _rateImage[i].fillAmount = 0f;
                }
            }
        }
    }
}
