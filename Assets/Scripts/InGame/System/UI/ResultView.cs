using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace InGame.System.UI
{
    public class ResultView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _missionCompleteMessageCanvasGroup;
        [SerializeField] private CanvasGroup _contentBackgroundCanvasGroup;
        [SerializeField] private TextMeshProUGUI _buildingCountText;
        [SerializeField] private CanvasGroup _buildingCountCanvasGroup;
        private Transform _missionCompleteMessageTransform;

        public void Initialize()
        {
            _missionCompleteMessageTransform = _missionCompleteMessageCanvasGroup.transform;
            _missionCompleteMessageCanvasGroup.alpha = 0f;
            _contentBackgroundCanvasGroup.alpha = 0f;
            _buildingCountCanvasGroup.alpha = 0f;
        }

        public async UniTask ResultViewAsync(int buildingCount)
        {
            var initScale = Vector3.one * 1.5f;
            var targetPos = _missionCompleteMessageTransform.localPosition;
            _missionCompleteMessageTransform.localScale = initScale;
            _missionCompleteMessageTransform.localPosition = Vector2.zero;
            _buildingCountText.transform.localScale = initScale;
            
            _buildingCountText.text = buildingCount.ToString();

            await DOTween.Sequence()
                .SetLink(gameObject)
                .Append(_missionCompleteMessageTransform.DOScale(Vector3.one, 1f))
                .Join(_missionCompleteMessageCanvasGroup.DOFade(1f, 1.5f))
                .AppendInterval(1f)

                .Append(_missionCompleteMessageTransform.DOLocalMove(targetPos, 1f))
                .Append(_contentBackgroundCanvasGroup.DOFade(1f, 1f))
                .AppendInterval(1f)

                .Append(_buildingCountCanvasGroup.DOFade(1f, 0.5f))
                .Join(_buildingCountText.transform.DOScale(Vector3.one, 0.7f))

                .ToUniTask();
        }
    }
}