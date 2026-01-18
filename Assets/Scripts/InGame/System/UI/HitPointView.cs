using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace InGame.System.UI
{
    public class HitPointView : MonoBehaviour, IVisibilityController
    {
        [SerializeField] private HitPointElementView[] _elementViews;
        [SerializeField] private RectTransform _rectTransform;

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

        public async UniTask ShowAsync(IVisibilityContext context = null)
        {
            if (context is not HitPointVisibilityContext hpContext) return;
            
            Show();
            var target = hpContext.TargetPositionX;
            await _rectTransform.DOAnchorPosX(target, 0.5f).ToUniTask();
        }

        public async UniTask HideAsync(IVisibilityContext context = null)
        {
            if (context is not HitPointVisibilityContext hpContext) return;

            var target = hpContext.TargetPositionX;
            await _rectTransform.DOAnchorPosX(target, 0.5f).ToUniTask();
            Hide();
        }
    }
    
    public struct HitPointVisibilityContext : IVisibilityContext
    {
        public readonly float TargetPositionX;
        public HitPointVisibilityContext(float targetPositionX)
        {
            TargetPositionX = targetPositionX;
        }
    }
}
