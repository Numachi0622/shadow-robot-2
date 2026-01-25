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

        public async UniTask ShowWarningAsync(string message, int fadeCount, float durationPerCount)
        {
            _warningText.text = message;
            await DOTween.Sequence()
                .Append(_warningCanvasGroup.DOFade(1f, durationPerCount).SetLoops(2 * fadeCount, LoopType.Yoyo))
                .ToUniTask();
        }
    }
}