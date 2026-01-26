using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace InGame.System.UI
{
    public class BossBattleView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _warningCanvasGroup;
        [SerializeField] private TextMeshProUGUI _warningText;
        [SerializeField] private PopupView _popupView;
        [SerializeField] private PartsDescriptionView _partsDescriptionView;
        
        public void Initialize()
        {
            _popupView.InitializeAndHide();
        }

        public async UniTask ShowWarningAsync(string message, int fadeCount, float durationPerCount)
        {
            _warningText.text = message;
            await DOTween.Sequence()
                .Append(_warningCanvasGroup.DOFade(1f, durationPerCount).SetLoops(2 * fadeCount, LoopType.Yoyo))
                .ToUniTask();
        }
        
        public async UniTask ShowAndHidePopupAsync(float displayTime)
        {
            await _popupView.ShowAndHideAsync(displayTime);
        }
        
        public async UniTask ShowAndHidePartsDescriptionAsync(IReadOnlyList<(string title, string message)> descriptions)
        {
            await _partsDescriptionView.ShowAndHideAsync(descriptions);
        }
    }
}