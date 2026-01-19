using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace InGame.System.UI
{
    public class PopupView : MonoBehaviour, IVisibilityController
    {
        [SerializeField] private CanvasGroup _contentCanvasGroup;
        
        public bool IsActive => gameObject.activeSelf;

        public void InitializeAndHide()
        {
            transform.localScale = new Vector3(0, 1, 1);
            _contentCanvasGroup.alpha = 0f;
            Hide();
        }
        
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
            transform.localScale = new Vector3(0, 1, 1);
            _contentCanvasGroup.alpha = 0f;
            Show();
            
            await transform.DOScaleX(1f, 0.4f).ToUniTask();
            await _contentCanvasGroup.DOFade(1f, 0.5f).ToUniTask();
        }

        public async UniTask HideAsync(IVisibilityContext context = null)
        {
            await _contentCanvasGroup.DOFade(0f, 0.5f).ToUniTask();
            await transform.DOScaleX(0f, 0.4f).ToUniTask();
            Hide();
        }

        public async UniTask ShowAndHideAsync(float displayTime, IVisibilityContext context = null)
        {
            await ShowAsync(context);
            await UniTask.Delay(TimeSpan.FromSeconds(displayTime));
            await HideAsync(context);
        }
    }
}