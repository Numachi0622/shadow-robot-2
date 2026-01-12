using UnityEngine;
using UnityEngine.UI;

namespace InGame.System.UI
{
    public class HitPointView : MonoBehaviour
    {
        [SerializeField] private HitPointElementView[] _elementViews;

        public void UpdateHp(float hpRate)
        {
            float totalFill = hpRate * _elementViews.Length;
            int fullImages = Mathf.FloorToInt(totalFill);
            float partialFill = totalFill - fullImages;

            for (int i = 0; i < _elementViews.Length; i++)
            {
                if (i < fullImages)
                {
                    // 完全に埋める
                    _elementViews[i].UpdateRate(1f);
                }
                else if (i == fullImages)
                {
                    // 部分的に埋める
                    _elementViews[i].UpdateRate(partialFill);
                }
                else
                {
                    // 空にする
                    _elementViews[i].UpdateRate(0f);
                }
            }
        }
    }
}
