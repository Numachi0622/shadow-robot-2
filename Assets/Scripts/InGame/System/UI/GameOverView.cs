using DG.Tweening;
using UnityEngine;

namespace InGame.System.UI
{
    public class GameOverView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _backgroundCanvasGroup;
        [SerializeField] private CanvasGroup _textViewCanvasGroup;
        private Sequence _showSequence;

        public void Initialize()
        {
            _backgroundCanvasGroup.alpha = 0f;
            _textViewCanvasGroup.alpha = 0f;
        }
        
        public void ShowGameOverView()
        {
            _showSequence?.Kill();
            _showSequence = DOTween.Sequence()
                .SetLink(gameObject)
                .Append(_backgroundCanvasGroup.DOFade(1f, 0.5f))
                .Append(_textViewCanvasGroup.DOFade(1f, 1f));
        }
    }
}