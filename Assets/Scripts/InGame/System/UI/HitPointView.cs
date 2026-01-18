using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.System.UI
{
    public class HitPointView : MonoBehaviour, IVisibilityController
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

        public bool IsActive => gameObject.activeSelf;
        public void Show()
        {
            if (gameObject.activeSelf) return;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (!gameObject.activeSelf) return;
            gameObject.SetActive(false);
        }

        public UniTask ShowAsync()
        {
            // todo: 左右に避けるアニメーションを実装予定
            Show();
            return UniTask.CompletedTask;
        }

        public UniTask HideAsync()
        {
            // todo: 左右に避けるアニメーションを実装予定
            Hide();
            return UniTask.CompletedTask;
        }
    }
}
