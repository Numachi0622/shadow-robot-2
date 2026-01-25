using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace InGame.System.UI
{
    public class NormalBattleView : MonoBehaviour
    {
        [SerializeField] private PopupView _popupView;
        [SerializeField] private CanvasGroup _gameStartMessageCanvasGroup;
        [SerializeField] private RectTransform _gameStartMessageTransform;
        [SerializeField] private TextMeshProUGUI _remainingEnemiesCountText;
        [SerializeField] private CanvasGroup _remainingEnemiesCountCanvasGroup;
        private Sequence _remainingEnemiesCountSequence;

        public void Initialize()
        {
            _popupView.InitializeAndHide();
            _gameStartMessageCanvasGroup.alpha = 0f;
            _remainingEnemiesCountCanvasGroup.alpha = 0f;
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
        
        public async UniTask ShowAndHideRemainingEnemiesCountViewAsync(int initialValue, UniTaskCompletionSource completionSource)
        {
            _remainingEnemiesCountText.text = initialValue.ToString();
            _remainingEnemiesCountCanvasGroup.alpha = 1f;
            await completionSource.Task;
            _remainingEnemiesCountCanvasGroup.alpha = 0f;
        }
        
        public void UpdateRemainingEnemiesCount(int count)
        {
            _remainingEnemiesCountText.text = count.ToString();
            
            var tf = _remainingEnemiesCountText.transform;
            _remainingEnemiesCountSequence?.Kill();
            _remainingEnemiesCountSequence = DOTween.Sequence()
                .SetLink(gameObject)
                .Append(tf.DOScale(Vector3.one * 1.3f, 0.3f).SetLoops(2, LoopType.Yoyo));
        }
    }
}