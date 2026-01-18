using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.System.UI
{
    public class PoseMatchView : MonoBehaviour, IVisibilityController
    {
        [SerializeField] private Image _matchGauge;
        [SerializeField] private Image _timeGauge;
        [SerializeField] private Transform _matchGaugeRoot;
        [SerializeField] private CanvasGroup _rootCanvasGroup;
        private Sequence _poseMatchSuccessSequence;
        
        public void UpdateMatchGauge(float matchRate)
        {
            var clampedRate = Mathf.Clamp01(matchRate);
            _matchGauge.fillAmount = clampedRate;
        }

        public void UpdateTimeGauge(float timeRate)
        {
            var clampedRate = Mathf.Clamp01(timeRate);
            _timeGauge.fillAmount = clampedRate;
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
            Show();
            _rootCanvasGroup.alpha = 0f;
            await _rootCanvasGroup.DOFade(1f, 0.5f).ToUniTask();
        }

        public async UniTask HideAsync(IVisibilityContext context = null)
        {
            _rootCanvasGroup.alpha = 1f;
            await _rootCanvasGroup.DOFade(0f, 0.5f).ToUniTask();
            Hide();
            Refresh();
        }

        public async UniTask PoseMatchSuccessAnimationAsync()
        {
            _poseMatchSuccessSequence?.Kill();
            _poseMatchSuccessSequence = PoseMatchSuccessSequence;
            
            await _poseMatchSuccessSequence.Play().ToUniTask();
        }
        
        private Sequence PoseMatchSuccessSequence => DOTween.Sequence()
            .Append(_matchGaugeRoot.DOScale(Vector3.one * 1.5f, 0.5f).SetLoops(2, LoopType.Yoyo))
            .AppendInterval(0.5f)
            .Append(_matchGaugeRoot.DOScale(Vector3.zero, 1f))
            .Join(_matchGaugeRoot.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360));

        private void Refresh()
        {
            _matchGauge.fillAmount = 1f;
            _timeGauge.fillAmount = 1f;
            _matchGaugeRoot.localScale = Vector3.one;
            _matchGaugeRoot.localRotation = Quaternion.identity;
            _rootCanvasGroup.alpha = 1f;
        } 
    }
}