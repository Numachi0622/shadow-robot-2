using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace InGame.System.UI
{
    public class NormalBattleView : MonoBehaviour
    {
        [SerializeField] private PopupView _popupView;
        [SerializeField] private CanvasGroup _gameStartMessageCanvasGroup;
        [SerializeField] private RectTransform _gameStartMessageTransform;

        public void Initialize()
        {
            _popupView.InitializeAndHide();
            _gameStartMessageCanvasGroup.alpha = 0f;
        }

        public async UniTask ShowAndHideBattleStartViewAsync()
        {
            await _popupView.ShowAndHideAsync(2f);
            await ShowAndHideGameStartMessageAsync();
        }
        
        private async UniTask ShowAndHideGameStartMessageAsync()
        {
            await DOTween.Sequence()
                .Append(_gameStartMessageCanvasGroup.DOFade(1f, 0.3f))
                .Append(_gameStartMessageTransform.DOScale(Vector3.one * 1.2f, 0.4f).SetLoops(2, LoopType.Yoyo))
                .Append(_gameStartMessageCanvasGroup.DOFade(0f, 0.3f))
                .ToUniTask();
        }
    }
}